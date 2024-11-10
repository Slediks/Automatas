using FileWorkers.FileReaders;
using Domain.Convertors;
using Domain.Convertors.Convertors;
using Domain.Convertors.Convertors.Minimization.Implementation;
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

        var automata = program switch
        {
            "convert" => automataType switch
            {
                "mealy" => Convert(Mealy()),
                "moore" => Convert(Moore()),
                "grammar" => CreateNfa(),
                "nfa" => Convert(Automata()),
                _ => throw new ArgumentException($"Program({program}) don't support type: {automataType}")
            },
            "minimize" => automataType switch
            {
                "mealy" => Minimize(Mealy()),
                "moore" => Minimize(Moore()),
                "dfa" => Minimize(Automata()),
                _ => throw new ArgumentException($"Program({program}) don't support type: {automataType}")
            },
            _ => throw new ArgumentException($"Unknown program type: {program}")
        };

        PrintAll(automata);
    }

    private static Mealy Mealy()
    {
        // Mealy
        var automata = new Mealy(new CsvFileReader().CreateAutomataFromFile(_inputFilePath));
        automata
            .PrintToConsole()
            .PrintToImage();
        return automata;
    }
    
    private static Moore Moore()
    {
        // Moore
        var automata = new Moore(new CsvFileReader().CreateAutomataFromFile(_inputFilePath)); //Automata creation
        automata
            .PrintToConsole() //Print to console as table  <- Moore
            .PrintToImage(); //Create .png file with graph  <- Moore
        return automata;
    }

    private static Automata Automata()
    {
        var automata = new CsvFileReader().CreateAutomataFromFile(_inputFilePath);
        automata
            .PrintToConsole()
            .PrintToImage();
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

    private static Automata Convert(Automata automata)
    {
        return automata.Convert(new NfaToDfaConvertor());
    }

    private static Automata CreateNfa()
    {
        return GrammarToNfaConvertor.Convert(new TxtFileReader().ReadTxtFile(_inputFilePath));
    }

    private static void PrintAll(Automata automata)
    {
        automata
            .PrintToConsole()
            .PrintToFile()
            .PrintToImage();
    }

    private static Automata Minimize(Automata automata)
    {
        return automata.Minimize(new Minimizer());
    }
}