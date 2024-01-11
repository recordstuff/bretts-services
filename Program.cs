
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

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

builder.Services.AddDbContext<BrettsAppContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("BrettsDbConnection")));

builder.Services.Configure<UserOptions>(
    builder.Configuration.GetSection(nameof(UserOptions)));

builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// get a cert!
//app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
