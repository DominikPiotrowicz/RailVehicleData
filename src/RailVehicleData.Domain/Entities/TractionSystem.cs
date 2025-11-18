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

    /// <summary>
    /// Initializes a new traction system instance.
    /// </summary>
    /// <param name="vehicleId">The unique identifier of the vehicle this system belongs to.</param>
    /// <param name="installedDate">The date when the system was installed.</param>
    /// <param name="maxPower">The maximum continuous power output, if applicable (null for steam traction).</param>
    /// <remarks>This constructor is protected and meant to be called by derived classes (ElectricTraction, DieselTraction, SteamTraction).</remarks>
    protected TractionSystem(Guid vehicleId, DateTime installedDate, Power? maxPower = null)
    {
        VehicleId = vehicleId;
        InstalledDate = installedDate;
        MaxPower = maxPower;
    }

    /// <summary>
    /// Decommissions this traction system by marking the removal date.
    /// </summary>
    /// <param name="removalDate">The date when the traction system was removed from service.</param>
    /// <exception cref="DomainException">Thrown if the removal date is before the installation date.</exception>
    /// <remarks>This marks the end of the traction system's service life but preserves its historical record for auditing.</remarks>
    public void Decommission(DateTime removalDate)
    {
        if (removalDate < InstalledDate)
            throw new DomainException("Removal date cannot be before installation date.");

        RemovedDate = removalDate;
    }

    /// <summary>
    /// Recommissions this traction system, restoring it to active status (e.g., after repair or modernization).
    /// </summary>
    /// <param name="reinstallDate">The date when the traction system was restored to service.</param>
    /// <exception cref="DomainException">Thrown if the system is already active, or if the reinstall date is before the removal date.</exception>
    /// <remarks>
    /// This is used when a previously decommissioned traction system is refurbished and returned to service.
    /// The reinstall date becomes the new installation date for historical tracking.
    /// </remarks>
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
