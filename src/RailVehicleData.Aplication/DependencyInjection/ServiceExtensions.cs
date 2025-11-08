using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using RailVehicleData.Aplication.Interfaces;
using RailVehicleData.Aplication.Mappings;
using RailVehicleData.Aplication.Services;
using RailVehicleData.Infrastrcture.Repositories;

namespace RailVehicleData.Aplication.DependencyInjection;

/// <summary>
/// Extension methods for registering application services in the dependency injection container.
/// </summary>
public static class ServiceExtensions
{
    /// <summary>
    /// Adds all application layer services to the DI container.
    /// Include this in your Program.cs or Startup.cs:
    ///
    /// services.AddApplicationServices();
    /// </summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register AutoMapper
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<AutoMapperConfig>();
        });
        var mapper = mapperConfig.CreateMapper();
        services.AddSingleton(mapper);

        // Register Repositories
        services.AddScoped<VehicleRepository>();
        services.AddScoped<MultipleUnitRepository>();

        // Register Services
        services.AddScoped<IVehicleService, VehicleService>();
        services.AddScoped<IMultipleUnitService, MultipleUnitService>();

        return services;
    }
}
