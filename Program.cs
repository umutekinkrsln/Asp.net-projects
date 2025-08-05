
using MapApp.Data;
using MapApp.Repositories;
using MapApp.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using NetTopologySuite;

var builder = WebApplication.CreateBuilder(args);

// CORS ayarı 
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173") 
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

// PostgreSQL bağlantısı
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
        x => x.UseNetTopologySuite()));

// DI
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<MapPointServices>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Map API",
        Version = "v1"
    });
});

builder.Services.AddControllers();
builder.Services.AddAuthorization();

var app = builder.Build();

// CORS aktif et
app.UseCors("AllowFrontend");

// wwwroot/index.html'i göstermesi için:
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Swagger
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Map API v1");
    c.RoutePrefix = "swagger";
});

app.Run();



