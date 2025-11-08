using Microsoft.Extensions.DependencyInjection;
using RailVehicleData.Aplication.Interfaces;
using RailVehicleData.Infrastrcture.Repositories;

namespace RailVehicleData.Infrastrcture.DependencyInjection;

/// <summary>
/// Extension methods for registering infrastructure services in the dependency injection container.
/// </summary>
public static class ServiceExtensions
{
    /// <summary>
    /// Adds all infrastructure layer services to the DI container.
    /// This includes repositories and data access abstractions.
    ///
    /// Include this in your Program.cs or Startup.cs:
    ///
    /// services.AddInfrastructureServices();
    /// </summary>
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        // Register Repositories with their interfaces
        services.AddScoped<IVehicleRepository, VehicleRepository>();
        services.AddScoped<IMultipleUnitRepository, MultipleUnitRepository>();

        return services;
    }
}
