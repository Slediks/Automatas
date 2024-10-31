using Domain.Models.ValueObjects;

namespace Domain.Models.Automatas;

public class Mealy : Automata
{
    public Mealy(ICollection<Transition> transitions)
        : base(transitions)
    {
    }

    public Mealy(Automata automata)
        : base(automata)
    {
    }
}