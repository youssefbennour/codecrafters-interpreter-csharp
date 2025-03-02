using System.Runtime.InteropServices.JavaScript;
using System.Security;
using codecrafters;

namespace Sharlox;

internal class Tokenizer
{
    public Tokenizer(string input)
    {
        this.Input = input.Trim();
    }
    
    public string Input { get; private init; }
    private List<Token> tokens = [];
    public bool HasFailed { get; private set; }

    private int currentIndex = 0;
    private int currentLine = 1;
    
    public IReadOnlyCollection<Token> Tokens => tokens;

    public void DisplayTokens()
    {
        if (Tokens.Count == 0)
        {
            Tokenize();
        }
        
        foreach (var token in tokens)
        {
            Console.WriteLine(token.ToString());
        }
    }
    
    public void Tokenize()
    {
        this.tokens = [];
        if (string.IsNullOrWhiteSpace(Input))
        {
            this.tokens.Add(new Token(TokenType.EOF, string.Empty, null));
            return;
        }

        currentIndex = 0;
        while (currentIndex < Input.Length)
        {
            var rawToken = Input[currentIndex];
            if (rawToken == '\n')
            {
                currentLine++;
                currentIndex++;
                continue;
            }
                
            if (char.IsWhiteSpace(Input[currentIndex]))
            {
                currentIndex++;
                continue;
            }

            if (rawToken == '/' && currentIndex + 1 < Input.Length && Input[currentIndex + 1] == '/')
            {
                currentIndex = Input.IndexOf('\n', currentIndex + 1);
                if (currentIndex < 0)
                {
                    currentIndex = Input.Length;
                    break;
                }

                continue;
            }

            if (char.IsDigit(rawToken))
            {
                string token = rawToken.ToString();
                currentIndex++;
                int floatingPointsNumber = 0;
                while (currentIndex < Input.Length && (char.IsDigit(Input[currentIndex]) || Input[currentIndex] == '.'))
                {
                    if (Input[currentIndex] == '.')
                    {
                        floatingPointsNumber++;
                    } 
                    
                    token += Input[currentIndex];
                    currentIndex++;
                }

                string lexeme = token;
                if (floatingPointsNumber == 0)
                {
                    token += ".0";
                }
                else
                {
                    token = token.TrimEnd('0');
                    if (token[^1] == '.')
                    {
                        token += '0';
                    }
                }
                
                tokens.Add(new Token(TokenType.NUMBER, lexeme, token));
                continue;
            }
            

            if (GetReservedToken() is { } reservedToken)
            {
               tokens.Add(reservedToken);
               continue;
            }
            
            switch (rawToken)
            {
                case '=':
                    if (currentIndex+1 < Input.Length && Input[currentIndex + 1] == '=')
                    {
                        currentIndex++;
                        tokens.Add(new Token(TokenType.EQUAL_EQUAL, $"{rawToken}{rawToken}", null));
                    }
                    else
                    {
                        tokens.Add(new Token(TokenType.EQUAL, rawToken, null));
                    }
                    break;
                case '!':
                    if (currentIndex+1 < Input.Length && Input[currentIndex + 1] == '=')
                    {
                        currentIndex++;
                        tokens.Add(new Token(TokenType.BANG_EQUAL, $"{rawToken}=", null));
                    }
                    else
                    {
                        tokens.Add(new Token(TokenType.BANG, rawToken, null));
                    }
                    break;
                case '>':
                    if (currentIndex+1 < Input.Length && Input[currentIndex + 1] == '=')
                    {
                        currentIndex++;
                        tokens.Add(new Token(TokenType.GREATER_EQUAL, $"{rawToken}=", null));
                    }
                    else
                    {
                        tokens.Add(new Token(TokenType.GREATER, rawToken, null));
                    }
                    break;
                
                case '<':
                    if (currentIndex+1 < Input.Length && Input[currentIndex + 1] == '=')
                    {
                        currentIndex++;
                        tokens.Add(new Token(TokenType.LESS_EQUAL, $"{rawToken}=", null));
                    }
                    else
                    {
                        tokens.Add(new Token(TokenType.LESS, rawToken, null));
                    }
                    break;
                    
                case '/':
                    tokens.Add(new Token(TokenType.SLASH, rawToken, null));
                    break;
                case '+':
                    tokens.Add(new Token(TokenType.PLUS, rawToken, null));
                    break;
                case '*':
                    tokens.Add(new Token(TokenType.STAR, rawToken, null));
                    break;
                case '-':
                    tokens.Add(new Token(TokenType.MINUS, rawToken, null));
                    break;
                case ',':
                    tokens.Add(new Token(TokenType.COMMA, rawToken, null));
                    break;
                case ';':
                    tokens.Add(new Token(TokenType.SEMICOLON, rawToken, null));
                    break;
                case '.':
                    tokens.Add(new Token(TokenType.DOT, rawToken, null));
                    break;
                case '(':
                    tokens.Add(new Token(TokenType.LEFT_PAREN, rawToken, null));
                    break;
                case ')':
                    tokens.Add(new Token(TokenType.RIGHT_PAREN, rawToken, null));
                    break;
                case '{':
                    tokens.Add(new Token(TokenType.LEFT_BRACE, rawToken, null));
                    break;
                case '}':
                    tokens.Add(new Token(TokenType.RIGHT_BRACE, rawToken, null));
                    break;
                case '\"':
                    var token = string.Empty;
                    currentIndex++;
                    while (currentIndex < Input.Length && Input[currentIndex] != '\"')
                    {
                        token += Input[currentIndex];
                        currentIndex++;
                    }


                    if (currentIndex >= Input.Length)
                    {
                        PrintError($"[line {currentLine}] Error: Unterminated string.");
                    } else
                    {
                        tokens.Add(new Token(TokenType.STRING, $"\"{token}\"", token));
                        currentIndex++;
                    }
                    
                    continue;
                
                default:
                    PrintError($"[line {currentLine}] Error: Unexpected character: {rawToken}");
                    break;
            }

            currentIndex++;
        }
        
        tokens.Add(new Token(TokenType.EOF, string.Empty, null));
    }


