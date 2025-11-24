namespace RailVehicleData.Domain.ValueObjects;

/// <summary>
/// Value Object containing specifications common to all vehicles.
/// Immutable and owned by Vehicle entity.
/// </summary>
public sealed class CommonSpecifications : ValueObject
{
    public Length Length { get; }
    public Length Width { get; }
    public Length Height { get; }
    public Length WheelDiameter { get; }
    public int AxleCount { get; }
    public Weight ServiceWeight { get; }
    public Speed MaxSpeed { get; }

    // Optional specifications for certain vehicle types
    public int? NumberOfSeats { get; }
    public int? NumberOfDoors { get; }
    public Volume? ToiletCapacity { get; }
    public bool HasClimateControl { get; }

    private CommonSpecifications(
        Length length,
        Length width,
        Length height,
        Length wheelDiameter,
        int axleCount,
        Weight serviceWeight,
        Speed maxSpeed,
        int? numberOfSeats = null,
        int? numberOfDoors = null,
        Volume? toiletCapacity = null,
        bool hasClimateControl = false)
    {
        Length = length;
        Width = width;
        Height = height;
        WheelDiameter = wheelDiameter;
        AxleCount = axleCount;
        ServiceWeight = serviceWeight;
        MaxSpeed = maxSpeed;
        NumberOfSeats = numberOfSeats;
        NumberOfDoors = numberOfDoors;
        ToiletCapacity = toiletCapacity;
        HasClimateControl = hasClimateControl;
    }

    public static CommonSpecifications Create(
        Length length,
        Length width,
        Length height,
        Length wheelDiameter,
        int axleCount,
        Weight serviceWeight,
        Speed maxSpeed,
        int? numberOfSeats = null,
        int? numberOfDoors = null,
        Volume? toiletCapacity = null,
        bool hasClimateControl = false)
    {
        if (axleCount <= 0)
            throw new DomainException("Axle count must be greater than 0.");

        return new CommonSpecifications(
            length,
            width,
            height,
            wheelDiameter,
            axleCount,
            serviceWeight,
            maxSpeed,
            numberOfSeats,
            numberOfDoors,
            toiletCapacity,
            hasClimateControl);
    }

    protected override IEnumerable<object?> GetAtomicValues()
    {
        yield return Length;
        yield return Width;
        yield return Height;
        yield return WheelDiameter;
        yield return AxleCount;
        yield return ServiceWeight;
        yield return MaxSpeed;
        yield return NumberOfSeats;
        yield return NumberOfDoors;
        yield return ToiletCapacity;
        yield return HasClimateControl;
    }
}
