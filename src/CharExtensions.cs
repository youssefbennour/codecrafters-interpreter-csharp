namespace codecrafters;

public static class CharExtensions
{
   public static bool IsSharloxToken(this char c) => 
      c is '(' or ')' or ';' or '{' or '}';
}