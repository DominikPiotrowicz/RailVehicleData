using RailVehicleData.Infrastructure.Data;
using RailVehicleData.Application.DependencyInjection;
using RailVehicleData.Infrastructure.DependencyInjection;
using RailVehicleData.Infrastructure.Seeders;
using Microsoft.EntityFrameworkCore;

var builder = WebApplicationBuilder.CreateBuilder(args);

// Add DbContext
builder.Services.AddDbContext<RailVehicleDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlServerOptions => sqlServerOptions.CommandTimeout(30)
    )
);

// Add application and infrastructure services
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices();

// Add controllers
builder.Services.AddControllers();

// Add Swagger/OpenAPI
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "RailVehicleData API",
        Version = "v1",
        Description = "RESTful API for managing rail vehicles, locomotives, and multiple units (EMU/DMU)",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "RailVehicleData Support"
        }
    });
});

// Add CORS if needed for frontend consumption
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

// Apply migrations and seed database on startup
try
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<RailVehicleDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    // Apply pending migrations
    try
    {
        dbContext.Database.Migrate();
        logger.LogInformation("Database migrations applied successfully.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Failed to apply database migrations. The application will continue, but database operations may fail.");
    }

    // Seed sample data using SeedDataService
    try
    {
        var seedService = new SeedDataService(dbContext, logger);
        var seeded = await seedService.SeedIfEmptyAsync();

        if (seeded)
        {
            var summary = await seedService.GetSeedDataSummaryAsync();
            logger.LogInformation("Database seeding completed. {Summary}", summary.ToString());
        }
    }
    catch (Exception ex)
    {
        logger.LogWarning(ex, "Failed to seed the database. This may occur if the database is not ready or already populated. Application will continue.");
    }
}
catch (Exception ex)
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An unexpected error occurred during database initialization. Application will continue with best effort.");
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "RailVehicleData API v1");
        options.RoutePrefix = string.Empty; // Swagger at root
    });
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();
