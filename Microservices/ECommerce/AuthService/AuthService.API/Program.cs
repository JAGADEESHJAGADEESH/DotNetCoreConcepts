using AuthService.Application.Services.PasswordService;
using AuthService.Application.Services.RefreshTokenService;
using AuthService.Application.Services.TokenService;
using AuthService.Application.Services.UserService;
using AuthService.Core.Models;
using AuthService.Infrastructure.Repositories.RefreshTokenRepository;
using AuthService.Infrastructure.Repositories.UserRepository;
using BuildingBlocks.ExceptionsHelper;
using DatabaseAccess.DapperRepository;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Configure CORS for React app
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173") // <-- change to your React app origin
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtSettings"));

builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        var jwt = builder.Configuration
            .GetSection("JwtSettings")
            .Get<JwtSettings>();

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwt!.Issuer,
            ValidAudience = jwt.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwt.Key)),
            RoleClaimType = ClaimTypes.Role
        };
    });

builder.Services.AddAuthorization();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


string connectionString = builder.Configuration.GetConnectionString("AuthDB") ?? string.Empty;

builder.Services.AddSingleton<IDapperRepository>(sp =>
new DapperRepository(connectionString));

//Services
builder.Services.AddSingleton<IUserService, UserService>();
builder.Services.AddSingleton<IPasswordService, PasswordService>();
builder.Services.AddSingleton<ITokenService, TokenService>();
builder.Services.AddSingleton<IRefreshTokenService, RefreshTokenService>();


//Repositories
builder.Services.AddSingleton<IUserRepository, UserRepository>();
builder.Services.AddSingleton<IRefreshTokenRepository, RefreshTokenRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Global Exception Handler
app.UseExceptionHandler(builder =>
{
    builder.Run(async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsJsonAsync(new
        {
            error = "Internal server error"
        });
    });
});

app.UseGlobalExceptionMiddleware();


// Enable CORS (must be before authentication/authorization)
app.UseCors("AllowReactApp");

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
