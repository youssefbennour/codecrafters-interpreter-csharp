namespace codecrafters;

internal sealed class Token(TokenType tokenType, string? lexeme, string? literal)
{

    public Token(TokenType tokenType, char? lexeme, string? literal) : this(tokenType, lexeme.ToString(), literal){}
    
    public TokenType Type { get; set; } = tokenType;
    public string? Lexeme { get; set; } = lexeme;
    public string? Literal { get; set; } = literal;
    
    public override string ToString() => $"{Type} {Lexeme} {Literal ?? "null"}";
}