    private Token? GetReservedToken()
    {
        var rawToken = Input[currentIndex];
        if (char.IsLetter(rawToken) || rawToken == '_')
        {
            var token = string.Empty;
            while (currentIndex < Input.Length &&
                   (char.IsLetterOrDigit(Input[currentIndex]) || Input[currentIndex] == '_'))
            {
                token += Input[currentIndex];
                currentIndex++;
            }

            switch (token)
            {
                case "and":
                    return new Token(TokenType.AND, token, null);
                case "else":
                    return new Token(TokenType.ELSE, token, null);
                case "false":
                    return new Token(TokenType.FALSE, token, null);
                case "for":
                    return new Token(TokenType.FOR, token, null);
                case "fun":
                    return new Token(TokenType.FUN, token, null);
                case "if":
                    return new Token(TokenType.IF, token, null);
                case "nil":
                    return new Token(TokenType.NIL, token, null);
                case "or":
                    return new Token(TokenType.OR, token, null);
                case "print":
                    return new Token(TokenType.PRINT, token, null);
                case "return":
                    return new Token(TokenType.RETURN, token, null);
                case "super":
                    return new Token(TokenType.SUPER, token, null);
                case "this":
                    return new Token(TokenType.THIS, token, null);
                case "true":
                    return new Token(TokenType.TRUE, token, null);
                case "var":
                    return new Token(TokenType.VAR, token, null);
                case "while":
                    return new Token(TokenType.WHILE, token, null);
                case "class":
                    return new Token(TokenType.CLASS, token, null);
                default:
                    return new Token(TokenType.IDENTIFIER, token, null);
                    break; 
            }
        }
        return null;
    }
    private void PrintError(string errorMessage)
    {
        Console.Error.WriteLine(errorMessage);
        HasFailed = true;
    }
}