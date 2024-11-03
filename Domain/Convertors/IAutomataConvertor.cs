using Domain.Models.Automatas;

namespace Domain.Convertors;

public interface IAutomataConvertor<out TOutput>
{
    TOutput Minimize(Automata automata);
}

public interface IAutomataConvertor<in TInput, out TOutput>
    where TInput : Automata
{
    TOutput Convert(TInput automata);
}