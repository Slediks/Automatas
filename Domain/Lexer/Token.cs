namespace Domain.Lexer;

public enum Token
{
    Keyword,
    Id,
    Int,
    FixedPoint,
    FloatPoint,
    Binary,
    Octal,
    Hex,
    Char,
    String,
    Operation,
    Delimiter,
    Comment,
    Comparison,
    Bracket,
    Error,
    Eof
}