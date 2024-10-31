﻿using CSVFilesWorkers.FileReaders;
using Domain.Convertors;
using Domain.Convertors.Convertors;
using Domain.Models.Automatas;
using Visualizations;

namespace MealyMoore;

public class Program
{
    private const string InputMealyFilePath = "input_mealy.csv";
    private const string InputMooreFilePath = "input_moore.csv";

    public static void Main()
    {
        Mealy();
        
        // Moore();
    }

    private static void Mealy()
    {
        // Mealy
        var automata = new Mealy(new AutomataFileReader().CreateAutomataFromFile(InputMealyFilePath));
        automata
            .PrintToConsole()
            .PrintToImage()
            .Convert(new MealyMooreConvertor())
            .PrintToConsole()
            .PrintToFile()
            .PrintToImage();
    }

    private static void Moore()
    {
        // Moore
        var automata = new Moore(new AutomataFileReader().CreateAutomataFromFile(InputMooreFilePath));
        automata
            .PrintToConsole()
            .PrintToImage()
            .Convert(new MooreMealyConvertor())
            .PrintToConsole()
            .PrintToFile()
            .PrintToImage();
    }
}