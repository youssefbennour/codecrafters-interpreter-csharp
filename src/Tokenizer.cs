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
            
            if (char.IsDigit(rawToken))
            {
                var token = string.Empty;
                while (char.IsDigit(Input[currentIndex]))
                {
                    token += Input[currentIndex];
                    currentIndex++;
                }
                tokens.Add(new Token(TokenType.NUMBER, token, token));
                continue;
            }

            if (char.IsLetter(rawToken))
            {
                var token = string.Empty;
                while (currentIndex < Input.Length &&
                       (char.IsLetterOrDigit(Input[currentIndex]) || Input[currentIndex] == '_'))
                {
                    token += Input[currentIndex];
                    currentIndex++;
                }

                tokens.Add(token == "var"
                    ? new Token(TokenType.VAR, "var", null)
                    : new Token(TokenType.IDENTIFIER, token, null));
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

    private void PrintError(string errorMessage)
    {
        Console.Error.WriteLine(errorMessage);
        HasFailed = true;
    }
}