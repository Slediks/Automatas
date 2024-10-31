using Domain.Models.Automatas;
using Visualizations.Implementation;

namespace Visualizations;

public static class Visualizer
{
    public static T PrintToConsole<T>(
        this T automata)
        where T : Automata
    {
        Console.WriteLine($"|------<  {automata.GetType().Name}  >------|");
        
        new AutomataConsoleVisualizer(automata).Print();
        
        return automata;
    }

    public static T PrintToFile<T>(
        this T automata)
        where T : Automata
    {
        var filePath = $"output_{automata.GetType().Name.ToLower()}.csv";
        
        new AutomataFileVisualizer().WriteAutomataToFile(automata, filePath);
        
        Console.WriteLine($"Table created: {filePath}");
        Console.WriteLine();
        
        return automata;
    }

    public static T PrintToImage<T>(this T automata)
        where T : Automata
    {
        var path = $"{automata.GetType().Name.ToLower()}.png";
        
        Task task = new AutomataImageVisualizer(automata).ToImage(path);
        task.Wait();
        
        Console.WriteLine($"Graph created: {path}");
        Console.WriteLine();
        
        return automata;
    }
}