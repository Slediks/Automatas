using ConsoleTables;
using Domain.Models.Automatas;
using Domain.Models.ValueObjects;

namespace Visualizations.Implementation;

public class AutomataConsoleVisualizer(Automata automata)
{
    private readonly bool _isMoore = automata.GetType().Name != "Automata"
        ? automata.GetType().Name == "Moore"
        : automata.AllStates.First().OutputSignal != null;

    public void Print()
    {
        // Create columns
        string[] columns = BuildColumns().ToArray();

        // Create rows
        var rows = BuildRows(columns).ToArray();

        var table = new ConsoleTable(columns);
        table.Options.EnableCount = false;

        foreach (var row in rows)
        {
            table.AddRow(row.ToArray());
        }

        if (automata.Overrides != null)
        {
            Console.WriteLine("Overrides:");

            foreach (var automataOverride in automata.Overrides)
            {
                var stateOverride = $"| {automataOverride.Key.Name} = {automataOverride.Value} ";
                Console.WriteLine(stateOverride);
            }

            Console.WriteLine();
        }

        table.Write();
    }

    private IEnumerable<List<string>> BuildRows(string[] columns)
    {
        // if (automata.Overrides != null)
        // {
        //     var additionalRow = new List<string> { "" };
        //     additionalRow.AddRange(automata.Overrides.Values.Select(v => v.Item2));
        //
        //     yield return additionalRow;
        // }

        foreach (Argument argument in automata.Alphabet)
        {
            var row = new List<string>();

            foreach (string column in columns)
            {
                if (column == "Id")
                {
                    row.Add(argument.Value);
                    continue;
                }

                row.Add(automata.Transitions
                    .Where(t =>
                        t.Argument.Equals(argument) &&
                        t.From.Name == column.Split("/")[0])
                    .Select(t =>
                    {
                        string transitionName = t.To.Name;
                        if (!_isMoore)
                        {
                            transitionName = $"{transitionName}/{t.AdditionalData}";
                        }

                        return transitionName;
                    })
                    .First());
            }

            yield return row;
        }
    }

    private IEnumerable<string> BuildColumns()
    {
        var columns = new List<string> { "Id" };


        columns.AddRange(_isMoore
            ? automata.AllStates.Select(s => s.ToString())
            : automata.AllStates.Select(s => s.Name));

        return columns;
    }
}