using Ecommerce.Application.Services.PasswordService;
using Ecommerce.Application.Services.TokenService;
using Ecommerce.Application.Services.UserService;
using Ecommerce.Core.Models;
using ECommerce.Infrastructure.TokenRepository;
using ECommerce.Infrastructure.UserRepository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// JWT signing key (replace with secure key in configuration for production)
var jwtSigningKey = builder.Configuration["Jwt:Key"] ?? "ReplaceWithAStrongRandomKey!ChangeMe";
var keyBytes = Encoding.UTF8.GetBytes(jwtSigningKey);
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);



// Add services to the container.
builder.Services.AddControllers();

// repositories
builder.Services.AddSingleton<ITokenRepository, TokenRepository>();
builder.Services.AddSingleton<IUserRepository, UserRepository>();

//services
builder.Services.AddSingleton<ITokenService, TokenService>();
builder.Services.AddSingleton<IUserService, UserService>();
builder.Services.AddSingleton<IPasswordService, PasswordService>();

builder.Services.Configure<FileStorageOptions>(
    builder.Configuration.GetSection("FileStorage"));


// Configure authentication with JWT Bearer
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(keyBytes)
        };
    });

builder.Services.AddAuthorization();

// Swagger (kept from existing file)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Optional: Add custom API info
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ECommerce",
        Version = "v1",
        Description = "A description of your API."
    });
});

var fileOptions = builder.Configuration
    .GetSection("FileStorage")
    .Get<FileStorageOptions>();

// Add CORS service
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173") // React dev server
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
});


var app = builder.Build();


// Configure the HTTP request pipeline.
// 3. Enable middleware to serve generated Swagger as a JSON endpoint
app.UseSwagger();

// 4. Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.)
// Make sure this is called AFTER app.UseSwagger()
app.UseSwaggerUI(options =>
{
    // Optional: Set Swagger UI to the app's root URL (e.g., http://localhost:port/)
    // options.RoutePrefix = string.Empty;
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Your API V1");
});



app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(fileOptions.ImagePath),
    RequestPath = "/product-images"
});

app.UseHttpsRedirection();

app.UseCors("AllowReactApp");

// Add authentication BEFORE authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers(); // Map attribute-routed controllers

app.Run();
