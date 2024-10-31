using Domain.Models.Automatas;
using Visualizations.Implementation;

namespace Visualizations;

public static class Visualizer
{
    public static T PrintToConsole<T>(
        this T automata,
        string? title = null)
        where T : Automata
    {
        Console.WriteLine(title);
        new AutomataConsoleVisualizer(automata).Print();
        return automata;
    }

    public static T PrintToFile<T>(
        this T automata)
        where T : Automata
    {
        new AutomataFileVisualizer().WriteAutomataToFile(automata);
        return automata;
    }

    public static T PrintToImage<T>(this T automata)
        where T : Automata
    {
        Task task = new AutomataImageVisualizer(automata).ToImage();
        task.Wait();
        
        return automata;
    }
}