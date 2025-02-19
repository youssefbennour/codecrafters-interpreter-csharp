namespace codecrafters;

public enum TokenType
{
    VAR,
    
    DOT, SEMICOLON, COMMA,
    
    EQUAL, EQUAL_EQUAL, BANG, BANG_EQUAL, STAR, PLUS, MINUS, SLASH,
    GREATER, GREATER_EQUAL, LESS, LESS_EQUAL,
    
    IDENTIFIER,NUMBER, STRING,
    
    LEFT_PAREN, RIGHT_PAREN, LEFT_BRACE, RIGHT_BRACE,
    
    EOF
}