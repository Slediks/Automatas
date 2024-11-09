using Domain.Models.ValueObjects;

namespace Domain.Models.Automatas;

public class Mealy(Automata automata) : Automata(automata);