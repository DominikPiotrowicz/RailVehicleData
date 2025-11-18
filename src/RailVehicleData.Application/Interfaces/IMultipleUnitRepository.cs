using RailVehicleData.Domain.Entities;

namespace RailVehicleData.Application.Interfaces;

/// <summary>
/// Repository abstraction for MultipleUnit (EMU/DMU) aggregate operations.
/// Provides type-safe data access methods for train sets and their composition management.
/// </summary>
public interface IMultipleUnitRepository
{
    /// <summary>
    /// Retrieves all multiple units from the database.
    /// </summary>
    Task<IEnumerable<MultipleUnit>> GetAllAsync();

    /// <summary>
    /// Retrieves a multiple unit by its unique identifier.
    /// </summary>
    Task<MultipleUnit> GetByIdAsync(Guid id);

    /// <summary>
    /// Retrieves a multiple unit by its designation (e.g., "Class 395", "Class 153").
    /// </summary>
    Task<MultipleUnit> GetByDesignationAsync(string designation);

    /// <summary>
    /// Retrieves all multiple units of a specific type (Electric, Diesel, Hybrid).
    /// </summary>
    Task<IEnumerable<MultipleUnit>> GetByTypeAsync(MultipleUnitType type);

    /// <summary>
    /// Retrieves all currently active (not decommissioned) multiple units.
    /// </summary>
    Task<IEnumerable<MultipleUnit>> GetActiveAsync();

    /// <summary>
    /// Retrieves all decommissioned multiple units.
    /// </summary>
    Task<IEnumerable<MultipleUnit>> GetDecommissionedAsync();

    /// <summary>
    /// Retrieves all incomplete multiple units (car count &lt; required count).
    /// Useful for identifying train sets that need additional cars.
    /// </summary>
    Task<IEnumerable<MultipleUnit>> GetIncompleteAsync();

    /// <summary>
    /// Adds a new multiple unit to the repository (staging change, not committed).
    /// Must call SaveChangesAsync() separately to persist changes atomically.
    /// </summary>
    Task AddAsync(MultipleUnit multipleUnit);

    /// <summary>
    /// Updates an existing multiple unit in the repository (staging change, not committed).
    /// Must call SaveChangesAsync() separately to persist changes atomically.
    /// </summary>
    Task UpdateAsync(MultipleUnit multipleUnit);

    /// <summary>
    /// Removes a multiple unit from the repository (staging change, not committed).
    /// Must call SaveChangesAsync() separately to persist changes atomically.
    /// </summary>
    Task DeleteAsync(Guid id);

    /// <summary>
    /// Commits all staged changes to the database atomically.
    /// Should be called after Add/Update/Delete operations.
    /// </summary>
    Task SaveChangesAsync();
}
