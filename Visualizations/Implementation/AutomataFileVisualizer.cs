using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Domain.Models.Automatas;

namespace Visualizations.Implementation;

public class AutomataFileVisualizer
{
    public void WriteAutomataToFile(Automata automata)
    {
        var filePath = "output_"
                       + automata.GetType().Name.ToLower()
                       + ".csv";
        
        Console.WriteLine($"Writing to \"{filePath}\"");
        
        WriteRowsToCsvFile(ConvertAutomataToRows(automata), filePath);
        Console.WriteLine($"Wrote to \"{filePath}\"");
    }

    private List<List<string>> ConvertAutomataToRows(Automata automata)
    {
        var rows = new List<List<string>>();
        var row = new List<string>();
        var automataType = automata.GetType().Name;
        var states = automata.AllStates.Select(s => s.Name).ToList();
        var alphabet = automata.Alphabet.Select(a => a.Value).ToList();
        
        // If Moore --> create outs row
        if (automataType.Equals("Moore"))
        {
            row.Add("");
            foreach (var state in states)
            {
                row.Add(automata.Transitions.First(t => t.To.Name.Equals(state)).AdditionalData);
            }
            
            rows.Add([..row]);
            row.Clear();
        }
        
        // Create states row
        row.Add("");
        row.AddRange(states);
        rows.Add([..row]);
        row.Clear();

        
        foreach (var argument in alphabet)
        {
            row.Add(argument);
            foreach (var state in states)
            {
                row.Add(automata.Transitions
                    .Where(t => 
                        t.From.Name.Equals(state)
                        && t.Argument.Value.Equals(argument))
                    .Select(t =>
                    {
                        string transitionName = t.To.Name;
                        if (automataType != "Moore")
                        {
                            transitionName = $"{transitionName}/{t.AdditionalData}";
                        }

                        return transitionName;
                    }).First());
            }
            rows.Add([..row]);
            row.Clear();
        }
        
        return rows;
    }
    
    private void WriteRowsToCsvFile(List<List<string>> rows, string filePath)
    {
        var tableRows = rows.Select(r => new TableRow(r)).ToList();
        
        if (File.Exists(filePath)) File.Delete(filePath);
        
        var configuration = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = false,
            Delimiter = ";"
        };
        using var writer = new StreamWriter(filePath);
        using var csv = new CsvWriter(writer, configuration);
        
        csv.WriteRecords(tableRows);
        writer.Flush();
    }
    
    private class TableRow(List<string> row)
    {
        public string[] Row { get; set; } = row.ToArray();
    }
}