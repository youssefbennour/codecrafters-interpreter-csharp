using codecrafters;

namespace Sharlox;

internal static class Tokenizer
{
    public static List<Token> Tokenize(string fileContents)
    { 
        var rawTokens = fileContents.Split(" ")
            .ToList();
        
        List<Token> tokens = [];
        for(var index = 0; index < rawTokens.Count; index++)
        {
            var rawToken = rawTokens[index].Trim();
            
            while (rawToken[^1].IsSharloxToken() && rawToken.Length > 1)
            {
                rawTokens.Add(rawToken[^1].ToString());
                rawToken = rawToken[..^1];
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

    private static Token? TokenizeSingle(string rawToken)
    {
        if (rawToken.Length == 0 || string.IsNullOrWhiteSpace(rawToken))
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