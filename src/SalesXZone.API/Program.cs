using Microsoft.EntityFrameworkCore;
using SalesXZone.Application.Interfaces;
using SalesXZone.Application.Services;
using SalesXZone.Infrastructure.Data;
using SalesXZone.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
// repository that calls stored proc (uses IConfiguration internally)
builder.Services.AddScoped<IItemRepository, StoredProcItemRepository>();
// application service
builder.Services.AddScoped<IItemMasterService, ItemMasterService>();

// Register Repositories and Unit of Work
//builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Register Services
//builder.Services.AddScoped<IProductService, ProductService>();

// Add CORS if needed
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.RoutePrefix = string.Empty; // Swagger UI at "/"
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "SalesXZone API V1");
});

// Configure the HTTP request pipeline
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();
//app.MapGet("/swagger/", () => "SalesXZone API is running successfully.");

app.Run();