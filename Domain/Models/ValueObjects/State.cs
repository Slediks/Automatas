namespace Domain.Models.ValueObjects;

public class State(string name, string? outputSignal = null) : IComparable
{
    public string Name { get; } = name;
    public string? OutputSignal { get; set; } = outputSignal;

    public override bool Equals(object? obj)
    {
        return obj is State other && Equals(other);
    }

    public bool Equals(State other)
    {
        return Name == other.Name && OutputSignal == other.OutputSignal;
    }

    public override int GetHashCode()
    {
        return OutputSignal != null
            ? HashCode.Combine(Name.GetHashCode(), OutputSignal.GetHashCode())
            : Name.GetHashCode();
    }

    public override string ToString()
    {
        return OutputSignal != null
            ? Name + "/" + OutputSignal
            : Name;
    }

    public int CompareTo(object? obj)
    {
        if (obj is not State other)
        {
            throw new ArgumentException($"Object must be of type {nameof(State)}");
        }
        
        return String.CompareOrdinal(Name, other.Name);
    }

    public static bool operator ==(State left, State right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(State left, State right)
    {
        return !left.Equals(right);
    }

    public State Clone() => new(
        Name, OutputSignal);
}