using RailVehicleData.Domain.ValueObjects;

namespace RailVehicleData.Domain.Entities;

/// <summary>
/// Represents a railway vehicle (locomotive, EMU car, wagon, etc.).
/// This is an Aggregate Root using composition over inheritance.
/// Vehicles can have multiple traction systems (electric, diesel, steam) via composition.
/// </summary>
public class Vehicle
{
    public Guid VehicleId { get; private set; } = Guid.NewGuid();

    /// <summary>
    /// Manufacturer name.
    /// </summary>
    public string Manufacturer { get; private set; }

    /// <summary>
    /// Model identifier/designation.
    /// </summary>
    public string Model { get; private set; }

    /// <summary>
    /// Year of manufacture.
    /// </summary>
    public int ManufacturedYear { get; private set; }

    /// <summary>
    /// Optional reference to a series/class (e.g., "Class 395", "91 class").
    /// </summary>
    public Guid? SeriesId { get; private set; }

    /// <summary>
    /// Role of this vehicle in a MultipleUnit (StandAlone if not part of any).
    /// </summary>
    public VehicleRole Role { get; private set; } = VehicleRole.StandAlone;

    /// <summary>
    /// If this vehicle is part of a MultipleUnit, this FK references it.
    /// </summary>
    public Guid? MultipleUnitId { get; private set; }

    /// <summary>
    /// Common specifications shared across all vehicles (owned value object).
    /// </summary>
    public CommonSpecifications CommonSpecifications { get; private set; }

    /// <summary>
    /// Collection of traction systems (electric, diesel, steam, etc.).
    /// Empty collection means this is a passenger wagon (no propulsion).
    /// </summary>
    private readonly List<TractionSystem> _tractionSystems = new();

    public IReadOnlyCollection<TractionSystem> TractionSystems => _tractionSystems.AsReadOnly();

    /// <summary>
    /// Total installed power across all active traction systems.
    /// </summary>
    public Power? TotalActivePower
    {
        get
        {
            var activeSystems = _tractionSystems
                .Where(ts => ts.IsActive && ts.MaxPower != null)
                .ToList();

            if (!activeSystems.Any())
                return null;

            var totalKw = activeSystems.Sum(ts => ts.MaxPower!.Kilowatts);
            return new Power(totalKw);
        }
    }

    /// <summary>
    /// Date when this vehicle was first registered/commissioned.
    /// </summary>
    public DateTime CommissionedDate { get; private set; }

    /// <summary>
    /// Date when this vehicle was decommissioned (null if still in service).
    /// </summary>
    public DateTime? DecommissionedDate { get; private set; }

    /// <summary>
    /// Indicates if vehicle is currently active.
    /// </summary>
    public bool IsActive => DecommissionedDate == null;

    private Vehicle() { }

    /// <summary>
    /// Creates a new standalone vehicle (locomotive, electric railcar, etc.).
    /// </summary>
    public static Vehicle CreateStandalone(
        string manufacturer,
        string model,
        int manufacturedYear,
        CommonSpecifications commonSpecifications,
        DateTime commissionedDate,
        Guid? seriesId = null)
    {
        if (string.IsNullOrWhiteSpace(manufacturer))
            throw new DomainException("Manufacturer cannot be empty.");

        if (string.IsNullOrWhiteSpace(model))
            throw new DomainException("Model cannot be empty.");

        if (manufacturedYear <= 1800 || manufacturedYear > DateTime.Now.Year)
            throw new DomainException("Manufactured year must be valid.");

        if (commonSpecifications == null)
            throw new DomainException("Common specifications are required.");

        return new Vehicle
        {
            VehicleId = Guid.NewGuid(),
            Manufacturer = manufacturer,
            Model = model,
            ManufacturedYear = manufacturedYear,
            SeriesId = seriesId,
            CommonSpecifications = commonSpecifications,
            CommissionedDate = commissionedDate,
            Role = VehicleRole.StandAlone
        };
    }

    /// <summary>
    /// Creates a new wagon (passenger, freight, etc.) - typically has no traction system.
    /// </summary>
    public static Vehicle CreateWagon(
        string manufacturer,
        string model,
        int manufacturedYear,
        CommonSpecifications commonSpecifications,
        DateTime commissionedDate,
        Guid multipleUnitId)
    {
        var vehicle = CreateStandalone(
            manufacturer,
            model,
            manufacturedYear,
            commonSpecifications,
            commissionedDate);

        vehicle.Role = VehicleRole.TrailerInMultipleUnit;
        vehicle.MultipleUnitId = multipleUnitId;

        return vehicle;
    }

    /// <summary>
    /// Adds a traction system to this vehicle (e.g., during modernization).
    /// </summary>
    public void AddTractionSystem(TractionSystem tractionSystem)
    {
        if (tractionSystem == null)
            throw new DomainException("Traction system cannot be null.");

        if (tractionSystem.VehicleId != VehicleId)
            throw new DomainException("Traction system must belong to this vehicle.");

        if (Role == VehicleRole.TrailerInMultipleUnit && _tractionSystems.Any())
            throw new DomainException("Wagons in multiple units cannot have traction systems.");

        _tractionSystems.Add(tractionSystem);
    }

    /// <summary>
    /// Removes a traction system from this vehicle (decommission).
    /// </summary>
    public void RemoveTractionSystem(Guid tractionSystemId)
    {
        var system = _tractionSystems.FirstOrDefault(ts => ts.TractionSystemId == tractionSystemId);
        if (system == null)
            throw new DomainException("Traction system not found.");

        _tractionSystems.Remove(system);
    }

    /// <summary>
    /// Gets all currently active traction systems.
    /// </summary>
    public IReadOnlyCollection<TractionSystem> GetActiveTractionSystems()
    {
        return _tractionSystems.Where(ts => ts.IsActive).ToList().AsReadOnly();
    }

    /// <summary>
    /// Gets all traction systems of a specific type (e.g., ElectricTraction).
    /// </summary>
    public IReadOnlyCollection<T> GetTractionSystemsByType<T>() where T : TractionSystem
    {
        return _tractionSystems.OfType<T>().ToList().AsReadOnly();
    }

    /// <summary>
    /// Decommissions this vehicle.
    /// </summary>
    public void Decommission(DateTime decommissionDate)
    {
        if (decommissionDate < CommissionedDate)
            throw new DomainException("Decommission date cannot be before commissioned date.");

        DecommissionedDate = decommissionDate;
    }

    /// <summary>
    /// Validates business rules (constraints).
    /// </summary>
    public bool IsValid()
    {
        // Standalone vehicles must have at least one traction system
        if (Role == VehicleRole.StandAlone && !_tractionSystems.Any())
            return false;

        // Wagons in multiple units cannot have traction systems
        if (Role == VehicleRole.TrailerInMultipleUnit && _tractionSystems.Any())
            return false;

        return true;
    }
}

/// <summary>
/// Enumeration for vehicle role within a multiple unit.
/// </summary>
public enum VehicleRole
{
    /// <summary>
    /// Standalone vehicle (locomotive).
    /// </summary>
    StandAlone,

    /// <summary>
    /// Powered car (head or mid) in a multiple unit.
    /// </summary>
    PoweredCarInMultipleUnit,

    /// <summary>
    /// Unpowered trailer car in a multiple unit.
    /// </summary>
    TrailerInMultipleUnit
}
