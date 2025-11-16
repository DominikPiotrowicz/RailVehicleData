using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using RailVehicleData.Application.Interfaces;
using RailVehicleData.Application.Mappings;
using RailVehicleData.Application.Services;
using RailVehicleData.Domain.Interfaces;
using RailVehicleData.Infrastructure.Data;
using RailVehicleData.Infrastructure.Repositories;
using RailVehicleData.WebAPI.Middleware;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Rejestracja repozytoriów
builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();

// Rejestracja serwisów aplikacji
builder.Services.AddScoped<IVehicleService, VehicleService>();

// Konfiguracja AutoMapper
builder.Services.AddSingleton(AutoMapperConfig.Initialize());

// Konfiguracja bazy danych
builder.Services.AddDbContext<RailVehicleDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("RailVehicleCS")));

// Konfiguracja Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
	c.SwaggerDoc("v1", new OpenApiInfo
	{
		Title = "RailVehicleData API",
		Version = "v1",
		Description = "API do zarządzania danymi technicznymi pojazdów szynowych",
		Contact = new OpenApiContact
		{
			Name = "RailVehicleData Team"
		}
	});

	// Włączenie komentarzy XML dla dokumentacji Swagger
	var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
	var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
	if (File.Exists(xmlPath))
	{
		c.IncludeXmlComments(xmlPath);
	}
});

// Konfiguracja CORS (opcjonalnie)
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowAll", builder =>
	{
		builder.AllowAnyOrigin()
			   .AllowAnyMethod()
			   .AllowAnyHeader();
	});
});

var app = builder.Build();

// Globalna obsługa błędów
app.UseGlobalExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI(c =>
	{
		c.SwaggerEndpoint("/swagger/v1/swagger.json", "RailVehicleData API v1");
		c.RoutePrefix = string.Empty; // Swagger UI na root URL
	});
}

app.UseHttpsRedirection();

// Włączenie CORS
app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

// Inicjalizacja bazy danych
using (var scope = app.Services.CreateScope())
{
	var services = scope.ServiceProvider;
	try
	{
		var context = services.GetRequiredService<RailVehicleDbContext>();
		// Automatyczne tworzenie bazy danych i aplikowanie migracji
		context.Database.EnsureCreated();
		app.Logger.LogInformation("Baza danych została zainicjalizowana pomyślnie");
	}
	catch (Exception ex)
	{
		app.Logger.LogError(ex, "Błąd podczas inicjalizacji bazy danych");
	}
}

app.Run();
