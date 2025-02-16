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
            }

            currentIndex++;
        }
        
        tokens.Add(new Token(TokenType.EOF, string.Empty, null));
    }

    private static Token? TokenizeSingle(string? rawToken)
    {
        if (string.IsNullOrWhiteSpace(rawToken) || rawToken.Length == 0)
        {
            return null;
        }
        
        

        if (rawToken.AsSpan().Count('\"') == 2)
        {
            return new Token(TokenType.STRING, rawToken, rawToken.Trim('\"'));
        }
        if (rawToken.All(char.IsDigit))
        {
            return new Token(TokenType.NUMBER, rawToken, rawToken);
        }
        if (!char.IsDigit(rawToken.First()) && !rawToken.Any(char.IsSymbol))
        {
            return new Token(TokenType.IDENTIFIER, rawToken, null);     
        }

        return null;
    }
}