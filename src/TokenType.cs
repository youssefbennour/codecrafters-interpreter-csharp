namespace codecrafters;

public enum TokenType
{
    VAR,
    
    DOT, SEMICOLON, COMMA,
    
    EQUAL, EQUAL_EQUAL, BANG, BANG_EQUAL, STAR, PLUS, MINUS, 
    
    IDENTIFIER,NUMBER, STRING,
    
    LEFT_PAREN, RIGHT_PAREN, LEFT_BRACE, RIGHT_BRACE,
    
    EOF
}