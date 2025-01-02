namespace Domain.Lexer;

public struct Lexeme()
{
    public string Name = string.Empty;
    public Token Token;
    public int Line = 0;
    public int Position = 0;
}