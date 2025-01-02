namespace Domain.Lexer;

public class Lexer
{
    public List<Lexeme> Lexemes { get; } = [];
    public HashSet<string> Identifiers { get; } = [];

    private readonly StreamReader _input;
    private Lexeme _lexeme = new();
    private Lexeme _previousLexeme = new();
    private string _currentLine = string.Empty;
    private int _lineNumber;
    private int _linePosition;
    private bool _isLexemeFound;
    private bool _isCurrentLexemeFound;
    private State _state = State.AnalysingCode;
    private DfaWalker? _currentDfaWalker;

    public Lexer(string input)
    {
        _input = new StreamReader(input);

        var lexeme = GetLexeme();
        while (lexeme.Name != string.Empty)
        {
            Lexemes.Add(lexeme);
            if (lexeme.Token == Token.Id)
            {
                Identifiers.Add(lexeme.Name);
            }

            lexeme = GetLexeme();
        }
    }

    private static Token GetToken(string lexeme)
    {
        if (IsGrammarWord(Constants.Keywords, lexeme)) return Token.Keyword;
        if (IsGrammarWord(Constants.Id, lexeme)) return Token.Id;
        if (IsGrammarWord(Constants.IntNumber, lexeme)) return Token.Int;
        if (IsGrammarWord(Constants.FixedPointNumber, lexeme)) return Token.FixedPoint;
        if (IsGrammarWord(Constants.FloatPointNumber, lexeme)) return Token.FloatPoint;
        if (IsGrammarWord(Constants.BinaryNumber, lexeme)) return Token.Binary;
        if (IsGrammarWord(Constants.OctalNumber, lexeme)) return Token.Octal;
        if (IsGrammarWord(Constants.HexNumber, lexeme)) return Token.Hex;
        if (IsGrammarWord(Constants.Operations, lexeme)) return Token.Operation;
        if (IsGrammarWord(Constants.Delimiters, lexeme)) return Token.Delimiter;
        if (IsGrammarWord(Constants.Comparisons, lexeme)) return Token.Comparison;
        if (IsGrammarWord(Constants.Brackets, lexeme)) return Token.Bracket;
        return Token.Error;
    }

    public static string TokenToString(Token token) => token.ToString();

    private Lexeme GetLexeme()
    {
        var resLexeme = _previousLexeme;
        if (_previousLexeme.Name != string.Empty)
        {
            _previousLexeme = new Lexeme();
            return resLexeme;
        }

        AnalyzeString();

        while (!_isLexemeFound &&
               (!_isCurrentLexemeFound || _previousLexeme.Name != string.Empty) &&
               !_input.EndOfStream)
        {
            _currentLine = _input.ReadLine()!;
            _lineNumber++;
            _linePosition = 0;
            AnalyzeString();
        }

        if (!_isLexemeFound)
        {
            if (GetToken(_lexeme.Name) == Token.Error &&
                _state is State.AnalysingLineComment or State.AnalysingMultilineComment)
            {
                FlushLexeme(Token.Eof);
            }
            else
            {
                FlushLexeme(GetToken(_lexeme.Name));
            }
        }

        if (_previousLexeme.Name == string.Empty &&
            _lexeme.Name != string.Empty &&
            _isCurrentLexemeFound)
        {
            resLexeme = _lexeme;
            _lexeme = new Lexeme();
            _isCurrentLexemeFound = false;
        }
        else
        {
            resLexeme = _previousLexeme;
            _previousLexeme = new Lexeme();
            if (_isCurrentLexemeFound)
            {
                _previousLexeme = _lexeme;
                _lexeme = new Lexeme();
                _isCurrentLexemeFound = false;
            }
        }

        _isLexemeFound = false;
        return resLexeme;
    }

    private static bool IsGrammarWord(Dfa dfa, string word) => new DfaWalker(dfa).IsGrammarWord(word);

    private static bool IsGrammarWord(List<string> words, string word) => words.Contains(word);

