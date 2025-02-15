using codecrafters;

namespace Sharlox;

internal static class Tokenizer
{
    public static List<Token> Tokenize(string fileContents)
    {
        if (string.IsNullOrEmpty(fileContents))
        {
            return [new Token(TokenType.EOF, string.Empty, null)];
        }
        
        Queue<string?> tokensQueue = new(fileContents.Split(" "));
        List<Token> tokens = [];
        while(tokensQueue.TryDequeue(out string? rawToken))
        {
            rawToken = rawToken?.Trim();
            
            Stack<char> tokenStack = new Stack<char>();
            
            while (rawToken != null && rawToken[^1].IsSharloxToken() && rawToken.Length > 1)
            {
                tokenStack.Push(rawToken[^1]);
                rawToken = rawToken[..^1];
            }

            while(tokenStack.TryPop(out var t))
            {
                tokensQueue.Enqueue(t.ToString());     
            }
            
            var token = TokenizeSingle(rawToken);
            if (token is null)
            {
                continue;
            }
            tokens.Add(token);
        }
        
        tokens.Add(new Token(TokenType.EOF, string.Empty, null));
        return tokens;
    }

    private static Token? TokenizeSingle(string? rawToken)
    {
        if (string.IsNullOrWhiteSpace(rawToken) || rawToken.Length == 0)
        {
            return null;
        }
        
        switch (rawToken)
        {
            case "var":
                 return new Token(TokenType.VAR, rawToken, null);
            case "=":
                return new Token(TokenType.EQUAL, rawToken, null);
            case ";":
                return new Token(TokenType.SEMICOLON, rawToken, null);
            case "(":
                return new Token(TokenType.LEFT_PAREN, rawToken, null);
            case ")":
                return new Token(TokenType.RIGHT_PAREN, rawToken, null);
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