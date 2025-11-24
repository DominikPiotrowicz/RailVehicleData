namespace RailVehicleData.Domain.ValueObjects;

/// <summary>
/// Value Object representing volume (fuel tanks, water tanks, etc.).
/// Internally stored in liters.
/// </summary>
public sealed class Volume : ValueObject
{
    public decimal Liters { get; }

    public Volume(decimal liters)
    {
        if (liters < 0)
            throw new DomainException("Volume cannot be negative.");

        Liters = liters;
    }

    public static Volume FromLiters(decimal liters) => new(liters);
    public static Volume FromCubicMeters(decimal m3) => new(m3 * 1000);
    public static Volume FromCubicCentimeters(decimal cc) => new(cc / 1000);
    public static Volume FromGallons(decimal gal) => new(gal * 3.78541m);

    public decimal ToLiters => Liters;
    public decimal ToCubicMeters => Liters / 1000m;
    public decimal ToCubicCentimeters => Liters * 1000m;
    public decimal ToGallons => Liters / 3.78541m;

    protected override IEnumerable<object?> GetAtomicValues()
    {
        yield return Liters;
    }

    public override string ToString() => $"{Liters:F0} L";
}
