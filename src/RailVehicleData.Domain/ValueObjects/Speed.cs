namespace RailVehicleData.Domain.ValueObjects;

/// <summary>
/// Value Object representing speed/velocity.
/// Internally stored in kilometers per hour (km/h).
/// </summary>
public sealed class Speed : ValueObject
{
    public decimal KilometersPerHour { get; }

    public Speed(decimal kmh)
    {
        if (kmh <= 0)
            throw new DomainException("Speed must be greater than 0 km/h.");

        KilometersPerHour = kmh;
    }

    public static Speed FromKilometersPerHour(decimal kmh) => new(kmh);
    public static Speed FromMetersPerSecond(decimal mps) => new(mps * 3.6m);
    public static Speed FromMilesPerHour(decimal mph) => new(mph * 1.60934m);

    public decimal ToKilometersPerHour => KilometersPerHour;
    public decimal ToMetersPerSecond => KilometersPerHour / 3.6m;
    public decimal ToMilesPerHour => KilometersPerHour / 1.60934m;

    protected override IEnumerable<object?> GetAtomicValues()
    {
        yield return KilometersPerHour;
    }

    public override string ToString() => $"{KilometersPerHour:F0} km/h";
}