    private void AnalyzeString()
    {
        var charNumber = 0;
        foreach (var ch in _currentLine)
        {
            _linePosition++;
            charNumber++;
            switch (_state)
            {
                case State.AnalysingCode:
                    AnalyzeCode(ch);
                    break;
                case State.AnalysingLineComment:
                case State.AnalysingMultilineComment:
                    _lexeme.Name += ch;
                    AnalyzeLexeme(ch, Token.Comment);
                    break;
                case State.AnalysingString:
                    _lexeme.Name += ch;
                    AnalyzeLexeme(ch, Token.String);
                    break;
                case State.AnalysingChar:
                    _lexeme.Name += ch;
                    AnalyzeLexeme(ch, Token.Char);
                    break;
                case State.LookingForFinalCommentLiteral:
                    AnalyzeFinalCommentLiteral(ch);
                    break;
                case State.LookingForFinalComparisonLiteral:
                    AnalyzeFinalComparisonLiteral(ch);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (!_isLexemeFound && (!_isCurrentLexemeFound || _previousLexeme.Name != string.Empty)) continue;
            
            if (charNumber >= _currentLine.Length)
            {
                _currentLine = string.Empty;
                if (_state is State.AnalysingMultilineComment or State.AnalysingString)
                {
                    _lexeme.Name += "\n";
                }
            }
            else
            {
                _currentLine = _currentLine[charNumber..];
            }

            return;
        }

        if (_currentLine == string.Empty) return;

        switch (_state)
        {
            case State.AnalysingMultilineComment:
            case State.AnalysingString:
                _lexeme.Name += "\n";
                break;
            case State.AnalysingLineComment:
                FlushLexeme(Token.Comment);
                _state = State.AnalysingCode;
                break;
            case State.AnalysingCode:
            case State.AnalysingChar:
            case State.LookingForFinalCommentLiteral:
            case State.LookingForFinalComparisonLiteral:
            default:
                FlushLexeme(GetToken(_lexeme.Name));
                _state = State.AnalysingCode;
                break;
        }

        _currentLine = string.Empty;
    }

    private void FlushLexeme(Token token)
    {
        if (_lexeme.Name == string.Empty) return;
        
        var resToken = token switch
        {
            Token.Id => _lexeme.Name.Length > 64 ? Token.Error : token,
            Token.Int => _lexeme.Name.Length > 11 ? Token.Error : token,
            Token.FixedPoint => _lexeme.Name.Length > 18 ? Token.Error : token,
            _ => token
        };
        
        _lexeme.Token = resToken;
        _previousLexeme = _lexeme;
        _lexeme = new Lexeme();
        _isLexemeFound = true;
    }

    private void AnalyzeCode(char ch)
    {
        var currChar = ch.ToString();
        if (_lexeme.Name == string.Empty)
        {
            _lexeme.Line = _lineNumber;
            _lexeme.Position = _linePosition;
        }

        switch (ch)
        {
            case Constants.CommentStart:
                FlushLexeme(GetToken(_lexeme.Name));
                _state = State.LookingForFinalCommentLiteral;
                _lexeme.Name += ch;
                _lexeme.Line = _lineNumber;
                _lexeme.Position = _linePosition;
                break;
            case Constants.EqualityStart:
            case Constants.InequalityStart:
                FlushLexeme(GetToken(_lexeme.Name));
                _state = State.LookingForFinalComparisonLiteral;
                _lexeme.Name += ch;
                _lexeme.Line = _lineNumber;
                _lexeme.Position = _linePosition;
                break;
            case Constants.StringStart:
                FlushLexeme(GetToken(_lexeme.Name));
                _currentDfaWalker = new DfaWalker(Constants.StringFinal);
                _state = State.AnalysingString;
                _lexeme.Name += ch;
                _lexeme.Line = _lineNumber;
                _lexeme.Position = _linePosition;
                break;
            case Constants.CharStart:
                FlushLexeme(GetToken(_lexeme.Name));
                _currentDfaWalker = new DfaWalker(Constants.CharFinal);
                _state = State.AnalysingChar;
                _lexeme.Name += ch;
                _lexeme.Line = _lineNumber;
                _lexeme.Position = _linePosition;
                break;
            default:
            {
                if (IsGrammarWord(Constants.Operations, currChar) ||
                    IsGrammarWord(Constants.Delimiters, currChar) ||
                    IsGrammarWord(Constants.Comparisons, currChar) ||
                    IsGrammarWord(Constants.Brackets, currChar))
                {
                    if (currChar != "+" && currChar != "-" || !IsFloatNumberBeforeSign())
                    {
                        FlushLexeme(GetToken(_lexeme.Name));
                        _lexeme = new Lexeme
                        {
                            Name = currChar,
                            Line = _lineNumber,
                            Position = _linePosition,
                            Token = GetToken(currChar)
                        };
                        _isCurrentLexemeFound = true;
                    }
                    else
                    {
                        _lexeme.Name += currChar;
                    }
                }
                else if (IsGrammarWord(Constants.Spaces, currChar))
                {
                    FlushLexeme(GetToken(_lexeme.Name));
                }
                else
                {
                    _lexeme.Name += ch;
                }

                break;
            }
        }
    }

    private void AnalyzeLexeme(char ch, Token token)
    {
        if (_currentDfaWalker != null && !_currentDfaWalker.GoToNextState(ch))
        {
            FlushLexeme(Token.Error);
            _state = State.AnalysingCode;
            _currentDfaWalker = null;
        }
        else if (_currentDfaWalker != null && _currentDfaWalker.IsFinalState())
        {
            FlushLexeme(token);
            _state = State.AnalysingCode;
            _currentDfaWalker = null;
        }
    }

    private void AnalyzeFinalCommentLiteral(char ch)
    {
        switch (ch)
        {
            case Constants.LineCommentStart:
                _currentDfaWalker = new DfaWalker(Constants.LineComment);
                _state = State.AnalysingLineComment;
                _lexeme.Name += ch;
                break;
            case Constants.MultilineCommentStart:
                _currentDfaWalker = new DfaWalker(Constants.CommentFinal);
                _state = State.AnalysingMultilineComment;
                _lexeme.Name += ch;
                break;
            default:
            {
                FlushLexeme(GetToken(_lexeme.Name));
                var currChar = ch.ToString();
                _currentLine = currChar + _currentLine;
                _linePosition--;
                _state = State.AnalysingCode;
                break;
            }
        }
    }

    private void AnalyzeFinalComparisonLiteral(char ch)
    {
        if (ch == '=')
        {
            _lexeme.Name += ch;
            FlushLexeme(GetToken(_lexeme.Name));
        }
        else
        {
            FlushLexeme(GetToken(_lexeme.Name));
            AnalyzeCode(ch);
        }

        _state = State.AnalysingCode;
    }

    private bool IsFloatNumberBeforeSign()
    {
        if (_lexeme.Name == string.Empty ||
            (!_lexeme.Name.EndsWith('e') && !_lexeme.Name.EndsWith('E')))
        {
            return false;
        }

        _currentDfaWalker = new DfaWalker(Constants.FloatPointNumber);
        foreach (var ch in _lexeme.Name)
        {
            if (!_currentDfaWalker.GoToNextState(ch)) return false;

            if (_currentDfaWalker.IsFinalState()) return false;
        }

        return true;
    }
}