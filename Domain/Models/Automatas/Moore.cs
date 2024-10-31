using Domain.Models.ValueObjects;

namespace Domain.Models.Automatas;

public class Moore : Automata
{
    public Moore(
        Dictionary<State, (State, string)> overrides,
        ICollection<Transition> transitions)
        : base(overrides, transitions)
    {
    }

    public Moore(Automata automata)
        : base(automata)
    {
    }
}