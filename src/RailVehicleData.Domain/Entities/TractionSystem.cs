namespace RailVehicleData.Domain.Entities;

/// <summary>
/// Abstract base class for all traction systems (electric, diesel, steam).
/// Uses Table-Per-Hierarchy (TPH) inheritance with EF Core.
/// Allows a vehicle to have multiple traction systems (e.g., hybrid vehicles).
/// </summary>
public abstract class TractionSystem
{
    public Guid TractionSystemId { get; private set; } = Guid.NewGuid();

    /// <summary>
    /// Foreign key to the Vehicle this traction system belongs to.
    /// </summary>
    public Guid VehicleId { get; private set; }

    /// <summary>
    /// Date when this traction system was installed.
    /// </summary>
    public DateTime InstalledDate { get; private set; }

    /// <summary>
    /// Date when this traction system was removed (null if still active).
    /// </summary>
    public DateTime? RemovedDate { get; private set; }

    /// <summary>
    /// Maximum continuous power output of this traction system (null for steam).
    /// </summary>
    public Power? MaxPower { get; protected set; }

    /// <summary>
    /// Flag indicating if this system is currently active (not removed).
    /// </summary>
    public bool IsActive => RemovedDate == null;

    protected TractionSystem(Guid vehicleId, DateTime installedDate, Power? maxPower = null)
    {
        VehicleId = vehicleId;
        InstalledDate = installedDate;
        MaxPower = maxPower;
    }

    /// <summary>
    /// Decommissions this traction system by setting removal date.
    /// </summary>
    public void Decommission(DateTime removalDate)
    {
        if (removalDate < InstalledDate)
            throw new DomainException("Removal date cannot be before installation date.");

        RemovedDate = removalDate;
    }

    /// <summary>
    /// Recommissions this traction system (e.g., after repair).
    /// </summary>
    public void Recommission(DateTime reinstallDate)
    {
        if (RemovedDate == null)
            throw new DomainException("Cannot recommission an active traction system.");

        if (reinstallDate < RemovedDate)
            throw new DomainException("Reinstall date cannot be before removal date.");

        RemovedDate = null;
        InstalledDate = reinstallDate;
    }
}
