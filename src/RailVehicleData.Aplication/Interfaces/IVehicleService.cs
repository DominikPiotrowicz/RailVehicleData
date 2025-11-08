using RailVehicleData.Aplication.Dto;

namespace RailVehicleData.Aplication.Interfaces;

/// <summary>
/// Service interface for Vehicle aggregate operations.
/// Defines business logic contracts for vehicle management.
/// </summary>
public interface IVehicleService
{
    /// <summary>
    /// Retrieves all vehicles.
    /// </summary>
    Task<IEnumerable<VehicleDto>> GetAllVehiclesAsync();

    /// <summary>
    /// Retrieves a vehicle by ID.
    /// </summary>
    Task<VehicleDto> GetVehicleByIdAsync(Guid vehicleId);

    /// <summary>
    /// Retrieves vehicles by manufacturer.
    /// </summary>
    Task<IEnumerable<VehicleDto>> GetVehiclesByManufacturerAsync(string manufacturer);

    /// <summary>
    /// Retrieves vehicles by model.
    /// </summary>
    Task<IEnumerable<VehicleDto>> GetVehiclesByModelAsync(string model);

    /// <summary>
    /// Retrieves vehicles by manufactured year.
    /// </summary>
    Task<IEnumerable<VehicleDto>> GetVehiclesByYearAsync(int year);

    /// <summary>
    /// Retrieves standalone vehicles (not part of any multiple unit).
    /// </summary>
    Task<IEnumerable<VehicleDto>> GetStandaloneVehiclesAsync();

    /// <summary>
    /// Retrieves all active vehicles.
    /// </summary>
    Task<IEnumerable<VehicleDto>> GetActiveVehiclesAsync();

    /// <summary>
    /// Retrieves all vehicles in a specific multiple unit.
    /// </summary>
    Task<IEnumerable<VehicleDto>> GetVehiclesByMultipleUnitAsync(Guid multipleUnitId);

    /// <summary>
    /// Retrieves traction systems for a vehicle.
    /// </summary>
    Task<IEnumerable<TractionSystemDto>> GetVehicleTractionSystemsAsync(Guid vehicleId);

    /// <summary>
    /// Creates a new standalone vehicle with traction systems.
    /// </summary>
    Task<VehicleDto> CreateStandaloneVehicleAsync(NewVehicleDto vehicleDto);

    /// <summary>
    /// Creates a new wagon (typically without traction system).
    /// </summary>
    Task<VehicleDto> CreateWagonAsync(NewVehicleDto wagonDto, Guid multipleUnitId);

    /// <summary>
    /// Updates an existing vehicle.
    /// </summary>
    Task<VehicleDto> UpdateVehicleAsync(Guid vehicleId, NewVehicleDto vehicleDto);

    /// <summary>
    /// Decommissions a vehicle.
    /// </summary>
    Task<VehicleDto> DecommissionVehicleAsync(Guid vehicleId, DateTime decommissionDate);

    /// <summary>
    /// Adds a traction system to a vehicle.
    /// </summary>
    Task<VehicleDto> AddTractionSystemAsync(Guid vehicleId, NewTractionSystemDto tractionSystemDto);

    /// <summary>
    /// Removes a traction system from a vehicle.
    /// </summary>
    Task<VehicleDto> RemoveTractionSystemAsync(Guid vehicleId, Guid tractionSystemId);

    /// <summary>
    /// Decommissions a traction system (soft delete via RemovedDate).
    /// </summary>
    Task<VehicleDto> DecommissionTractionSystemAsync(Guid vehicleId, Guid tractionSystemId, DateTime removalDate);

    /// <summary>
    /// Deletes a vehicle.
    /// </summary>
    Task DeleteVehicleAsync(Guid vehicleId);
}
