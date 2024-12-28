namespace Domain.Convertors.Convertors.Implementation;

public class RegexNode
{
    private bool _nullable;
    public readonly SortedSet<int> FirstPositions = [];
    private readonly SortedSet<int> _lastPositions = [];
    private readonly char _item;
    private int _position = -1;
    private readonly List<RegexNode> _children = [];

    public RegexNode(string regex)
    {
        Console.WriteLine(regex);
        if (regex.Length == 1 && IsLetter(regex[0]))
        {
            _item = regex[0];
            _nullable = _item == RegexToDfaConvertor.Epsilon;
            return;
        }

        var kleene = -1;
        var plus = -1;
        var or = -1;
        var concat = -1;
        var index = 0;

        while (index < regex.Length)
        {
            if (regex[index] == '(')
            {
                var bracketLevel = 1;
                index++;
                while (bracketLevel != 0 && index < regex.Length)
                {
                    switch (regex[index])
                    {
                        case '(':
                            bracketLevel++;
                            break;
                        case ')':
                            bracketLevel--;
                            break;
                    }

                    index++;
                }
            }
            else index++;
            
            if (index == regex.Length) break;

            if (IsConcat(regex[index]))
            {
                if (concat == -1) concat = index;
                continue;
            }

            switch (regex[index])
            {
                case '*':
                {
                    if (kleene == -1) kleene = index;
                    continue;
                }
                case '+':
                {
                    if (plus == -1) plus = index;
                    continue;
                }
                case '|':
                {
                    if (or == -1) or = index;
                    index++;
                    break;
                }
            }
        }

        if (or != -1)
        {
            _item = '|';
            _children.Add(new RegexNode(TrimBrackets(regex[..or])));
            _children.Add(new RegexNode(TrimBrackets(regex[(or+1)..])));
        }
        else if (concat != -1)
        {
            _item = '.';
            _children.Add(new RegexNode(TrimBrackets(regex[..concat])));
            _children.Add(new RegexNode(TrimBrackets(regex[concat..])));
        }
        else if (plus != -1)
        {
            _item = '+';
            _children.Add(new RegexNode(regex[..plus] + regex[..plus] + "*"));
        }
        else if (kleene != -1)
        {
            _item = '*';
            _children.Add(new RegexNode(TrimBrackets(regex[..kleene])));
        }
    }

    public int CalculateFunctions(int pos, List<KeyValuePair<char, SortedSet<int>>> fPoss)
    {
        if (IsLetter(_item))
        {
            FirstPositions.Add(pos);
            _lastPositions.Add(pos);
            _position = pos;
            fPoss.Add(new KeyValuePair<char, SortedSet<int>>(_item, []));
            return pos + 1;
        }

        foreach (var child in _children)
        {
            pos = child.CalculateFunctions(pos, fPoss);
        }

        if (_item == '.')
        {
            FirstPositions.UnionWith(_children.First().FirstPositions);
            if (_children.First()._nullable)
            {
                FirstPositions.UnionWith(_children.Last().FirstPositions);
            }
            
            _lastPositions.UnionWith(_children.Last()._lastPositions);
            if (_children.Last()._nullable)
            {
                _lastPositions.UnionWith(_children.First()._lastPositions);
            }

            _nullable = _children.First()._nullable && _children.Last()._nullable;

            UpdateFollowPoss(fPoss);
        }
        else if (_item == '|')
        {
            FirstPositions.UnionWith(_children.First().FirstPositions);
            FirstPositions.UnionWith(_children.Last().FirstPositions);
            
            _lastPositions.UnionWith(_children.First()._lastPositions);
            _lastPositions.UnionWith(_children.Last()._lastPositions);
            
            _nullable = _children.First()._nullable || _children.Last()._nullable;
        }
        else if (_item is '*' or '+')
        {
            FirstPositions.UnionWith(_children.First().FirstPositions);
            
            _lastPositions.UnionWith(_children.First()._lastPositions);

            _nullable = _item == '*' || _children.First()._nullable;
            
            UpdateFollowPoss(fPoss);
        }
        return pos;
    }

    public void WriteLevel(int level)
    {
        Console.Write($"{level} {_item} [ ");
        FirstPositions.ToList().ForEach(i => Console.Write($"{i} "));
        Console.Write("] [ ");
        _lastPositions.ToList().ForEach(i => Console.Write($"{i} "));
        Console.WriteLine($"] {_nullable} {_position}");
        _children.ForEach(child => child.WriteLevel(level + 1));
    }

    private void UpdateFollowPoss(List<KeyValuePair<char, SortedSet<int>>> fPoss)
    {
        foreach (var lastPosition in _children.First()._lastPositions)
        {
            foreach (var firstPosition in _children.Last().FirstPositions)
            {
                fPoss[lastPosition].Value.Add(firstPosition);
            }
        }
    }
    
    private static string TrimBrackets(string regex)
    {
        var newRegex = regex;
        while (newRegex.First() == '(' && newRegex.Last() == ')' && RegexToDfaConvertor.IsRegexValid(newRegex.Substring(1, newRegex.Length - 2)))
        {
            newRegex = newRegex.Substring(1, newRegex.Length - 2);
        }
        return newRegex;
    }
    
    private static bool IsLetter(char c) => RegexToDfaConvertor.Alphabet.Contains(c);
    
    private static bool IsConcat(char c) => c == '(' || IsLetter(c);
}