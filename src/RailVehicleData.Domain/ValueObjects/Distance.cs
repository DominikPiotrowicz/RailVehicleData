namespace RailVehicleData.Domain.ValueObjects;

/// <summary>
/// Value Object representing distance (range, kilometers, etc.).
/// Internally stored in kilometers.
/// </summary>
public sealed class Distance : ValueObject
{
    public decimal Kilometers { get; }

    public Distance(decimal kilometers)
    {
        if (kilometers <= 0)
            throw new DomainException("Distance must be greater than 0 km.");

        Kilometers = kilometers;
    }

    public static Distance FromKilometers(decimal km) => new(km);
    public static Distance FromMeters(decimal m) => new(m / 1000);
    public static Distance FromMiles(decimal miles) => new(miles * 1.60934m);

    public decimal ToKilometers => Kilometers;
    public decimal ToMeters => Kilometers * 1000;
    public decimal ToMiles => Kilometers / 1.60934m;

    protected override IEnumerable<object?> GetAtomicValues()
    {
        yield return Kilometers;
    }

    public override string ToString() => $"{Kilometers:F0} km";
}
