using Domain.Convertors.Convertors.Minimization.Implementation.Models;
using Domain.Models.Automatas;
using Domain.Models.ValueObjects;

namespace Domain.Convertors.Convertors.Minimization.Implementation;

public static class MinimizationGroupsConvertor
{
    public static List<MinimizationGroup> ParseFromStates(List<State> states)
    {
        var minimizationGroups = new List<MinimizationGroup> { new(states.First()) };

        foreach (var state in states.Skip(1))
        {
            bool addedToGroup = false;
            foreach (var minimizationGroup in minimizationGroups)
            {
                if (state.OutputSignal == minimizationGroup.GetStates().First().OutputSignal)
                {
                    minimizationGroup.Add(state);
                    addedToGroup = true;
                    break;
                }
            }

            if (!addedToGroup)
            {
                minimizationGroups.Add(new MinimizationGroup(state));
            }
        }

        return minimizationGroups;
    }

    public static List<MinimizationGroup> ParseFromOutputs(Automata automata)
    {
        var minimizationGroups = new List<MinimizationGroup>() { new(automata.AllStates.First()) };

        foreach (var state in automata.AllStates.Skip(1))
        {
            var curStateTransitions = automata
                .Transitions
                .Where(t => t.From == state)
                .ToList();
            
            bool addedToGroup = false;
            foreach (var minimizationGroup in minimizationGroups)
            {
                var curGroupTransitions = automata
                    .Transitions
                    .Where(t => t.From == minimizationGroup.GetStates().First())
                    .ToList();

                bool isEquivalent = true;
                foreach (var transition in curStateTransitions)
                {
                    if (!curGroupTransitions.Any(t => t.Argument == transition.Argument 
                                                     &&  t.AdditionalData == transition.AdditionalData))
                    {
                        isEquivalent = false;
                        break;
                    }
                }

                if (isEquivalent)
                {
                    minimizationGroup.Add(state);
                    addedToGroup = true;
                    break;
                }
            }

            if (!addedToGroup)
            {
                minimizationGroups.Add(new MinimizationGroup(state));
            }
        }

        return minimizationGroups;
    }

    public static Automata ConvertToAutomata(
        List<MinimizationGroup> groups,
        HashSet<Transition> oldTransitions)
    {
        var states = new HashSet<State>();
        var oldStateToNewState = new Dictionary<State, State>();
        var overrides = new Dictionary<string, string>();

        List<MinimizationGroup> reachableGroups = DeleteUnreachGroups(groups, oldTransitions);
        
        foreach (var group in reachableGroups)
        {
            var newState = new State($"s{states.Count.ToString()}", group.GetStates().First().OutputSignal);
            
            overrides.Add(string.Join(", ", group.GetStates()), newState.ToString());
            group.GetStates().ForEach(s => oldStateToNewState.Add(s, newState));
            
            states.Add(newState);
        }
        
        foreach (var group in groups.Where(g => !reachableGroups.Contains(g)))
        {
            overrides.Add(string.Join(", ", group.GetStates()), "\"Unreachable\"");
        }
        
        var transitions = new HashSet<Transition>();
        foreach (var transition in oldTransitions)
        {
            if (!(oldStateToNewState.ContainsKey(transition.From) && oldStateToNewState.ContainsKey(transition.To)))
            {
                continue;
            }
            
            var from = oldStateToNewState[transition.From];
            var to = oldStateToNewState[transition.To];
            var argument = transition.Argument;
            var additionalData = transition.AdditionalData;

            bool hasThisTransition = transitions.Any(t =>
                t.From.Equals(from) &&
                t.To.Equals(to) &&
                t.Argument.Equals(argument) &&
                t.AdditionalData.Equals(additionalData));
            if (!hasThisTransition)
            {
                transitions.Add(new Transition(from, argument, to, additionalData));
            }
        }

        return new Automata(
            overrides,
            states,
            transitions);
    }

    private static List<MinimizationGroup> DeleteUnreachGroups(List<MinimizationGroup> groups,
        HashSet<Transition> oldTransitions)
    {
        var reachableGroups = new List<MinimizationGroup> {groups.First()};
        var untestedGroups = new List<MinimizationGroup> {groups.First()};
        
        while (untestedGroups.Count != 0)
        {
            var fromGroup = untestedGroups.First();

            foreach (var transition in oldTransitions.Where(t => t.From.Equals(fromGroup.GetStates().First())))
            {
                var toGroup = groups.Single(g => g.GetStates().Contains(transition.To));
                if (reachableGroups.Contains(toGroup)) continue;
                reachableGroups.Add(toGroup);
                untestedGroups.Add(toGroup);
            }

            untestedGroups.Remove(fromGroup);
            
        }

        return reachableGroups;
    }
}