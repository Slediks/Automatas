using System.Text.RegularExpressions;
using Domain.Models.Automatas;
using Domain.Models.ValueObjects;

namespace Domain.Convertors.Convertors;

public static class GrammarToNfaConvertor
{
    private static bool _isLeft = false;
    private const string FinalName = "FinalState";

    public static Automata Convert(IEnumerable<string> lines)
    {
        var grammar = ParseLines(lines);
        
        if (grammar.Any(pair => pair.Value.Any(s => Regex.IsMatch(s, @"<(?:\d|\w)+> \S"))))
        {
            _isLeft = true;
        }

        var oldStatesToNewStates = CreateNewStates(grammar.Keys.ToList());
        var transitions = CreateTransitions(oldStatesToNewStates, grammar);

        
        return new Automata(oldStatesToNewStates, transitions);
    }

    private static Dictionary<string, List<string>> ParseLines(IEnumerable<string> lines) // return [state, [transition]]
    {
        var grammar = new Dictionary<string, List<string>>();
        var linesQueue = new Queue<string>(lines.Where(s => !string.IsNullOrWhiteSpace(s))) ;

        while (linesQueue.Count != 0)
        {
            var line = linesQueue.Dequeue();
            
            while (linesQueue.Count != 0 && !linesQueue.Peek().Contains("->"))
            {
                line += linesQueue.Dequeue();
            }
            
            var transitions = line.Split("->");
            grammar.Add(transitions[0].Trim(), transitions[1].Split('|').Select(s => s.Trim()).ToList()); 
        }

        return grammar;
    }

    private static Dictionary<string, State> CreateNewStates(List<string> oldStates) // return [oldStateName, newState]
    {
        var states = new Dictionary<string, State>();

        if (_isLeft)
        {
            states.Add(FinalName, new State($"q{states.Count}"));
            
            foreach (var state in oldStates.Skip(1))
            {
                var newName = $"q{states.Count}";

                states.Add(state, new State(newName));
            }
            
            states.Add(oldStates.First(), new State($"q{states.Count}", "F", true));
        }
        else
        {
            foreach (var state in oldStates)
            {
                var newName = $"q{states.Count}";
                
                states.Add(state, new State(newName));
            }
            
            states.Add(FinalName, new State($"q{states.Count}", "F", true));
        }
        
        return states;
    }

    private static HashSet<Transition> CreateTransitions(Dictionary<string, State> states,
        Dictionary<string, List<string>> grammar)
    {
        var transitions = new HashSet<Transition>();

        foreach (var grammarState in grammar)
        {
            foreach (var grammarTransition in grammarState.Value.Select(value => value.Split(' ')))
            {
                State from;
                Argument argument;
                State to;

                if (grammarTransition.Length == 1)
                {
                    argument = new Argument(grammarTransition[0]);
                    if (_isLeft)
                    {
                        from = states[FinalName];
                        to = states[grammarState.Key];
                    }
                    else
                    {
                        from = states[grammarState.Key];
                        to = states[FinalName];
                    }
                }
                else
                {
                    if (_isLeft)
                    {
                        from = states[grammarTransition[0]];
                        argument = new Argument(grammarTransition[1]);
                        to = states[grammarState.Key];
                    }
                    else
                    {
                        from = states[grammarState.Key];
                        argument = new Argument(grammarTransition[0]);
                        to = states[grammarTransition[1]];
                    }
                }
                
                transitions.Add(new Transition(from, argument, to));
            }
        }

        return transitions;
    }
}