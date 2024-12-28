using System.Text;
using Domain.Models.Automatas;
using Domain.Models.ValueObjects;

namespace Domain.Convertors.Convertors.Implementation;

public class RegexTree
{
    private readonly RegexNode _root;
    // FollowPositions = [ 0{'символ', [id записей из данной таблицы, в которые можно попасть после выполнения данной]}, 1{'a', [0, 1]}, 2{..} ]
    private readonly List<KeyValuePair<char, SortedSet<int>>> _followPositions = [];
    
    public RegexTree(string regex)
    {
        _root = new RegexNode(regex);
        _root.CalculateFunctions(0, _followPositions);
    }

    public Automata ToDfa()
    {
        HashSet<char> alphabet = [..RegexToDfaConvertor.Alphabet];
        alphabet.Remove(RegexToDfaConvertor.Epsilon);
        alphabet.Remove(RegexToDfaConvertor.FinalChar);

        List<SortedSet<int>> marked = [];
        List<SortedSet<int>> allStates = [];
        
        List<Argument> dfaAlphabet = [];
        dfaAlphabet.AddRange(alphabet.Select(c => new Argument(c.ToString())));
        List<State> states = []; // <- allStates
        List<Transition> transitions = []; // == d
        Dictionary<string, string> overrides = [];
        
        allStates.Add(_root.FirstPositions);
        states.Add(new State($"q{states.Count}", ContainsFinal(allStates.First()) ? "F" : null));
        GenerateOverride(overrides, allStates.First(), states.First());

        while (allStates.Count > marked.Count)
        {
            var currentState = allStates.First(state => !marked.Contains(state));
            marked.Add(currentState);
            foreach (var al in alphabet)
            {
                SortedSet<int> nextState = [];
                foreach (var val in currentState.Where(i => _followPositions[i].Key == al).SelectMany(i => _followPositions[i].Value))
                {
                    nextState.Add(val);
                }
                
                if (nextState.Count == 0) continue;

                if (!IsContainsState(allStates, nextState))
                {
                    allStates.Add(nextState);
                    states.Add(new State($"q{states.Count}", ContainsFinal(nextState) ? "F" : null));
                    GenerateOverride(overrides, nextState, states.Last());
                }
                transitions.Add(new Transition(
                    states.ElementAt(GetIndexOfState(allStates, currentState)), 
                    dfaAlphabet.First(a => a.Value == al.ToString()), 
                    states.ElementAt(GetIndexOfState(allStates, nextState))));
            }
        }
        
        return new Automata(overrides, states, transitions);
    }
    
    public void WriteToConsole()
    {
        _root.WriteLevel(0);
        for (var i = 0; i < _followPositions.Count; i++)
        {
            var follow = _followPositions[i];
            Console.Write($"{i} [{follow.Key}]: ");
            follow.Value.ToList().ForEach(v => Console.Write($"{v} "));
            Console.WriteLine();
        }
    }

    private static void GenerateOverride(Dictionary<string, string> overrides, SortedSet<int> stateSet, State state)
    {
        var sb = new StringBuilder();
        sb.AppendJoin(' ', stateSet);
        
        overrides.Add(sb.ToString(), state.Name);
    }
    
    private static int GetIndexOfState(List<SortedSet<int>> states, SortedSet<int> state)
    {
        var newStates = states.Where(st => st.Count == state.Count).ToList();
        return states.IndexOf(newStates.First(st =>
        {
            return !st.Where((t, i) => t != state.ToList()[i]).Any();
        }));
    }
    
    private static bool IsContainsState(List<SortedSet<int>> states, SortedSet<int> state)
    {
        var newStates = states.Where(st => st.Count == state.Count).ToList();
        return !newStates.All(st =>
        {
            return st.Where((t, i) => t != state.ToList()[i]).Any();
        });
    }

    private bool ContainsFinal(SortedSet<int> states)
    {
        return states.Any(state => _followPositions[state].Key == '#');
    }
}