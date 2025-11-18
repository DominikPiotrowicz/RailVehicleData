using Microsoft.Extensions.DependencyInjection;
using RailVehicleData.Application.Interfaces;
using RailVehicleData.Application.Mappings;
using RailVehicleData.Application.Services;

namespace RailVehicleData.Application.DependencyInjection;

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
        // Register AutoMapper using built-in extension method
        services.AddAutoMapper(cfg => cfg.AddProfile<AutoMapperConfig>());

        // Register Application Services
        services.AddScoped<IVehicleService, VehicleService>();
        services.AddScoped<IMultipleUnitService, MultipleUnitService>();

        return services;
    }
}
