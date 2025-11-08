namespace RailVehicleData.Domain.ValueObjects;

/// <summary>
/// Value Object representing area (grate area, heating surface, etc.).
/// Internally stored in square meters.
/// </summary>
public sealed class Area : ValueObject
{
    public decimal SquareMeters { get; }

    public Area(decimal squareMeters)
    {
        if (squareMeters <= 0)
            throw new DomainException("Area must be greater than 0 m².");

        SquareMeters = squareMeters;
    }

    public static Area FromSquareMeters(decimal m2) => new(m2);
    public static Area FromSquareCentimeters(decimal cm2) => new(cm2 / 10_000);
    public static Area FromSquareKilometers(decimal km2) => new(km2 * 1_000_000);

    public decimal ToSquareMeters => SquareMeters;
    public decimal ToSquareCentimeters => SquareMeters * 10_000;
    public decimal ToSquareKilometers => SquareMeters / 1_000_000;

    protected override IEnumerable<object?> GetAtomicValues()
    {
        yield return SquareMeters;
    }

    public override string ToString() => $"{SquareMeters:F2} m²";
}
