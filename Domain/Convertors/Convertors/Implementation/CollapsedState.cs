using Domain.Models.ValueObjects;

namespace Domain.Convertors.Convertors.Implementation;

internal class CollapsedState
{
    public string Name { get; }
    public bool IsFinal => OutputSignal == "F";
    public readonly string? OutputSignal;
    
    public readonly HashSet<State> States = new();

    public CollapsedState(State state)
    {
        Name = state.Name;
        OutputSignal = state.OutputSignal;
        States.Add(state);
    }

    public CollapsedState(HashSet<State> states, string? outputSignal = null)
    {
        Name = string.Join(", ", states.Select(s => s.Name).OrderBy(s => s));
        OutputSignal = outputSignal;

        foreach (var state in states)
        {
            OutputSignal ??= state.OutputSignal;
            States.Add(state);
        }
    }

    public State ToState()
    {
        return new State(Name, OutputSignal, IsFinal);
    }

    public override bool Equals(object? obj)
    {
        return obj is CollapsedState other && Equals(other);
    }

    private bool Equals(CollapsedState? other)
    {
        return Name == other?.Name;
    }

    public override int GetHashCode()
    {
        return Name.GetHashCode();
    }
}