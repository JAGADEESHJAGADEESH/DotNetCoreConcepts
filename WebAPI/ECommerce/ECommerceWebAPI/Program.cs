using System.Text;
using ECommerceWebAPI.Services.Token;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// JWT signing key (replace with secure key in configuration for production)
var jwtSigningKey = "ReplaceWithAStrongRandomKey!ChangeMe";

// Add services to the container.
builder.Services.AddControllers();


// Register token service with the signing key
builder.Services.AddSingleton<ITokenService, TokenService>(provider =>
{
    var logger = provider.GetRequiredService<ILogger<TokenService>>();
    return new TokenService(logger);
});

// Configure authentication with JWT Bearer
var keyBytes = Encoding.UTF8.GetBytes(jwtSigningKey);
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

app.UseHttpsRedirection();

// Add authentication BEFORE authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers(); // Map attribute-routed controllers

app.Run();
