using Inventory.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Inventory.Application.Abstractions.Persistence;
using Inventory.Application.Features.Categories.Commands;
using Inventory.Application.Features.Categories.Queries;
using Inventory.Application.Features.Inventory.Commands;
using Inventory.Application.Features.Products.Commands;
using Inventory.Application.Features.Products.Queries;
using Inventory.Infrastructure.Persistence.Repositories;
using Inventory.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddScoped<IProductQueryRepository, ProductQueryRepository>();
builder.Services.AddScoped<IProductCommandRepository, ProductCommandRepository>();
builder.Services.AddScoped<ICategoryQueryRepository, CategoryQueryRepository>();
builder.Services.AddScoped<ICategoryCommandRepository, CategoryCommandRepository>();
builder.Services.AddScoped<IInventoryMovementCommandRepository, InventoryMovementCommandRepository>();

builder.Services.AddScoped<CreateProductHandler>();
builder.Services.AddScoped<UpdateProductHandler>();
builder.Services.AddScoped<DeactivateProductHandler>();
builder.Services.AddScoped<GetAllProductsHandler>();
builder.Services.AddScoped<GetProductByIdHandler>();

builder.Services.AddScoped<CreateCategoryHandler>();
builder.Services.AddScoped<UpdateCategoryHandler>();
builder.Services.AddScoped<DeleteCategoryHandler>();
builder.Services.AddScoped<GetAllCategoriesHandler>();
builder.Services.AddScoped<GetCategoryByIdHandler>();

builder.Services.AddScoped<RegisterInventoryMovementHandler>();

var connectionString = builder.Configuration.GetConnectionString("InventoryDb");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "The InventoryDb connection string is not configured.");
}
builder.Services.AddDbContext<InventoryDbContext>(options =>
    options.UseSqlServer(connectionString));


// Add services to the container.

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

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
