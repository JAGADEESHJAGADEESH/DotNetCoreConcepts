using DatabaseAccess.DapperRepository;
using ProductService.Application.Services.CategoryService;
using ProductService.Application.Services.ProductCatelogService;
using ProductService.Infrastructure.Repositories.CategoryRepository;
using ProductService.Infrastructure.Repositories.ProductRepository;
using BuildingBlocks.ExceptionsHelper;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

string connectionString = builder.Configuration.GetConnectionString("ProductDB") ?? string.Empty;

builder.Services.AddSingleton<IDapperRepository>(sp =>
new DapperRepository(connectionString));

//Services
builder.Services.AddSingleton<IProductCatelogService, ProductCatelogService>();
builder.Services.AddSingleton<ICategoryService, CategoryService>();

//Repositories
builder.Services.AddSingleton<IProductRepository, ProductRepository>();
builder.Services.AddSingleton<ICategoryRepository, CategoryRepository>();

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
