using System.Security.Cryptography.X509Certificates;
using Domain.Models.ValueObjects;

namespace Domain.Models.Automatas;

public class Automata
{
    public readonly HashSet<Argument> Alphabet;
    public readonly HashSet<Transition> Transitions;
    public readonly HashSet<State> AllStates;
    public readonly Dictionary<string, string>? Overrides;

    public Automata(
        IEnumerable<Argument> alphabet,
        IEnumerable<Transition> transitions,
        IEnumerable<State> allStates)
    {
        Alphabet = alphabet.ToHashSet();
        AllStates = allStates.ToHashSet();
        Transitions = transitions.ToHashSet();

        foreach (var transition in Transitions)
        {
            if (!AllStates.Contains(transition.From))
            {
                throw new ArgumentException( 
                    $"Some of the transitions has a state that is not presented in the {nameof(AllStates)}. " + 
                    $"Transition: {transition}. State: {transition.From}");
            }

            if (!AllStates.Contains(transition.To))
            {
                throw new ArgumentException( 
                    $"Some of the transitions has a state that is not presented in the {nameof(AllStates)}. " + 
                    $"Transition: {transition}. State: {transition.To}");
            }

            if (!Alphabet.Contains(transition.Argument))
            {
                throw new ArgumentException( 
                    $"Some of the transitions has an argument that is not presented in the {nameof(Alphabet)}. " + 
                    $"Transition: {transition}. Argument: {transition.Argument}");
            }
        }
    }

    public Automata(Dictionary<string, string> overrides, IEnumerable<State> states, ICollection<Transition> transitions)
        : this(
            transitions.Select(t => t.Argument).Distinct(),
            transitions,
            states)
    {
        Overrides = overrides;
    }

    public Automata(IEnumerable<State> allStates, ICollection<Transition> transitions)
        : this(transitions.Select(t => t.Argument).Distinct(),
            transitions,
            allStates)
    {
    }
    
    public Automata(ICollection<Transition> transitions)
        : this(
            transitions.Select(t => t.Argument).Distinct(),
            transitions,
            transitions.SelectMany(t => new[] {t.From, t.To}))
    {
    }

    protected Automata(Automata automata)
        : this(
            automata.Alphabet,
            automata.Transitions,
            automata.AllStates)
    {
    }

    public HashSet<State> Move(State from, Argument argument, HashSet<State>? epsClosures = null)
    {
        epsClosures ??= EpsClosure(from).ToHashSet();

        return Transitions
            .Where(transition =>
                transition.Argument.Equals(argument) &&
                epsClosures.Contains(transition.From))
            .Select(transition => transition.To)
            .ToHashSet();
    }

    public Dictionary<State, HashSet<State>> EpsClosure()
    {
        var result = new Dictionary<State, HashSet<State>>();
        foreach (var state in AllStates)
        {
            result[state] = EpsClosure(state);
        }

        return result;
    }

    private HashSet<State> EpsClosure(State from)
    {
        var closures = new HashSet<State> { from };

        var statesToProcess = new Queue<State>();
        var processedStates = new HashSet<State>();
        statesToProcess.Enqueue(from);

        while (statesToProcess.Count != 0)
        {
            var state = statesToProcess.Dequeue();
            if (!processedStates.Add(state))
            {
                continue;
            }

            var stateTransitions = new Queue<Transition>(Transitions.Where(transition => transition.From == state));

            while (stateTransitions.Count != 0)
            {
                var transition = stateTransitions.Dequeue();
                if (transition.Argument.Value != Argument.Epsilon)
                {
                    continue;
                }

                closures.Add(transition.To);
                statesToProcess.Enqueue(transition.To);
            }
        }

        return closures;
    }
    
    public Automata Clone() => new(
        alphabet: Alphabet.Select(a => a.Clone()).ToHashSet(),
        transitions: Transitions.Select(t => t.Clone()).ToHashSet(),
        allStates: AllStates.Select(s => s.Clone()).ToHashSet());
}