
var builder = WebApplication.CreateBuilder(args);

// global exception handler

builder.Services.AddExceptionHandler<ExceptionHandler>();

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

builder.Services.AddDbContext<BrettsAppContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("BrettsDbConnection")));

// automapper

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// our options and services

builder.Services.Configure<UserOptions>(
    builder.Configuration.GetSection(nameof(UserOptions)));

builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddControllers();

// swagger

builder.Services.AddEndpointsApiExplorer(); 
builder.Services.AddSwaggerGen();

// configure the request pipeline using the features that were added above

var app = builder.Build();

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

app.Run();
