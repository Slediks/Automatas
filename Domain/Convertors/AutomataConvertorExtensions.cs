using Domain.Convertors.Convertors;
using Domain.Models.Automatas;

namespace Domain.Convertors;

public static class AutomataConvertorExtensions
{
    public static TOutput Minimize<TOutput>(
        this Automata automata,
        IAutomataMinimizer<TOutput> minimizer)
    {
        return minimizer.Minimize(automata);
    }

    public static TOutput Convert<TOutput>(
        this Automata automata,
        IAutomataConvertor<TOutput> converter)
    {
        return converter.Convert(automata);
    }

    public static TOutput Convert<TInput, TOutput>(
        this TInput automata,
        IAutomataConvertor<TInput, TOutput> convertor)
    {
        return convertor.Convert(automata);
    }
}