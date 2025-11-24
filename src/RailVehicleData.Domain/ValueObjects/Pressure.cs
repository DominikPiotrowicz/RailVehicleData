namespace RailVehicleData.Domain.ValueObjects;

/// <summary>
/// Value Object representing pressure (mainly for steam boilers).
/// Internally stored in bars.
/// </summary>
public sealed class Pressure : ValueObject
{
    public decimal Bars { get; }

    public Pressure(decimal bars)
    {
        if (bars < 0)
            throw new DomainException("Pressure cannot be negative.");

        Bars = bars;
    }

    public static Pressure FromBars(decimal bars) => new(bars);
    public static Pressure FromAtmospheres(decimal atm) => new(atm * 1.01325m);
    public static Pressure FromPascals(decimal pa) => new(pa / 100_000m);
    public static Pressure FromPSI(decimal psi) => new(psi * 0.0689476m);

    public decimal ToBars => Bars;
    public decimal ToAtmospheres => Bars / 1.01325m;
    public decimal ToPascals => Bars * 100_000m;
    public decimal ToPSI => Bars * 14.5038m;

    protected override IEnumerable<object?> GetAtomicValues()
    {
        yield return Bars;
    }

    public override string ToString() => $"{Bars:F2} bar";
}
