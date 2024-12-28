using System.Collections.Immutable;
using Domain.Convertors.Convertors.Implementation;
using Domain.Models.Automatas;
using Domain.Models.ValueObjects;

namespace Domain.Convertors.Convertors;

public static class RegexToDfaConvertor
{
    public static HashSet<char> Alphabet { get; private set; } = null!;
    public static readonly char Epsilon = Argument.Epsilon[0];
    public const char FinalChar = '#';

    private static readonly ImmutableHashSet<char> Functions = ImmutableHashSet.Create('(', ')', '+', '*', '|');

    public static Automata Convert(string input)
    {
        var regex = Preprocess(input);
        GenerateAlphabet(regex);

        var tree = new RegexTree(regex);
        tree.WriteToConsole();
        return tree.ToDfa();
    }
    public static bool IsRegexValid(string regex) => IsBracketsValid(regex) && IsOperationsValid(regex);

    private static string Preprocess(string input)
    {
        var regex = input;
        regex = regex.Replace(" ", "");
        CleanKleenePlus(regex);
        CleanDoubleKleeneOrPlus(regex);
        regex = "(" + regex + ")" + FinalChar;
        CleanEmptyBrackets(regex);
        return regex;
    }

    private static void GenerateAlphabet(string regex)
    {
        Alphabet = [..regex];
        Alphabet.RemoveWhere(item => Functions.Contains(item));
    }
    
    private static void CleanKleenePlus(string regex)
    {
        while (regex.Contains("+*") || regex.Contains("*+"))
        {
            regex = regex.Replace("+*", "*");
            regex = regex.Replace("*+", "*");
        }
    }

    private static void CleanDoubleKleeneOrPlus(string regex)
    {
        while (regex.Contains("**") || regex.Contains("++"))
        {
            regex = regex.Replace("**", "*");
            regex = regex.Replace("++", "+");
        }
    }

    private static void CleanEmptyBrackets(string regex)
    {
        while (regex.Contains("()"))
        {
            regex = regex.Replace("()", "");
        }
    }
    
    private static bool IsBracketsValid(string regex)
    {
        var bracketLevel = 0;
        foreach (var ch in regex)
        {
            switch (ch)
            {
                case '(':
                    bracketLevel++;
                    break;
                case ')':
                    bracketLevel--;
                    break;
            }

            if (bracketLevel < 0) return false;
        }
        return bracketLevel == 0;
    }

    private static bool IsOperationsValid(string regex)
    {
        for (var i = 0; i < regex.Length; i++)
        {
            if (regex[i] is '*' or '+')
            {
                if (i == 0) return false;
                if (regex[i - 1] is '(' or '|') return false;
            }

            if (regex[i] != '|') continue;
            if (i == 0 || i == regex.Length - 1) return false;
            if (regex[i - 1] is '(' or '|') return false;
            if (regex[i + 1] is ')' or '|') return false;
        }
        return true;
    }
}