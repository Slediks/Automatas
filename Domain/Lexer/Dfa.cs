namespace Domain.Lexer;

public struct Dfa
{
    public readonly List<List<int>> Table;
    public readonly List<int> Terminals;
    public readonly List<Func<char, bool>> TransConditions;

    public Dfa(List<List<int>> table, List<int> terminals, List<Func<char, bool>> transConditions)
    {
        Table = table;
        Terminals = terminals;
        TransConditions = transConditions;

        if (table[0].Count != transConditions.Count)
        {
            throw new ArgumentException("Table must have the same length");
        }
    }
}