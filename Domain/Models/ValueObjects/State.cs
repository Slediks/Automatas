namespace Domain.Models.ValueObjects;

public class State : IComparable
{
    public string Name { get; set; }

    public State(string name)
    {
        Name = name;
        
    }

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
        return Name;
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
        Name);
}