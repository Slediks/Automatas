using ConsoleTables;
using Domain.Models.Automatas;
using Domain.Models.ValueObjects;

namespace Visualizations.Implementation;

public class AutomataConsoleVisualizer(Automata automata)
{
    public void Print()
    {
        // Create columns
        string[] columns = BuildColumns().ToArray();
        
        // Create rows
        var rows = BuildRows(columns).ToArray();

        var table = new ConsoleTable(columns);
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

                var transitions = automata.Transitions
                    .Where(t =>
                        t.Argument.Equals(argument) &&
                        t.From.Name == column.Split("/")[0])
                    .Select(t =>
                    {
                        string transitionName = t.To.Name;
                        if (automata.GetType().Name != "Moore")
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
        // columns.AddRange(automata.Alphabet.Select(a => a.Value).Order());
        // columns.AddRange(automata.Overrides != null
        //     ? automata.Overrides.Keys.Select(k => k.Name)
        //     : automata.AllStates.Select(s => s.Name).Order());

        if (automata.GetType() == typeof(Moore))
        {
            if (automata.Overrides != null)
            {
                foreach (var automataOverride in automata.Overrides)
                {
                    columns.Add(automataOverride.Key.Name + "/" + automataOverride.Value.Item2);
                }
            }
            else
            {
                foreach (var state in automata.AllStates)
                {
                    columns.Add(state.Name + "/" + automata.Transitions.First(t => t.To.Name == state.Name).AdditionalData);
                }
            }
        }
        else
        {
            columns.AddRange(automata.AllStates.Select(s => s.Name));
        }

        return columns;
    }
}