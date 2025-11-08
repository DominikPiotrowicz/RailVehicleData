namespace RailVehicleData.Domain.ValueObjects;

/// <summary>
/// Value Object representing a length measurement.
/// Internally stored in millimeters to avoid floating-point precision issues.
/// </summary>
public sealed class Length : ValueObject
{
    public decimal Millimeters { get; }

    public Length(decimal millimeters)
    {
        if (millimeters <= 0)
            throw new DomainException("Length must be greater than 0 mm.");

        Millimeters = millimeters;
    }

    public static Length FromMillimeters(decimal mm) => new(mm);
    public static Length FromCentimeters(decimal cm) => new(cm * 10);
    public static Length FromMeters(decimal m) => new(m * 1000);
    public static Length FromKilometers(decimal km) => new(km * 1_000_000);

    public decimal ToCentimeters => Millimeters / 10;
    public decimal ToMeters => Millimeters / 1000;
    public decimal ToKilometers => Millimeters / 1_000_000;

    protected override IEnumerable<object?> GetAtomicValues()
    {
        yield return Millimeters;
    }

    public override string ToString() => $"{ToMeters:F2} m";
}
