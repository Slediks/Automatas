using Domain.Models.Automatas;

namespace Domain.Convertors;

public interface IAutomataMinimizer<out TOutput>
{
    TOutput Minimize(Automata automata);
}

public interface IAutomataConvertor<out TOutput>
{
    TOutput Convert(Automata automata);
}

public interface IAutomataConvertor<in TInput, out TOutput>
{
    TOutput Convert(TInput automata);
}