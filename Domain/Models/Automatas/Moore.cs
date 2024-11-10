using Domain.Models.ValueObjects;

namespace Domain.Models.Automatas;

public class Moore : Automata
{
    public Moore(
        Dictionary<string, string> overrides,
        IEnumerable<State> states,
        ICollection<Transition> transitions)
        : base(overrides, states, transitions)
    {
    }

    public Moore(Automata automata)
        : base(automata)
    {
    }
}