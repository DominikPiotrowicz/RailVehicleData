using Microsoft.EntityFrameworkCore;
using RailVehicleData.Aplication.Interfaces;
using RailVehicleData.Domain.Entities;
using RailVehicleData.Domain.Exceptions;
using RailVehicleData.Infrastrcture.Data;

namespace RailVehicleData.Infrastrcture.Repositories;

/// <summary>
/// Repository for MultipleUnit aggregate root.
/// Handles all data access operations for EMU/DMU (trains).
/// Implements IMultipleUnitRepository from Application layer for proper dependency inversion.
/// </summary>
public class MultipleUnitRepository : IMultipleUnitRepository
{
    private readonly RailVehicleDbContext _dbContext;

    public MultipleUnitRepository(RailVehicleDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Retrieves all multiple units.
    /// </summary>
    public async Task<IEnumerable<MultipleUnit>> GetAllAsync()
    {
        return await _dbContext.MultipleUnits
            .AsNoTracking()
            .ToListAsync();
    }

    /// <summary>
    /// Retrieves a multiple unit by ID.
    /// </summary>
    public async Task<MultipleUnit> GetByIdAsync(Guid id)
    {
        var multipleUnit = await _dbContext.MultipleUnits
            .AsNoTracking()
            .FirstOrDefaultAsync(mu => mu.MultipleUnitId == id);

        if (multipleUnit == null)
            throw new DomainException($"Multiple unit with ID {id} not found.");

        return multipleUnit;
    }

    /// <summary>
    /// Retrieves a multiple unit by designation.
    /// </summary>
    public async Task<MultipleUnit> GetByDesignationAsync(string designation)
    {
        var multipleUnit = await _dbContext.MultipleUnits
            .AsNoTracking()
            .FirstOrDefaultAsync(mu => mu.Designation == designation);

        if (multipleUnit == null)
            throw new DomainException($"Multiple unit with designation {designation} not found.");

        return multipleUnit;
    }

    /// <summary>
    /// Retrieves all multiple units of a specific type (Electric, Diesel, Hybrid).
    /// </summary>
    public async Task<IEnumerable<MultipleUnit>> GetByTypeAsync(MultipleUnitType type)
    {
        return await _dbContext.MultipleUnits
            .AsNoTracking()
            .Where(mu => mu.Type == type)
            .ToListAsync();
    }

    /// <summary>
    /// Retrieves all currently active multiple units.
    /// </summary>
    public async Task<IEnumerable<MultipleUnit>> GetActiveAsync()
    {
        return await _dbContext.MultipleUnits
            .AsNoTracking()
            .Where(mu => mu.IsActive)
            .ToListAsync();
    }

    /// <summary>
    /// Retrieves all decommissioned multiple units.
    /// </summary>
    public async Task<IEnumerable<MultipleUnit>> GetDecommissionedAsync()
    {
        return await _dbContext.MultipleUnits
            .AsNoTracking()
            .Where(mu => !mu.IsActive)
            .ToListAsync();
    }

    /// <summary>
    /// Retrieves all incomplete multiple units (missing some cars).
    /// </summary>
    public async Task<IEnumerable<MultipleUnit>> GetIncompleteAsync()
    {
        return await _dbContext.MultipleUnits
            .AsNoTracking()
            .Where(mu => !mu.IsComplete)
            .ToListAsync();
    }

    /// <summary>
    /// Adds a new multiple unit to the database.
    /// </summary>
    public async Task AddAsync(MultipleUnit multipleUnit)
    {
        if (multipleUnit == null)
            throw new DomainException("Multiple unit cannot be null.");

        await _dbContext.MultipleUnits.AddAsync(multipleUnit);
        await _dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Updates an existing multiple unit.
    /// </summary>
    public async Task UpdateAsync(MultipleUnit multipleUnit)
    {
        if (multipleUnit == null)
            throw new DomainException("Multiple unit cannot be null.");

        _dbContext.MultipleUnits.Update(multipleUnit);
        await _dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Deletes a multiple unit by ID.
    /// </summary>
    public async Task DeleteAsync(Guid id)
    {
        var multipleUnit = await GetByIdAsync(id);
        _dbContext.MultipleUnits.Remove(multipleUnit);
        await _dbContext.SaveChangesAsync();
    }
}
