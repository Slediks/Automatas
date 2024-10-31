using Domain.Models.Automatas;

namespace Domain.Convertors;

public static class AutomataConvertorExtensions
{
    public static TOutput Convert<TOutput>(
        this Automata automata,
        IAutomataConvertor<TOutput> convertor)
    {
        return convertor.Convert(automata);
    }

    public static TOutput Convert<TInput, TOutput>(
        this TInput automata,
        IAutomataConvertor<TInput, TOutput> convertor)
        where TInput : Automata
    {
        return convertor.Convert(automata);
    }
}