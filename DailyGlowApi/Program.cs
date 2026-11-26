using System.Text;
using System.Text.Json.Serialization;
using DailyGlowApi.Data;
using DailyGlowApi.Models;
using DailyGlowApi.Repositories;
using DailyGlowApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Send enums as words, not numbers
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter(allowIntegerValues: false));
    });

builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Dependency injection: ask for the interface, get the class
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IRoutineRepository, RoutineRepository>();
builder.Services.AddScoped<IRoutineService, RoutineService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

// Login with JWT.
// Based on Microsoft Learn: Configure JWT bearer authentication in ASP.NET Core
// https://learn.microsoft.com/aspnet/core/security/authentication/configure-jwt-bearer-authentication
// The secret key is not in the repo. It comes from user-secrets (development)
// or the Jwt__Key environment variable (hosting).
// https://learn.microsoft.com/aspnet/core/security/app-secrets
var jwtSection = builder.Configuration.GetSection("Jwt");
builder.Services.Configure<JwtSettings>(jwtSection);
var jwt = jwtSection.Get<JwtSettings>() ?? new JwtSettings();

if (jwt.Key.Length < 32)
{
    throw new InvalidOperationException(
        "Jwt:Key is missing or too short (use 32 or more characters). " +
        "Set it in Manage User Secrets, or in the Jwt__Key environment variable.");
}

// Check the token on each request: issuer, audience, signature and expiry
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwt.Issuer,
            ValidateAudience = true,
            ValidAudience = jwt.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Create the database tables when the app starts
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Show the web page in the wwwroot folder (index.html is the home page).
// Based on Microsoft Learn: Static files in ASP.NET Core
// https://learn.microsoft.com/aspnet/core/fundamentals/static-files
app.UseDefaultFiles();
app.UseStaticFiles();

// First read the token (who are you?), then check [Authorize] (are you allowed?)
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
