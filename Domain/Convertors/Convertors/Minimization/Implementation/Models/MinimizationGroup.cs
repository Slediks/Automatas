using Domain.Models.ValueObjects;

namespace Domain.Convertors.Convertors.Minimization.Implementation.Models;

public class MinimizationGroup
{
    private readonly Dictionary<string, State> _states = new();
    
    public MinimizationGroup(){}

    public MinimizationGroup(State state)
    {
        _states.Add(state.Name, state);
    }

    public MinimizationGroup(IEnumerable<State> states)
    {
        foreach (var state in states)
        {
            _states.Add(state.Name, state);
        }
    }

    public void Add(State state)
    {
        if (!_states.TryAdd(state.Name, state))
        {
            throw new InvalidOperationException($"State with name '{state.Name}' already exists in {nameof(MinimizationGroup)}.");
        }
    }

    public void Remove(string name)
    {
        if (!_states.Remove(name))
        {
            throw new InvalidOperationException($"State with name '{name}' does not exists in {nameof(MinimizationGroup)}.");
        }
    }
    
    public bool Contains(string name) => _states.ContainsKey(name);
    
    public int Count => _states.Count;
    
    public bool Any() => _states.Any();
    
    public bool Any(Func<State, bool> predicate) => _states.Values.Any(predicate);
    
    public List<State> GetStates() => _states.Values.ToList();
    
    public MinimizationGroup Copy() => new(_states.Values);
}