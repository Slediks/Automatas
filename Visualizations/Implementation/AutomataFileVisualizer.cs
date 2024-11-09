using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Domain.Models.Automatas;

namespace Visualizations.Implementation;

public class AutomataFileVisualizer
{
    private bool _isMoore = false;
    public void WriteAutomataToFile(Automata automata, string filePath)
    {
        _isMoore = automata.GetType().Name != "Automata"
            ? automata.GetType().Name == "Moore"
            : automata.AllStates.Any(s => s.OutputSignal != null);
        WriteRowsToCsvFile(ConvertAutomataToRows(automata), filePath);
    }

    private List<List<string>> ConvertAutomataToRows(Automata automata)
    {
        var rows = new List<List<string>>();
        var row = new List<string>();
        var states = automata.AllStates.Select(s => s.Name).ToList();
        var alphabet = automata.Alphabet.Select(a => a.Value).ToList();
        
        // If Moore --> create outs row
        if (_isMoore)
        {
            row.Add("");
            row.AddRange(automata.AllStates.Select(s => s.OutputSignal ?? ""));
            
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
                var transitions = automata.Transitions
                    .Where(t => 
                        t.From.Name.Equals(state)
                        && t.Argument.Value.Equals(argument))
                    .Select(t =>
                    {
                        string transitionName = t.To.Name;
                        if (!_isMoore)
                        {
                            transitionName = $"{transitionName}/{t.AdditionalData}";
                        }

                        return transitionName;
                    }).ToList();
                row.Add(string.Join(',', transitions));
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