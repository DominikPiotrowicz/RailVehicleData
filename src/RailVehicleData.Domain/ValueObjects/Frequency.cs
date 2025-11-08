namespace RailVehicleData.Domain.ValueObjects;

/// <summary>
/// Value Object representing electrical frequency (AC systems).
/// Stored in hertz.
/// </summary>
public sealed class Frequency : ValueObject
{
    public int Hertz { get; }

    public Frequency(int hertz)
    {
        if (hertz <= 0)
            throw new DomainException("Frequency must be greater than 0 Hz.");

        Hertz = hertz;
    }

    public static Frequency FromHertz(int hz) => new(hz);
    public static Frequency FromKilohertz(decimal khz) => new((int)(khz * 1000));

    public int ToHertz => Hertz;
    public decimal ToKilohertz => Hertz / 1000m;

    protected override IEnumerable<object?> GetAtomicValues()
    {
        yield return Hertz;
    }

    public override string ToString() => $"{Hertz} Hz";
}
