using RailVehicleData.Domain.Exceptions;
using RailVehicleData.Domain.ValueObjects;

namespace RailVehicleData.Domain.Entities;

/// <summary>
/// Represents a diesel traction system (diesel locomotive/DMU).
/// </summary>
public sealed class DieselTraction : TractionSystem
{
    /// <summary>
    /// Fuel tank capacity.
    /// </summary>
    public Volume FuelCapacity { get; }

    /// <summary>
    /// Engine model/type description.
    /// </summary>
    public string EngineType { get; }

    /// <summary>
    /// Emission standard (Euro 0, Euro I, Euro II, Euro III, Euro IV, Euro V, Stage 3A, etc.).
    /// </summary>
    public string EmissionStandard { get; }

    /// <summary>
    /// Number of cylinders in the engine.
    /// </summary>
    public int CylinderCount { get; }

    /// <summary>
    /// Estimated fuel consumption in liters per 100 km.
    /// </summary>
    public decimal? FuelConsumptionPer100Km { get; }

    public DieselTraction(
        Guid vehicleId,
        DateTime installedDate,
        Power maxPower,
        Volume fuelCapacity,
        string engineType,
        string emissionStandard,
        int cylinderCount,
        decimal? fuelConsumptionPer100Km = null) : base(vehicleId, installedDate, maxPower)
    {
        if (fuelCapacity == null)
            throw new DomainException("Fuel capacity is required.");

        if (string.IsNullOrWhiteSpace(engineType))
            throw new DomainException("Engine type cannot be empty.");

        if (string.IsNullOrWhiteSpace(emissionStandard))
            throw new DomainException("Emission standard cannot be empty.");

        if (cylinderCount <= 0)
            throw new DomainException("Cylinder count must be greater than 0.");

        FuelCapacity = fuelCapacity;
        EngineType = engineType;
        EmissionStandard = emissionStandard;
        CylinderCount = cylinderCount;
        FuelConsumptionPer100Km = fuelConsumptionPer100Km;
    }
}
