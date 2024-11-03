using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using Domain.Models.Automatas;
using Domain.Models.ValueObjects;

namespace Visualizations.Implementation;

public class AutomataImageVisualizer(Automata automata)
{
    private readonly string _graphvizPath = "./Graphviz/bin/dot.exe";
    private readonly string _graphName = "Graph";
    private readonly bool _isMoore = automata.AllStates.First().OutputSignal != null;

    public async Task ToImage(string imagePath)
    {
        
        var tempDataPath = $"{imagePath}.dot";

        await File.WriteAllTextAsync(
            tempDataPath,
            BuildData());

        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = _graphvizPath,
                Arguments = $"-Tpng {tempDataPath} -o {imagePath}",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };
        
        process.Start();
        
        var work = process.WaitForExitAsync();

        await work;
        File.Delete(tempDataPath);
    }

    private string BuildData()
    {
        var dataBuilder = new StringBuilder();
        dataBuilder.Append($"digraph {_graphName}");
        dataBuilder.Append('{');
        dataBuilder.Append("rankdir=LR;");
        
        dataBuilder.Append('{');
        dataBuilder.AppendJoin("", BuildNodes());
        dataBuilder.Append('}');
        
        dataBuilder.Append('{');
        dataBuilder.AppendJoin("", BuildTransitions());
        dataBuilder.Append('}');
        
        dataBuilder.Append('}');
        
        return Regex.Replace( dataBuilder.ToString(), @"\s\s+", " " );
    }

    private IEnumerable<string> BuildNodes()
    {
        foreach (var state in automata.AllStates)
        {
            var style = "filled";
            var label = _isMoore
                ? state.ToString()
                : state.Name;
            var shape = "circle";
            var fillcolor = "white";
            
            yield return $"{state.Name} [style=\"{style}\" fillcolor=\"{fillcolor}\" label=\"{label}\" shape=\"{shape}\"];";
        }
    }

    private IEnumerable<string> BuildTransitions()
    {
        foreach (var transition in automata.Transitions)
        {
            var label = transition.Argument.Value
                        + (!_isMoore
                            ? "/" + transition.AdditionalData
                            : "");
            yield return $"{transition.From.Name} -> {transition.To.Name} [label=\"{label}\"];";
        }
    }
}