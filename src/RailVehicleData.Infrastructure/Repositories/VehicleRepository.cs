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
    /// <returns>An enumerable collection of all vehicles in the database.</returns>
    public async Task<IEnumerable<Vehicle>> GetAllVehiclesAsync()
    {
        return await _dbContext.Vehicles
            .AsNoTracking()
            .ToListAsync();
    }

    /// <summary>
    /// Retrieves a vehicle by ID with all related traction systems.
    /// </summary>
    /// <param name="id">The unique identifier of the vehicle to retrieve.</param>
    /// <returns>The Vehicle entity if found.</returns>
    /// <exception cref="DomainException">Thrown if no vehicle with the specified ID exists.</exception>
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
    /// <param name="manufacturer">The name of the manufacturer (e.g., "Bombardier", "Siemens").</param>
    /// <returns>An enumerable collection of vehicles from the specified manufacturer.</returns>
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
    /// <param name="model">The model designation (e.g., "EU07", "SP32").</param>
    /// <returns>An enumerable collection of vehicles with the specified model.</returns>
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
    /// <param name="year">The year of manufacture.</param>
    /// <returns>An enumerable collection of vehicles manufactured in the specified year.</returns>
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
    /// <param name="multipleUnitId">The unique identifier of the multiple unit (train set).</param>
    /// <returns>An enumerable collection of vehicles belonging to the specified multiple unit.</returns>
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
    /// <returns>An enumerable collection of all standalone vehicles (locomotives, independent cars, etc.).</returns>
    public async Task<IEnumerable<Vehicle>> GetStandaloneVehiclesAsync()
    {
        return await _dbContext.Vehicles
            .AsNoTracking()
            .Where(v => v.Role == VehicleRole.StandAlone)
            .ToListAsync();
    }

    /// <summary>
    /// Retrieves all currently active vehicles (not decommissioned).
    /// </summary>
    /// <returns>An enumerable collection of all active vehicles in service.</returns>
    public async Task<IEnumerable<Vehicle>> GetActiveVehiclesAsync()
    {
        return await _dbContext.Vehicles
            .AsNoTracking()
            .Where(v => v.IsActive)
            .ToListAsync();
    }

    /// <summary>
    /// Adds a new vehicle to the database (without committing).
    /// SaveChangesAsync must be called by the calling service to commit atomically.
    /// </summary>
    /// <param name="vehicle">The Vehicle entity to add.</param>
    /// <exception cref="DomainException">Thrown if vehicle is null.</exception>
    public async Task AddAsync(Vehicle vehicle)
    {
        if (vehicle == null)
            throw new DomainException("Vehicle cannot be null.");

        await _dbContext.Vehicles.AddAsync(vehicle);
    }

    /// <summary>
    /// Updates an existing vehicle (without committing).
    /// SaveChangesAsync must be called by the calling service to commit atomically.
    /// </summary>
    /// <param name="vehicle">The Vehicle entity with updated values.</param>
    /// <exception cref="DomainException">Thrown if vehicle is null.</exception>
    public async Task UpdateAsync(Vehicle vehicle)
    {
        if (vehicle == null)
            throw new DomainException("Vehicle cannot be null.");

        _dbContext.Vehicles.Update(vehicle);
    }

    /// <summary>
    /// Deletes a vehicle by ID (without committing).
    /// SaveChangesAsync must be called by the calling service to commit atomically.
    /// </summary>
    /// <param name="id">The unique identifier of the vehicle to delete.</param>
    /// <exception cref="DomainException">Thrown if no vehicle with the specified ID exists.</exception>
    public async Task DeleteAsync(Guid id)
    {
        var vehicle = await GetByIdAsync(id);
        _dbContext.Vehicles.Remove(vehicle);
    }

    /// <summary>
    /// Commits all staged changes to the database atomically.
    /// Must be called after Add/Update/Delete operations to persist changes.
    /// </summary>
    /// <remarks>Implements the Unit of Work pattern for transactional consistency.</remarks>
    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Gets all traction systems for a specific vehicle.
    /// </summary>
    /// <param name="vehicleId">The unique identifier of the vehicle.</param>
    /// <returns>An enumerable collection of all traction systems installed on the vehicle.</returns>
    public async Task<IEnumerable<TractionSystem>> GetVehicleTractionSystemsAsync(Guid vehicleId)
    {
        return await _dbContext.TractionSystems
            .AsNoTracking()
            .Where(ts => ts.VehicleId == vehicleId)
            .ToListAsync();
    }

    /// <summary>
    /// Gets all traction systems of a specific type (e.g., ElectricTraction, DieselTraction).
    /// </summary>
    /// <typeparam name="T">The traction system type to retrieve.</typeparam>
    /// <returns>An enumerable collection of traction systems of the specified type across all vehicles.</returns>
    public async Task<IEnumerable<T>> GetTractionSystemsByTypeAsync<T>() where T : TractionSystem
    {
        return await _dbContext.TractionSystems
            .AsNoTracking()
            .OfType<T>()
            .ToListAsync();
    }
}
