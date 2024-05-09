using Microsoft.AspNetCore.Diagnostics.HealthChecks;

const string sourceContext = "SourceContext";
const string outputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] ({SourceContext}) {Message:lj}{NewLine}{Exception}";

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

// global exception handler

builder.Services.AddExceptionHandler<ExceptionHandler>();

Log.Logger.ForContext(sourceContext, nameof(Program)).Information("Replacing Bootstrap Logger.");

var connectionString = builder.Configuration.GetConnectionString("BrettsDbConnection");

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

var userOptions = builder.Configuration
    .GetSection(nameof(UserOptions))
    .Get<UserOptions>()!;

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

// automapper

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// our options and services

builder.Services.Configure<UserOptions>(
    builder.Configuration.GetSection(nameof(UserOptions)));

builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services
    .AddControllers()
    .AddJsonOptions((configure) =>
{
    configure.JsonSerializerOptions.PropertyNamingPolicy = null;
});

// swagger

builder.Services.AddEndpointsApiExplorer(); 
builder.Services.AddSwaggerGen();

// health checks

builder.Services.AddHealthChecks()
    .AddCheck("liveness", () => HealthCheckResult.Healthy(), ["live"])
    .AddCheck<DeepHealthCheck>("deep", tags: ["deep"]);

// configure the request pipeline using the features that were added above

var app = builder.Build();

/*
app.Lifetime.ApplicationStopped.Register(async () =>
{
    Log.Logger.ForContext(sourceContext, nameof(Program)).Information("Calling CloseAndFlushAsync()");

    await Log.CloseAndFlushAsync();
}); */


app.UseSerilogRequestLogging();

app.UseExceptionHandler("/Error");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// TODO: get a cert!
//app.UseHttpsRedirection();

app.UseCors();

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

#if DEBUG

var automapperConfig = app.Services.GetService<AutoMapper.IConfigurationProvider>();

ArgumentNullException.ThrowIfNull(automapperConfig, nameof(AutoMapper.IConfigurationProvider));

automapperConfig.AssertConfigurationIsValid();

#endif

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






