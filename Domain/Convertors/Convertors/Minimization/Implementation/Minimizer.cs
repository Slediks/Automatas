using Domain.Convertors.Convertors.Minimization.Implementation.Models;
using Domain.Models.Automatas;
using Domain.Models.ValueObjects;

namespace Domain.Convertors.Convertors.Minimization.Implementation;

public class Minimizer : IAutomataConvertor<Automata>
{
    private Automata _automata = null!;

    public Automata Minimize(Automata automata)
    {
        _automata = automata;
        return MinimizationGroupsConvertor.ConvertToAutomata(
            FindEquivalentStates(),
            _automata.Alphabet,
            _automata.Transitions);
    }

    private List<MinimizationGroup> FindEquivalentStates()
    {
        var groups = _automata.AllStates.First().OutputSignal != null
            ? MinimizationGroupsConvertor.ParseFromStates(_automata.AllStates)
            : MinimizationGroupsConvertor.ParseFromOutputs(_automata);
        
        bool hasChanges = true;
        while (hasChanges)
        {
            var newGroups = new List<MinimizationGroup>();

            hasChanges = false;
            foreach (var group in groups)
            {
                var curGroup = group.Copy();
                newGroups.Add(curGroup);
                
                var createdGroups = new List<MinimizationGroup>();

                var groupExample = curGroup.GetStates().First();
                var statesQueue = new Queue<State>(curGroup.GetStates().Skip(1));
                while (statesQueue.Any())
                {
                    var curState = statesQueue.Dequeue();
                    if (HasSameEquivalenceClass(groupExample, curState, groups))
                    {
                        continue;
                    }
                    
                    curGroup.Remove(curState.Name);
                    
                    bool addedToGroup = false;
                    foreach (var createdGroup in createdGroups)
                    {
                        var createdGroupExample = createdGroup.GetStates().First();
                        if (HasSameEquivalenceClass(curState, createdGroupExample, groups))
                        {
                            createdGroup.Add(curState);
                            addedToGroup = true;
                            break;
                        }
                    }

                    if (!addedToGroup)
                    {
                        createdGroups.Add(new MinimizationGroup(curState));
                    }
                }

                if (createdGroups.Any())
                {
                    hasChanges = true;
                }
                
                newGroups.AddRange(createdGroups);
            }
            
            groups = newGroups;
        }

        return groups;
    }

    private bool HasSameEquivalenceClass(
        State firstState,
        State secondState,
        ICollection<MinimizationGroup> groups)
    {
        foreach (var argument in _automata.Alphabet)
        {
            var firstGroup = groups.SingleOrDefault(g =>
                g.Any(s => s == _automata
                    .Transitions
                    .Single(t =>
                        t.From == firstState &&
                        t.Argument == argument)
                    .To));
            if (firstGroup == null)
            {
                return false;
            }

            var secondGroup = groups.SingleOrDefault(g =>
                g.Any(s => s == _automata
                    .Transitions
                    .Single(t =>
                        t.From == secondState &&
                        t.Argument == argument)
                    .To));
            if (secondGroup == null)
            {
                return false;
            }

            if (!firstGroup.Equals(secondGroup))
            {
                return false;
            }
        }

        return true;
    }
}