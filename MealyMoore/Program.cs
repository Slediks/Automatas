using CSVFilesWorkers.FileReaders;
using Domain.Convertors;
using Domain.Convertors.Convertors;
using Domain.Models.Automatas;
using Visualizations;

namespace MealyMoore;

public class Program
{
    private static string _inputFilePath = string.Empty;
    public static void Main(string[] args)
    {
        var program = args[0];
        var automataType = args[1];
        _inputFilePath = args[2];

        Automata automata;
        switch (automataType)
        {
            case "mealy":
                break;
            case "moore":
                break;
            default:
                throw new ArgumentException($"Unknown automata type: {automataType}");
        }

        switch (program)
        {
            case "convert":
                automata = automataType == "mealy"
                    ? Convert(Mealy())
                    : Convert(Moore());
                break;
            default:
                throw new ArgumentException($"Unknown program type: {program}");
        }
        
        PrintAll(automata);
    }

    private static Mealy Mealy()
    {
        // Mealy
        var automata = new Mealy(new AutomataFileReader().CreateAutomataFromFile(_inputFilePath));
        automata
            .PrintToConsole()
            .PrintToImage();
        return automata;
    }
    
    private static Moore Moore()
    {
        // Moore
        var automata = new Moore(new AutomataFileReader().CreateAutomataFromFile(_inputFilePath)); //Automata creation
        automata
            .PrintToConsole() //Print to console as table  <- Moore
            .PrintToImage(); //Create .png file with graph  <- Moore
        return automata;
    }

    private static Moore Convert(Mealy automata)
    {
        return automata.Convert(new MealyMooreConvertor());
    }

    private static Mealy Convert(Moore automata)
    {
        return automata.Convert(new MooreMealyConvertor());
    }

    private static void PrintAll(Automata automata)
    {
        automata
            .PrintToConsole()
            .PrintToFile()
            .PrintToImage();
    }
}