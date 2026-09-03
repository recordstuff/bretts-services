using bretts_services.Mappings;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.OpenApi;

const string sourceContext = "SourceContext";
const string outputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] ({SourceContext}) {Message:lj}{NewLine}{Exception}";
const string dockerSecretsPath = "/run/secrets";

#if DEBUG

Serilog.Debugging.SelfLog.Enable(message => 
{
    Debug.WriteLine(message);
    Debugger.Break();
});

#endif

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(outputTemplate: outputTemplate)
    .CreateBootstrapLogger();

Log.Logger.ForContext(sourceContext, nameof(Program)).Information("Creating Builder.");

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddKeyPerFile(dockerSecretsPath, optional: true);

// global exception handler

builder.Services.AddExceptionHandler<ExceptionHandler>();

Log.Logger.ForContext(sourceContext, nameof(Program)).Information("Replacing Bootstrap Logger.");

var connectionString = builder.Configuration.GetConnectionString("BrettsDbConnection");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "The ConnectionStrings:BrettsDbConnection secret is required. Configure it with .NET User Secrets for development or a Docker secret for production.");
}

var junkEmailCleanerConnectionString = builder.Configuration.GetConnectionString("JunkEmailCleanerDbConnection");

if (string.IsNullOrWhiteSpace(junkEmailCleanerConnectionString))
{
    throw new InvalidOperationException(
        "The ConnectionStrings:JunkEmailCleanerDbConnection secret is required. Configure it with .NET User Secrets for development or a Docker secret for production.");
}

// Serilog

var sinkOptions = new MSSqlServerSinkOptions 
{
    AutoCreateSqlTable = true,
    TableName = "Logs", 
};

var columnOptions = new ColumnOptions();

columnOptions.Store.Remove(StandardColumn.Properties);
columnOptions.Store.Add(StandardColumn.LogEvent);
columnOptions.PrimaryKey = columnOptions.Id;
columnOptions.PrimaryKey.NonClusteredIndex = true;
columnOptions.TimeStamp.NonClusteredIndex = true;

columnOptions.AdditionalColumns = new List<SqlColumn>
{
    new SqlColumn { DataType = SqlDbType.VarChar, ColumnName = "SourceContext" },
    new SqlColumn { DataType = SqlDbType.VarChar, ColumnName = "ServerName" },
    new SqlColumn { DataType = SqlDbType.VarChar, ColumnName = "Environment" },
};

builder.Services.AddSerilog((services, lc) => lc
    .ReadFrom.Configuration(builder.Configuration)
    .ReadFrom.Services(services)
    .Enrich.WithProperty("ServerName", Environment.MachineName)
    .Enrich.WithProperty("Environment", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production")
    .Enrich.FromLogContext()
    .WriteTo.Console(outputTemplate: outputTemplate)
    .WriteTo.MSSqlServer(connectionString: connectionString, sinkOptions, columnOptions: columnOptions));

// CORS

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            var origins = builder.Configuration.GetSection("AllowedCorsOrigins").Get<string[]>();

            if (origins != null)
            {
                policy.WithOrigins(origins)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            }
        });
});

// authentication and authorization

var userOptionsSection = builder.Configuration.GetSection(nameof(UserOptions));
var userOptions = userOptionsSection.Get<UserOptions>();

if (userOptions == null)
{
    throw new InvalidOperationException(
        "The UserOptions configuration section is required.");
}

if (string.IsNullOrWhiteSpace(userOptions.SigningKey))
{
    throw new InvalidOperationException(
        "The UserOptions:SigningKey secret is required. Configure it with .NET User Secrets for development or a Docker secret for production.");
}

builder.Services.AddAuthentication(auth =>
{
    auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = userOptions.Issuer,
        ValidateAudience = true,
        ValidAudience = userOptions.Audience,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(userOptions.SigningKey))
    };
});

// EF dbcontext

builder.Services.AddDbContext<Entities.BrettsAppContext>(options =>
{
#if DEBUG
    options.EnableSensitiveDataLogging();
#endif
    options.UseSqlServer(connectionString);
    // default this to on but could start with: options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking); 
    // or could default to QueryTrackingBehavior.NoTrackingWithIdentityResolution
});

builder.Services.AddDbContext<Entities.JunkEmailCleanerContext>(options =>
{
    options.UseSqlServer(junkEmailCleanerConnectionString);
});

// our options and services

builder.Services.AddHttpClient<LmStudioClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("LMStudio") ?? string.Empty);
});


builder.Services.Configure<UserOptions>(
    userOptionsSection);

builder.Services.AddScoped<ILogService, LogService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<IChatHistory, SessionChatHistory>();

// Mapperly

builder.Services.AddScoped<RoleMapping>();
builder.Services.AddScoped<UserMapping>();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.None;
});

builder.Services.AddHttpContextAccessor();

builder.Services
    .AddControllers()
    .AddJsonOptions((configure) =>
{
    configure.JsonSerializerOptions.PropertyNamingPolicy = null;
});

// swagger

builder.Services.AddEndpointsApiExplorer(); 
builder.Services.AddSwaggerGen(options =>
{
    var xmlFileName = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlFilePath = Path.Combine(AppContext.BaseDirectory, xmlFileName);

    options.IncludeXmlComments(xmlFilePath, includeControllerXmlComments: true);

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
    });

    options.OperationFilter<SwaggerFilter>();
});

// health checks

builder.Services.AddHealthChecks()
    .AddCheck("liveness", () => HealthCheckResult.Healthy(), ["live"])
    .AddCheck<DeepHealthCheck>("deep", tags: ["deep"]);

// configure the request pipeline using the features that were added above

var app = builder.Build();

app.UseSerilogRequestLogging();

app.UseExceptionHandler("/Error");

app.UseStaticFiles();

// Swagger is intentionally enabled in every environment, including production deploys.
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.DefaultModelsExpandDepth(2);
    options.InjectStylesheet("/swagger-custom.css?v=portrait-frame");
    options.InjectJavascript("/swagger-custom.js?v=portrait-frame");
});

// Serve HTTP here except for dev; Apache handles HTTPS in production.
//app.UseHttpsRedirection();

app.UseForwardedHeaders();

app.UseCors();

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/liveness", new HealthCheckOptions
{
    Predicate = healthCheck => healthCheck.Tags.Contains("live")
});

app.MapHealthChecks("/deep", new HealthCheckOptions
{
    Predicate = healthCheck => healthCheck.Tags.Contains("deep")
});

try
{
    app.Run();
}
catch (Exception ex)
{
    Log.Logger.ForContext(sourceContext, nameof(Program)).Fatal(ex, "Caught fatal exception.  Terminating.");
}
finally
{
    Log.Logger.ForContext(sourceContext, nameof(Program)).Information("Calling CloseAndFlushAsync()");
    await Log.CloseAndFlushAsync();
}






