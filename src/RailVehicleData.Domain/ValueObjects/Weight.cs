namespace RailVehicleData.Domain.ValueObjects;

/// <summary>
/// Value Object representing a weight measurement.
/// Internally stored in kilograms.
/// </summary>
public sealed class Weight : ValueObject
{
    public decimal Kilograms { get; }

    public Weight(decimal kilograms)
    {
        if (kilograms <= 0)
            throw new DomainException("Weight must be greater than 0 kg.");

        Kilograms = kilograms;
    }

    public static Weight FromKilograms(decimal kg) => new(kg);
    public static Weight FromTons(decimal tons) => new(tons * 1000);
    public static Weight FromGrams(decimal grams) => new(grams / 1000);
    public static Weight FromPounds(decimal lbs) => new(lbs * 0.453592m);

    public decimal ToKilograms => Kilograms;
    public decimal ToTons => Kilograms / 1000;
    public decimal ToGrams => Kilograms * 1000;
    public decimal ToPounds => Kilograms * 2.20462m;

    protected override IEnumerable<object?> GetAtomicValues()
    {
        yield return Kilograms;
    }

    public override string ToString() => $"{ToTons:F2} t";
}
