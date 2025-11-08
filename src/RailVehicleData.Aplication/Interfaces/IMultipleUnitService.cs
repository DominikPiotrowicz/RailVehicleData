using RailVehicleData.Aplication.Dto;

namespace RailVehicleData.Aplication.Interfaces;

/// <summary>
/// Service interface for MultipleUnit (EMU/DMU) aggregate operations.
/// Defines business logic contracts for multiple unit management.
/// </summary>
public interface IMultipleUnitService
{
    /// <summary>
    /// Retrieves all multiple units.
    /// </summary>
    Task<IEnumerable<MultipleUnitDto>> GetAllMultipleUnitsAsync();

    /// <summary>
    /// Retrieves a multiple unit by ID.
    /// </summary>
    Task<MultipleUnitDto> GetMultipleUnitByIdAsync(Guid multipleUnitId);

    /// <summary>
    /// Retrieves a multiple unit by designation.
    /// </summary>
    Task<MultipleUnitDto> GetMultipleUnitByDesignationAsync(string designation);

    /// <summary>
    /// Retrieves all multiple units of a specific type (Electric, Diesel, Hybrid).
    /// </summary>
    Task<IEnumerable<MultipleUnitDto>> GetMultipleUnitsByTypeAsync(string type);

    /// <summary>
    /// Retrieves all active multiple units.
    /// </summary>
    Task<IEnumerable<MultipleUnitDto>> GetActiveMultipleUnitsAsync();

    /// <summary>
    /// Retrieves all decommissioned multiple units.
    /// </summary>
    Task<IEnumerable<MultipleUnitDto>> GetDecommissionedMultipleUnitsAsync();

    /// <summary>
    /// Retrieves all incomplete multiple units (missing cars).
    /// </summary>
    Task<IEnumerable<MultipleUnitDto>> GetIncompleteMultipleUnitsAsync();

    /// <summary>
    /// Creates a new multiple unit.
    /// </summary>
    Task<MultipleUnitDto> CreateMultipleUnitAsync(NewMultipleUnitDto multipleUnitDto);

    /// <summary>
    /// Updates an existing multiple unit.
    /// </summary>
    Task<MultipleUnitDto> UpdateMultipleUnitAsync(Guid multipleUnitId, NewMultipleUnitDto multipleUnitDto);

    /// <summary>
    /// Decommissions a multiple unit.
    /// </summary>
    Task<MultipleUnitDto> DecommissionMultipleUnitAsync(Guid multipleUnitId, DateTime decommissionDate);

    /// <summary>
    /// Adds a car to a multiple unit.
    /// </summary>
    Task<MultipleUnitDto> AddCarToMultipleUnitAsync(Guid multipleUnitId, Guid vehicleId);

    /// <summary>
    /// Removes a car from a multiple unit.
    /// </summary>
    Task<MultipleUnitDto> RemoveCarFromMultipleUnitAsync(Guid multipleUnitId, Guid vehicleId);

    /// <summary>
    /// Gets all cars in a multiple unit.
    /// </summary>
    Task<IEnumerable<VehicleDto>> GetCarsInMultipleUnitAsync(Guid multipleUnitId);

    /// <summary>
    /// Deletes a multiple unit.
    /// </summary>
    Task DeleteMultipleUnitAsync(Guid multipleUnitId);
}
