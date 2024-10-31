using Domain.Models.Automatas;

namespace Domain.Convertors.Convertors;

public class MooreMealyConvertor : IAutomataConvertor<Moore, Mealy>
{
    public Mealy Convert(Moore automata)
    {
        return new Mealy(automata);
    }
}