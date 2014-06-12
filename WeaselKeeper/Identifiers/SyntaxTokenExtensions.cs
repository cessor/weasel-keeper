using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace WeaselKeeper.Identifiers
{
    public static class SyntaxTokenExtensions
    {
        public static SyntaxToken RenameWith(this SyntaxToken token, string newName)
        {
            return SyntaxFactory.Identifier(token.LeadingTrivia, newName, token.TrailingTrivia);
        }

        public static string IdentifierName(this SyntaxToken token)
        {
            return token.ValueText.Trim();
        }
    }
}