using AuthService.Application.Services.PasswordService;
using AuthService.Application.Services.UserService;
using AuthService.Infrastructure.Repositories.UserRepository;
using DatabaseAccess.DapperRepository;
using BuildingBlocks.ExceptionsHelper;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

string connectionString = builder.Configuration.GetConnectionString("AuthDB") ?? string.Empty;

builder.Services.AddSingleton<IDapperRepository>(sp =>
new DapperRepository(connectionString));

//Services
builder.Services.AddSingleton<IUserService, UserService>();
builder.Services.AddSingleton<IPasswordService, PasswordService>();

//Repositories
builder.Services.AddSingleton<IUserRepository, UserRepository>();

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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
