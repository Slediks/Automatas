using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Domain.Models.Automatas;
using Domain.Models.ValueObjects;

namespace FileWorkers.FileReaders;

public class CsvFileReader
{
    public Automata CreateAutomataFromFile(string filePath)
    {
        // Get table as rows<cells>
        var records = ReadFileAndParse(filePath).ToList();

        //Type of table
        var headerRowsCnt = records.Count(r => r.First().Equals(""));

        List<string>? additionalData = headerRowsCnt == 2
            ? records.First().Skip(1).ToList()
            : null;
        
        var states = GetStatesFromRecord(records[headerRowsCnt - 1]);
        if (additionalData != null)
        {
            for (int i = 0; i < states.Count; i++)
            {
                states[i].OutputSignal = additionalData[i];
            }
        }
        
        var alphabet = GetArgumentsFromRecord(records.Skip(headerRowsCnt));
        var transitions = GetTransitionsFromRecord(records.Skip(headerRowsCnt).ToList(),
            states,
            alphabet);
        
        return new Automata(alphabet, transitions, states);
    }
    
    private IEnumerable<List<string>> ReadFileAndParse(string filePath)
    {
        var configuration = new CsvConfiguration(CultureInfo.CurrentCulture)
        {
            HasHeaderRecord = false,
            Delimiter = ";"
        };
        using var reader = new StreamReader(filePath);
        using var csv = new CsvReader(reader, configuration);
        
        var records = csv.GetRecords<dynamic>();
        
        foreach (var record in records)
        {
            var row = new List<string>();
            
            foreach (var r in record)
            {
                row.Add(r.Value);
            }

            yield return row;
        }
    }

    private List<State> GetStatesFromRecord(List<string> record)
    {
        return record
            .Skip(1)
            .Select(s => new State(s))
            .ToList();
    }

    private List<Argument> GetArgumentsFromRecord(IEnumerable<List<string>> record)
    {
        return record
            .Select(l => new Argument(l.First()))
            .ToList();
    }

    private List<Transition> GetTransitionsFromRecord(List<List<string>> records, List<State> states,
        List<Argument> alphabet)
    {
        var transitions = new List<Transition>();
        
        for (int i = 0; i < records.Count(); i++)
        {
            for (int j = 1; j < records[0].Count; j++)
            {
                var parsedCell = records[i][j].Split("/");

                transitions.Add(new Transition(
                    from: states[j - 1],
                    to: states.First(s => s.Name == parsedCell[0]),
                    argument: alphabet[i],
                    additionalData: states.First(s => s.Name == parsedCell[0]).OutputSignal ?? parsedCell[1]));
            }
        }

        return transitions;
    }
}