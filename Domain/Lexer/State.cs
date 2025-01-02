namespace Domain.Lexer;

public enum State
{
    AnalysingCode,
    AnalysingLineComment,
    AnalysingMultilineComment,
    AnalysingString,
    AnalysingChar,
    LookingForFinalCommentLiteral,
    LookingForFinalComparisonLiteral
}