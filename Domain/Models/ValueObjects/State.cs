namespace Domain.Models.ValueObjects;

public class State(string name, string? outputSignal = null, bool isFinal = false) : IComparable
{
    public string Name { get; set; } = name;
    public string? OutputSignal { get; set; } = outputSignal;
    
    public bool IsFinal { get; set; } = isFinal;

    public override bool Equals(object? obj)
    {
        return obj is State other && Equals(other);
    }

    public bool Equals(State other)
    {
        return Name == other.Name;
    }

    public override int GetHashCode()
    {
        return Name.GetHashCode();
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

    public static bool operator ==(State? left, State? right)
    {
        if (left is null && right is null) return true;
        if (left is null || right is null) return false;
        return left.Equals(right);
    }

    public static bool operator !=(State left, State right)
    {
        return !left.Equals(right);
    }

    public State Clone() => new(
        Name, OutputSignal);
}