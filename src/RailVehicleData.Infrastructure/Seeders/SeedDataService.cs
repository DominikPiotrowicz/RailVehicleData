using Microsoft.Extensions.Logging;
using RailVehicleData.Infrastructure.Data;

namespace RailVehicleData.Infrastructure.Seeders;

/// <summary>
/// Production-ready seeding service that wraps VehicleSeeder with logging and error handling.
/// Automatically seeds the database with sample rail vehicle data on application startup.
/// </summary>
public class SeedDataService
{
    private readonly RailVehicleDbContext _dbContext;
    private readonly ILogger<SeedDataService>? _logger;

    public SeedDataService(RailVehicleDbContext dbContext, ILogger<SeedDataService>? logger = null)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _logger = logger;
    }

    /// <summary>
    /// Seeds the database with sample data if it's empty.
    /// Safe to call multiple times - checks if data exists before seeding.
    /// </summary>
    /// <returns>True if seeding was performed, false if database already had data.</returns>
    public async Task<bool> SeedIfEmptyAsync()
    {
        try
        {
            // Check if database already has vehicles
            if (_dbContext.Vehicles.Any())
            {
                _logger?.LogInformation("Database already contains vehicle data. Skipping seed operation.");
                return false;
            }

            _logger?.LogInformation("Starting database seeding with sample rail vehicle data...");

            // Run the vehicle seeder
            var seeder = new VehicleSeeder(_dbContext);
            await seeder.SeedAsync();

            _logger?.LogInformation("Database seeding completed successfully. Added sample locomotives and multiple units.");
            return true;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "An error occurred while seeding the database. Seeding operation failed.");
            throw;
        }
    }

    /// <summary>
    /// Seeds the database unconditionally, replacing any existing data.
    /// WARNING: This will delete all existing vehicles and related data!
    /// Use only in development environments or with explicit user confirmation.
    /// </summary>
    public async Task SeedAsync()
    {
        try
        {
            _logger?.LogWarning("Starting database seeding (unconditional). Existing data will be replaced.");

            // Clear existing data
            _dbContext.Vehicles.RemoveRange(_dbContext.Vehicles);
            _dbContext.MultipleUnits.RemoveRange(_dbContext.MultipleUnits);
            await _dbContext.SaveChangesAsync();

            _logger?.LogInformation("Cleared existing vehicle and multiple unit data.");

            // Run the vehicle seeder
            var seeder = new VehicleSeeder(_dbContext);
            await seeder.SeedAsync();

            _logger?.LogInformation("Database seeding completed successfully.");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "An error occurred during unconditional database seeding.");
            throw;
        }
    }

    /// <summary>
    /// Gets a summary of current seed data in the database.
    /// </summary>
    public async Task<SeedDataSummary> GetSeedDataSummaryAsync()
    {
        try
        {
            var vehicleCount = _dbContext.Vehicles.Count();
            var multipleUnitCount = _dbContext.MultipleUnits.Count();
            var tractionSystemCount = _dbContext.Set<RailVehicleData.Domain.Entities.TractionSystem>().Count();

            var electricCount = _dbContext.Vehicles
                .Where(v => v.TractionSystems.Any(ts => ts.GetType().Name == "ElectricTraction"))
                .Count();

            var dieselCount = _dbContext.Vehicles
                .Where(v => v.TractionSystems.Any(ts => ts.GetType().Name == "DieselTraction"))
                .Count();

            var steamCount = _dbContext.Vehicles
                .Where(v => v.TractionSystems.Any(ts => ts.GetType().Name == "SteamTraction"))
                .Count();

            return new SeedDataSummary
            {
                TotalVehicles = vehicleCount,
                TotalMultipleUnits = multipleUnitCount,
                TotalTractionSystems = tractionSystemCount,
                ElectricLocomotives = electricCount,
                DieselLocomotives = dieselCount,
                SteamLocomotives = steamCount
            };
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "An error occurred while retrieving seed data summary.");
            throw;
        }
    }
}

/// <summary>
/// Summary of seeded data in the database.
/// </summary>
public class SeedDataSummary
{
    public int TotalVehicles { get; set; }
    public int TotalMultipleUnits { get; set; }
    public int TotalTractionSystems { get; set; }
    public int ElectricLocomotives { get; set; }
    public int DieselLocomotives { get; set; }
    public int SteamLocomotives { get; set; }

    public override string ToString()
    {
        return $"Vehicles: {TotalVehicles}, MultipleUnits: {TotalMultipleUnits}, TractionSystems: {TotalTractionSystems} " +
               $"(Electric: {ElectricLocomotives}, Diesel: {DieselLocomotives}, Steam: {SteamLocomotives})";
    }
}
