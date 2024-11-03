using Domain.Convertors.Convertors;
using Domain.Models.Automatas;

namespace Domain.Convertors;

public static class AutomataConvertorExtensions
{
    public static TOutput Minimize<TOutput>(
        this Automata automata,
        IAutomataConvertor<TOutput> minimizer)
    {
        return minimizer.Minimize(automata);
    }

    public static TOutput Convert<TInput, TOutput>(
        this TInput automata,
        IAutomataConvertor<TInput, TOutput> convertor)
        where TInput : Automata
    {
        return convertor.Convert(automata);
    }
}