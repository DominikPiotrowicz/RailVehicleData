namespace RailVehicleData.Domain.ValueObjects;

/// <summary>
/// Base class for Value Objects.
/// Value Objects are immutable and are compared by their content, not by identity.
/// </summary>
public abstract class ValueObject : IEquatable<ValueObject>
{
    /// <summary>
    /// Gets the atomic values of this value object.
    /// </summary>
    protected abstract IEnumerable<object?> GetAtomicValues();

    /// <summary>
    /// Determines equality based on atomic values.
    /// </summary>
    public override bool Equals(object? obj)
    {
        if (obj is null || obj.GetType() != GetType())
            return false;

        if (obj is not ValueObject other)
            return false;

        return GetAtomicValues().SequenceEqual(other.GetAtomicValues());
    }

    public bool Equals(ValueObject? other)
    {
        return Equals(other as object);
    }

    public override int GetHashCode()
    {
        return GetAtomicValues()
            .Aggregate(
                default(HashCode),
                (hashCode, value) =>
                {
                    hashCode.Add(value);
                    return hashCode;
                })
            .ToHashCode();
    }

    public static bool operator ==(ValueObject left, ValueObject right)
    {
        if (left is null || right is null)
            return Equals(left, right);

        return left.Equals(right);
    }

    public static bool operator !=(ValueObject left, ValueObject right)
    {
        return !(left == right);
    }
}
