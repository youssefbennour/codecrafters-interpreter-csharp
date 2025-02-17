using System.Runtime.InteropServices.JavaScript;
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

        int currentIndex = 0;
        while (currentIndex < Input.Length)
        {
            var rawToken = Input[currentIndex];
            if (char.IsWhiteSpace(Input[currentIndex]))
            {
                currentIndex++;
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
                    if (Input[currentIndex + 1] == '=')
                    {
                        tokens.Add(new Token(TokenType.EQUAL_EQUAL, $"{rawToken}{rawToken}", null));
                    }
                    tokens.Add(new Token(TokenType.EQUAL, rawToken, null));
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
                    while (Input[currentIndex] != '\"' && currentIndex < Input.Length)
                    {
                        token += Input[currentIndex];
                        currentIndex++;
                    }

                    tokens.Add(new Token(TokenType.STRING, $"\"{token}\"", token));
                    continue;
                default:
                    HasFailed = true;
                    Console.Error.WriteLine($"[line 1] Error: Unexpected character: {rawToken}");
                    break;
            }

            currentIndex++;
        }
        
        tokens.Add(new Token(TokenType.EOF, string.Empty, null));
    }
}