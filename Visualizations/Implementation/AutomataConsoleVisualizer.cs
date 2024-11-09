using ConsoleTables;
using Domain.Models.Automatas;
using Domain.Models.ValueObjects;

namespace Visualizations.Implementation;

public class AutomataConsoleVisualizer(Automata automata)
{
    private readonly bool _isMoore = automata.GetType().Name != "Automata"
        ? automata.GetType().Name == "Moore"
        : automata.AllStates.Any(s => s.OutputSignal != null);

    public void Print()
    {
        // Create columns
        string[] columns = BuildColumns().ToArray();

        // Create rows
        var rows = BuildRows(columns).ToArray();

        var table = new ConsoleTable(columns)
        {
            Options =
            {
                EnableCount = false
            }
        };

        foreach (var row in rows)
        {
            table.AddRow(row.ToArray());
        }

        if (automata.Overrides != null)
        {
            Console.WriteLine("Overrides:");

            foreach (var stateOverride in automata.Overrides.Select(automataOverride =>
                         $"| {automataOverride.Key} -> {automataOverride.Value.Name}"))
            {
                Console.WriteLine(stateOverride);
            }

            Console.WriteLine();
        }

        table.Write();
    }

    private IEnumerable<List<string>> BuildRows(string[] columns)
    {
        foreach (var argument in automata.Alphabet)
        {
            var row = new List<string>();

            foreach (var column in columns)
            {
                if (column == "Id")
                {
                    row.Add(argument.Value);
                    continue;
                }

                var transitions = automata.Transitions
                    .Where(t =>
                        t.Argument.Equals(argument) &&
                        t.From.Name == column.Split("/")[0])
                    .Select(t =>
                    {
                        var transitionName = t.To.Name;
                        if (!_isMoore)
                        {
                            transitionName = $"{transitionName}/{t.AdditionalData}";
                        }

                        return transitionName;
                    })
                    .ToList();
                row.Add(string.Join(",", transitions));
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