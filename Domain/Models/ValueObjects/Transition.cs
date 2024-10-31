namespace Domain.Models.ValueObjects;

public class Transition
{
    public State From { get; set; }
    public Argument Argument { get; }
    public State To { get; set; }

    public string AdditionalData { get; }

    public Transition(
        State from,
        Argument argument,
        State to,
        string additionalData)
    {
        From = from;
        Argument = argument;
        To = to;
        AdditionalData = additionalData;
    }

    public override bool Equals(object? obj)
    {
        return obj is Transition other && Equals(other);
    }

    public bool Equals(Transition other)
    {
        return From == other.From && Argument == other.Argument && To == other.To;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(From.GetHashCode(), Argument.GetHashCode(), To.GetHashCode());
    }

    public override string ToString()
    {
        return $"From: {From}; To: {To}; Argument: {Argument}";
    }

    public static bool operator ==(Transition left, Transition right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Transition left, Transition right)
    {
        return !left.Equals(right);
    }
    
    public Transition Clone() => new(From.Clone(), Argument.Clone(), To.Clone(), AdditionalData);
}