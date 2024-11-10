using Domain.Convertors.Convertors.Implementation;
using Domain.Models.Automatas;
using Domain.Models.ValueObjects;

namespace Domain.Convertors.Convertors;

public class NfaToDfaConvertor : IAutomataConvertor<Automata>
{
    public Automata Convert(Automata automata)
    {
        var dfaTransitions = new HashSet<Transition>();
        var dfaStartState = new CollapsedState(automata.AllStates.First());

        var alphabet = automata.Alphabet
            .Where(a => a.Value != Argument.Epsilon)
            .ToHashSet();

        var stateToEpsClosures = automata
            .EpsClosure();

        var queue = new Queue<CollapsedState>();
        queue.Enqueue(dfaStartState);
        var processedStates = new HashSet<CollapsedState>();

        while (queue.Count != 0)
        {
            var fromState = queue.Dequeue();
            processedStates.Add(fromState);

            foreach (var argument in alphabet)
            {
                var reachableStates = fromState.States
                    .SelectMany(s => automata.Move(s, argument, stateToEpsClosures[s]))
                    .ToHashSet();
                if (reachableStates.Count == 0)
                {
                    continue;
                }

                var toState = new CollapsedState(reachableStates);
                if (!processedStates.Contains(toState))
                {
                    queue.Enqueue(toState);
                }

                dfaTransitions.Add(new Transition(
                    fromState.ToState(),
                    argument,
                    toState.ToState()));
            }
        }

        var dfa = BuildDfa(
            dfaTransitions,
            processedStates.Select(c => c.ToState()),
            alphabet);

        return dfa;
    }

    private static Automata BuildDfa(ICollection<Transition> transitions, IEnumerable<State> states, IEnumerable<Argument> alphabet)
    {
        var overrides = new Dictionary<string, string>();
        var statesList = states.ToList();
        foreach (var state in statesList)
        {
            var newName = $"S{overrides.Count}";
            overrides.Add(state.Name, newName);
            state.Name = newName;
        }

        foreach (var transition in transitions)
        {
            transition.From = statesList.Single(s => s.Name == overrides[transition.From.Name]);
            transition.To = statesList.Single(s => s.Name == overrides[transition.To.Name]);
        }

        return new Automata(
            overrides,
            statesList,
            transitions);
    }
}