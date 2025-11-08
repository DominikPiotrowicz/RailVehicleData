namespace RailVehicleData.Domain.Entities;

/// <summary>
/// Represents a steam traction system (steam locomotive).
/// </summary>
public sealed class SteamTraction : TractionSystem
{
    /// <summary>
    /// Working pressure of the boiler.
    /// </summary>
    public Pressure BoilerWorkingPressure { get; }

    /// <summary>
    /// Maximum pressure the boiler can withstand.
    /// </summary>
    public Pressure BoilerMaxPressure { get; }

    /// <summary>
    /// Water capacity of the boiler.
    /// </summary>
    public Volume BoilerCapacity { get; }

    /// <summary>
    /// Grate area (where fuel burns).
    /// </summary>
    public Area GrateArea { get; }

    /// <summary>
    /// Type of firebox (e.g., "Wootten", "Scottish", "Belpaire").
    /// </summary>
    public string FireboxType { get; }

    /// <summary>
    /// Total heating surface area of the boiler.
    /// </summary>
    public Area? HeatingArea { get; }

    /// <summary>
    /// Fuel type (coal, oil, wood).
    /// </summary>
    public string FuelType { get; }

    public SteamTraction(
        Guid vehicleId,
        DateTime installedDate,
        Pressure boilerWorkingPressure,
        Pressure boilerMaxPressure,
        Volume boilerCapacity,
        Area grateArea,
        string fireboxType,
        string fuelType,
        Area? heatingArea = null) : base(vehicleId, installedDate, maxPower: null)
    {
        if (boilerWorkingPressure == null)
            throw new DomainException("Boiler working pressure is required.");

        if (boilerMaxPressure == null)
            throw new DomainException("Boiler max pressure is required.");

        if (boilerWorkingPressure.Bars > boilerMaxPressure.Bars)
            throw new DomainException("Working pressure cannot exceed max pressure.");

        if (boilerCapacity == null)
            throw new DomainException("Boiler capacity is required.");

        if (grateArea == null)
            throw new DomainException("Grate area is required.");

        if (string.IsNullOrWhiteSpace(fireboxType))
            throw new DomainException("Firebox type cannot be empty.");

        if (string.IsNullOrWhiteSpace(fuelType))
            throw new DomainException("Fuel type cannot be empty.");

        BoilerWorkingPressure = boilerWorkingPressure;
        BoilerMaxPressure = boilerMaxPressure;
        BoilerCapacity = boilerCapacity;
        GrateArea = grateArea;
        FireboxType = fireboxType;
        FuelType = fuelType;
        HeatingArea = heatingArea;
    }
}
