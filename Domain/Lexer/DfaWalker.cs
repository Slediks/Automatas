namespace Domain.Lexer;

public class DfaWalker(Dfa dfa)
{
    private int _state;

    public bool IsGrammarWord(string word)
    {
        _state = 0;
        return word.All(GoToNextState) && IsFinalState();
    }

    public bool GoToNextState(char ch)
    {
        for (var i = 0; i < dfa.TransConditions.Count; i++)
        {
            if (!dfa.TransConditions[i](ch)) continue;
            var newState = dfa.Table[_state][i];
            if (newState == -1)
            {
                break;
            }

            _state = newState;
            return true;
        }

        return false;
    }

    public bool IsFinalState()
    {
        return dfa.Terminals.Contains(_state);
    }
}