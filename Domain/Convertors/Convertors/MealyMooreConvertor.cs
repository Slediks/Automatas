using Domain.Models.Automatas;
using Domain.Models.ValueObjects;

namespace Domain.Convertors.Convertors;

public class MealyMooreConvertor : IAutomataConvertor<Mealy, Moore>
{
    public Moore Convert(Mealy automata)
    {
        var newStates = BuildStateOverrides(automata);
        
        return new Moore(newStates, BuildTransitions(newStates, automata));
    }

    private static HashSet<Transition> BuildTransitions(Dictionary<State, (State, string)> states, Mealy automata)
    {
        var transitions = new HashSet<Transition>();
        
        foreach (var state in states)
        {
            var oldStateTransitions = new HashSet<Transition>(
                automata.Transitions
                    .Where(t => t.From.Equals(state.Value.Item1)));

            var stateTransitions = oldStateTransitions.Select(t => new Transition(
                from: state.Key,
                to: states
                    .First(s => s.Value.Equals((t.To, t.AdditionalData)))
                    .Key,
                argument: t.Argument,
                additionalData: t.AdditionalData));
            
            transitions.UnionWith(stateTransitions);
        }
        
        return transitions;
    }

    private static Dictionary<State, (State, string)> BuildStateOverrides(Mealy automata)
    {
        var states = new Dictionary<State, (State, string)>();
        var preStates = new HashSet<(State, string)>();

        foreach (var transition in automata.Transitions)
        {
            preStates.Add((transition.To, transition.AdditionalData));
        }

        if (!preStates.Select(s => s.Item1).Contains(automata.AllStates.First()))
        {
            states.Add(new State($"q{states.Count}", "-"), (automata.AllStates.First(), "-"));
        }
        
        foreach (var preState in preStates.Order())
        {
            states.Add(new State($"q{states.Count}", preState.Item2), preState);
        }

        return states;
    }
}