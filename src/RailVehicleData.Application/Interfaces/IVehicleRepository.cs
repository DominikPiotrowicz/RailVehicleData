using RailVehicleData.Domain.Entities;

namespace RailVehicleData.Application.Interfaces;

/// <summary>
/// Repository abstraction for Vehicle aggregate operations.
/// Provides type-safe data access methods for vehicles and their traction systems.
/// </summary>
public interface IVehicleRepository
{
    /// <summary>
    /// Retrieves all vehicles from the database.
    /// </summary>
    Task<IEnumerable<Vehicle>> GetAllVehiclesAsync();

    /// <summary>
    /// Retrieves a vehicle by its unique identifier.
    /// </summary>
    Task<Vehicle> GetByIdAsync(Guid id);

    /// <summary>
    /// Retrieves all vehicles manufactured by a specific manufacturer.
    /// </summary>
    Task<IEnumerable<Vehicle>> GetByManufacturerAsync(string manufacturer);

    /// <summary>
    /// Retrieves all vehicles of a specific model.
    /// </summary>
    Task<IEnumerable<Vehicle>> GetByModelAsync(string model);

    /// <summary>
    /// Retrieves all vehicles manufactured in a specific year.
    /// </summary>
    Task<IEnumerable<Vehicle>> GetByManufacturedYearAsync(int year);

    /// <summary>
    /// Retrieves all vehicles that belong to a specific multiple unit.
    /// </summary>
    Task<IEnumerable<Vehicle>> GetByMultipleUnitAsync(Guid multipleUnitId);

    /// <summary>
    /// Retrieves all standalone vehicles (not part of any multiple unit).
    /// </summary>
    Task<IEnumerable<Vehicle>> GetStandaloneVehiclesAsync();

    /// <summary>
    /// Retrieves all currently active (not decommissioned) vehicles.
    /// </summary>
    Task<IEnumerable<Vehicle>> GetActiveVehiclesAsync();

    /// <summary>
    /// Adds a new vehicle to the repository.
    /// </summary>
    Task AddAsync(Vehicle vehicle);

    /// <summary>
    /// Updates an existing vehicle in the repository.
    /// </summary>
    Task UpdateAsync(Vehicle vehicle);

    /// <summary>
    /// Removes a vehicle from the repository by its identifier.
    /// </summary>
    Task DeleteAsync(Guid id);

    /// <summary>
    /// Retrieves all traction systems for a specific vehicle.
    /// </summary>
    Task<IEnumerable<TractionSystem>> GetVehicleTractionSystemsAsync(Guid vehicleId);

    /// <summary>
    /// Retrieves all traction systems of a specific type (Electric, Diesel, Steam).
    /// Uses type-safe generic querying via OfType&lt;T&gt;().
    /// </summary>
    Task<IEnumerable<T>> GetTractionSystemsByTypeAsync<T>() where T : TractionSystem;
}
