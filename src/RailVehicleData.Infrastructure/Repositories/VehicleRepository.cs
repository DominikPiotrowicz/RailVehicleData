using Microsoft.EntityFrameworkCore;
using RailVehicleData.Aplication.Interfaces;
using RailVehicleData.Domain.Entities;
using RailVehicleData.Domain.Exceptions;
using RailVehicleData.Infrastructure.Data;

namespace RailVehicleData.Infrastructure.Repositories;

/// <summary>
/// Repository for Vehicle aggregate root.
/// Handles all data access operations for vehicles and their traction systems.
/// Implements IVehicleRepository from Application layer for proper dependency inversion.
/// </summary>
public class VehicleRepository : IVehicleRepository
{
    private readonly RailVehicleDbContext _dbContext;

    public VehicleRepository(RailVehicleDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Retrieves all vehicles including their traction systems.
    /// </summary>
    public async Task<IEnumerable<Vehicle>> GetAllVehiclesAsync()
    {
        return await _dbContext.Vehicles
            .AsNoTracking()
            .ToListAsync();
    }

    /// <summary>
    /// Retrieves a vehicle by ID with all related traction systems.
    /// </summary>
    public async Task<Vehicle> GetByIdAsync(Guid id)
    {
        var vehicle = await _dbContext.Vehicles
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.VehicleId == id);

        if (vehicle == null)
            throw new DomainException($"Vehicle with ID {id} not found.");

        return vehicle;
    }

    /// <summary>
    /// Retrieves vehicles by manufacturer.
    /// </summary>
    public async Task<IEnumerable<Vehicle>> GetByManufacturerAsync(string manufacturer)
    {
        return await _dbContext.Vehicles
            .AsNoTracking()
            .Where(v => v.Manufacturer == manufacturer)
            .ToListAsync();
    }

    /// <summary>
    /// Retrieves vehicles by model.
    /// </summary>
    public async Task<IEnumerable<Vehicle>> GetByModelAsync(string model)
    {
        return await _dbContext.Vehicles
            .AsNoTracking()
            .Where(v => v.Model == model)
            .ToListAsync();
    }

    /// <summary>
    /// Retrieves vehicles by manufactured year.
    /// </summary>
    public async Task<IEnumerable<Vehicle>> GetByManufacturedYearAsync(int year)
    {
        return await _dbContext.Vehicles
            .AsNoTracking()
            .Where(v => v.ManufacturedYear == year)
            .ToListAsync();
    }

    /// <summary>
    /// Retrieves all vehicles that are part of a specific multiple unit.
    /// </summary>
    public async Task<IEnumerable<Vehicle>> GetByMultipleUnitAsync(Guid multipleUnitId)
    {
        return await _dbContext.Vehicles
            .AsNoTracking()
            .Where(v => v.MultipleUnitId == multipleUnitId)
            .ToListAsync();
    }

    /// <summary>
    /// Retrieves all standalone vehicles (not part of any multiple unit).
    /// </summary>
    public async Task<IEnumerable<Vehicle>> GetStandaloneVehiclesAsync()
    {
        return await _dbContext.Vehicles
            .AsNoTracking()
            .Where(v => v.Role == VehicleRole.StandAlone)
            .ToListAsync();
    }

    /// <summary>
    /// Retrieves all currently active vehicles.
    /// </summary>
    public async Task<IEnumerable<Vehicle>> GetActiveVehiclesAsync()
    {
        return await _dbContext.Vehicles
            .AsNoTracking()
            .Where(v => v.IsActive)
            .ToListAsync();
    }

    /// <summary>
    /// Adds a new vehicle to the database.
    /// </summary>
    public async Task AddAsync(Vehicle vehicle)
    {
        if (vehicle == null)
            throw new DomainException("Vehicle cannot be null.");

        await _dbContext.Vehicles.AddAsync(vehicle);
        await _dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Updates an existing vehicle.
    /// </summary>
    public async Task UpdateAsync(Vehicle vehicle)
    {
        if (vehicle == null)
            throw new DomainException("Vehicle cannot be null.");

        _dbContext.Vehicles.Update(vehicle);
        await _dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Deletes a vehicle by ID.
    /// </summary>
    public async Task DeleteAsync(Guid id)
    {
        var vehicle = await GetByIdAsync(id);
        _dbContext.Vehicles.Remove(vehicle);
        await _dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Gets traction systems for a specific vehicle.
    /// </summary>
    public async Task<IEnumerable<TractionSystem>> GetVehicleTractionSystemsAsync(Guid vehicleId)
    {
        return await _dbContext.TractionSystems
            .AsNoTracking()
            .Where(ts => ts.VehicleId == vehicleId)
            .ToListAsync();
    }

    /// <summary>
    /// Gets all traction systems of a specific type (e.g., ElectricTraction).
    /// </summary>
    public async Task<IEnumerable<T>> GetTractionSystemsByTypeAsync<T>() where T : TractionSystem
    {
        return await _dbContext.TractionSystems
            .AsNoTracking()
            .OfType<T>()
            .ToListAsync();
    }
}
