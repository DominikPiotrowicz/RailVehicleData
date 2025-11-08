using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using RailVehicleData.Aplication.Interfaces;
using RailVehicleData.Aplication.Mappings;
using RailVehicleData.Aplication.Services;

namespace RailVehicleData.Aplication.DependencyInjection;

/// <summary>
/// Extension methods for registering application layer services in the dependency injection container.
/// This includes business logic services and data transformation (AutoMapper).
///
/// NOTE: Repository registration is handled by Infrastructure.ServiceExtensions.
/// This maintains proper separation of concerns per Clean Architecture.
/// </summary>
public static class ServiceExtensions
{
    /// <summary>
    /// Adds all application layer services to the DI container.
    /// Include this in your Program.cs or Startup.cs:
    ///
    /// services.AddApplicationServices();
    /// services.AddInfrastructureServices();
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

        // Register Application Services
        services.AddScoped<IVehicleService, VehicleService>();
        services.AddScoped<IMultipleUnitService, MultipleUnitService>();

        return services;
    }
}
