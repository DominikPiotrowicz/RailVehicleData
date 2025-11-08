namespace RailVehicleData.Domain.ValueObjects;

/// <summary>
/// Value Object representing electrical or mechanical power.
/// Internally stored in kilowatts.
/// </summary>
public sealed class Power : ValueObject
{
    public int Kilowatts { get; }

    public Power(int kilowatts)
    {
        if (kilowatts <= 0)
            throw new DomainException("Power must be greater than 0 kW.");

        Kilowatts = kilowatts;
    }

    public static Power FromKilowatts(int kw) => new(kw);
    public static Power FromMegawatts(decimal mw) => new((int)(mw * 1000));
    public static Power FromHorsepower(decimal hp) => new((int)(hp * 0.7457m));

    public int ToKilowatts => Kilowatts;
    public decimal ToMegawatts => Kilowatts / 1000m;
    public decimal ToHorsepower => Kilowatts * 1.341m;

    protected override IEnumerable<object?> GetAtomicValues()
    {
        yield return Kilowatts;
    }

    public override string ToString() => $"{Kilowatts} kW";
}
