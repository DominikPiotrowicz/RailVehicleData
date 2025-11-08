namespace RailVehicleData.Domain.Entities;

/// <summary>
/// Represents a Multiple Unit (EMU - Electric Multiple Unit, or DMU - Diesel Multiple Unit).
/// This is an Aggregate Root that owns a collection of Vehicle entities (cars).
/// </summary>
public class MultipleUnit
{
    /// <summary>
    /// Unique identifier for this multiple unit.
    /// </summary>
    public Guid MultipleUnitId { get; private set; } = Guid.NewGuid();

    /// <summary>
    /// Designation/classification (e.g., "Class 395", "EMU 325", "Class 153").
    /// </summary>
    public string Designation { get; private set; }

    /// <summary>
    /// Number of cars in this multiple unit.
    /// </summary>
    public int CarCount { get; private set; }

    /// <summary>
    /// First car number (e.g., 395001).
    /// </summary>
    public int FirstCarNumber { get; private set; }

    /// <summary>
    /// Last car number (e.g., 395040).
    /// </summary>
    public int LastCarNumber { get; private set; }

    /// <summary>
    /// Total passenger capacity across all cars.
    /// </summary>
    public int TotalCapacity { get; private set; }

    /// <summary>
    /// Date when this multiple unit was commissioned.
    /// </summary>
    public DateTime CommissionedDate { get; private set; }

    /// <summary>
    /// Date when this multiple unit was decommissioned (null if still in service).
    /// </summary>
    public DateTime? DecommissionedDate { get; private set; }

    /// <summary>
    /// Indicates if this multiple unit is currently active.
    /// </summary>
    public bool IsActive => DecommissionedDate == null;

    /// <summary>
    /// Total length of the entire multiple unit.
    /// </summary>
    public Length TotalLength { get; private set; }

    /// <summary>
    /// Maximum speed for the entire multiple unit.
    /// </summary>
    public Speed MaxSpeed { get; private set; }

    /// <summary>
    /// Type of multiple unit (Electric or Diesel).
    /// </summary>
    public MultipleUnitType Type { get; private set; }

    /// <summary>
    /// Collection of cars (vehicles) in this multiple unit.
    /// </summary>
    private readonly List<Guid> _carIds = new();
    public IReadOnlyCollection<Guid> CarIds => _carIds.AsReadOnly();

    private MultipleUnit() { }

    /// <summary>
    /// Factory method to create a new Multiple Unit.
    /// </summary>
    public static MultipleUnit Create(
        string designation,
        int carCount,
        int firstCarNumber,
        int lastCarNumber,
        int totalCapacity,
        Length totalLength,
        Speed maxSpeed,
        MultipleUnitType type,
        DateTime commissionedDate)
    {
        if (string.IsNullOrWhiteSpace(designation))
            throw new DomainException("Designation cannot be empty.");

        if (carCount < 2)
            throw new DomainException("Multiple unit must have at least 2 cars.");

        if (firstCarNumber >= lastCarNumber)
            throw new DomainException("Last car number must be greater than first car number.");

        if (totalCapacity < 0)
            throw new DomainException("Total capacity cannot be negative.");

        if (totalLength == null)
            throw new DomainException("Total length is required.");

        if (maxSpeed == null)
            throw new DomainException("Maximum speed is required.");

        return new MultipleUnit
        {
            MultipleUnitId = Guid.NewGuid(),
            Designation = designation,
            CarCount = carCount,
            FirstCarNumber = firstCarNumber,
            LastCarNumber = lastCarNumber,
            TotalCapacity = totalCapacity,
            CommissionedDate = commissionedDate,
            TotalLength = totalLength,
            MaxSpeed = maxSpeed,
            Type = type
        };
    }

    /// <summary>
    /// Adds a car to this multiple unit.
    /// </summary>
    public void AddCar(Guid carId)
    {
        if (carId == Guid.Empty)
            throw new DomainException("Car ID cannot be empty.");

        if (_carIds.Contains(carId))
            throw new DomainException("Car is already part of this multiple unit.");

        if (_carIds.Count >= CarCount)
            throw new DomainException($"Multiple unit can only have {CarCount} cars.");

        _carIds.Add(carId);
    }

    /// <summary>
    /// Removes a car from this multiple unit.
    /// </summary>
    public void RemoveCar(Guid carId)
    {
        if (!_carIds.Contains(carId))
            throw new DomainException("Car is not part of this multiple unit.");

        _carIds.Remove(carId);
    }

    /// <summary>
    /// Gets total active power across all powered cars in this multiple unit.
    /// This should be implemented by the Application layer using repository data.
    /// </summary>
    public bool IsComplete => _carIds.Count == CarCount;

    /// <summary>
    /// Decommissions this multiple unit.
    /// </summary>
    public void Decommission(DateTime decommissionDate)
    {
        if (decommissionDate < CommissionedDate)
            throw new DomainException("Decommission date cannot be before commissioned date.");

        DecommissionedDate = decommissionDate;
    }

    /// <summary>
    /// Validates business rules.
    /// </summary>
    public bool IsValid()
    {
        // Must have at least 2 cars
        if (CarCount < 2)
            return false;

        // All cars must be accounted for
        if (_carIds.Count != CarCount)
            return false;

        // Must have at least one powered car
        // This check needs to be done in Application layer with actual vehicle data

        return true;
    }
}

/// <summary>
/// Enumeration for multiple unit type.
/// </summary>
public enum MultipleUnitType
{
    /// <summary>
    /// Electric Multiple Unit (powered by overhead lines).
    /// </summary>
    Electric,

    /// <summary>
    /// Diesel Multiple Unit (self-powered by diesel engines).
    /// </summary>
    Diesel,

    /// <summary>
    /// Hybrid Multiple Unit (combination of electric and diesel).
    /// </summary>
    Hybrid
}
