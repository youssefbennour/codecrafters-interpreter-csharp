namespace codecrafters;

internal sealed class Token(TokenType tokenType, string? lexeme, string? literal)
{
    public TokenType Type { get; set; } = tokenType;
    public string? Lexeme { get; set; } = lexeme;
    public string? Literal { get; set; } = literal;
}