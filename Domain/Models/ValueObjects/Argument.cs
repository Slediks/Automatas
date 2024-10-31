namespace Domain.Models.ValueObjects;

public class Argument(string value) : IComparable
{
    public readonly string Value = value;

    public override bool Equals(object? obj)
    {
        return obj is Argument other && Equals(other);
    }

    public bool Equals(Argument other)
    {
        return Value == other.Value;
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public override string ToString()
    {
        return Value;
    }

    public int CompareTo(object? obj)
    {
        if (obj is not Argument other)
        {
            throw new ArgumentException($"Object must be of type {nameof(Argument)}");
        }
        
        return String.CompareOrdinal(Value, other.Value);
    }

    public static bool operator ==(Argument left, Argument right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Argument left, Argument right)
    {
        return !left.Equals(right);
    }
    
    public Argument Clone() => new Argument(Value);
}