using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace WeaselKeeper
{
    internal class Manipulation
    {
        private static readonly Random Random = new Random();

        public static void Normal(Snippet snippet)
        {
            Console.WriteLine(snippet.Root.ToString().Trim());
        }

        public static void Abbrev(Snippet snippet)
        {
            ReplaceIdentifiersWith(snippet.Root, snippet.Identifiers, AbbreviateIdentifiers);
        }

        public static void Single(Snippet snippet)
        {
            ReplaceIdentifiersWith(snippet.Root, snippet.Identifiers, MakeSingleCharIdentifiers);
        }

        private static void ReplaceIdentifiersWith(SyntaxNode tree, IEnumerable<SyntaxToken> identifiers,
            Func<SyntaxToken, SyntaxToken, SyntaxToken> replacement)
        {
            SyntaxNode newTree = tree.ReplaceTokens(identifiers, replacement);
            Console.WriteLine(newTree.ToString().Trim());
        }

        private static SyntaxToken AbbreviateIdentifiers(SyntaxToken a, SyntaxToken b)
        {
            string identifierName = a.ValueText;
            const int length = 3;
            string mangeledName =
                String.Concat(
                    Enumerable.Range(0, length).Select(_ => RandomCharacter(identifierName).ToString().ToLower()));
            return SyntaxFactory.Identifier(a.LeadingTrivia, mangeledName, a.TrailingTrivia);
        }

        private static char RandomCharacter(string identifierName)
        {
            return identifierName[Random.Next(identifierName.Length)];
        }

        private static SyntaxToken MakeSingleCharIdentifiers(SyntaxToken a, SyntaxToken b)
        {
            string mangeledName = a.ValueText[0].ToString();
            return SyntaxFactory.Identifier(a.LeadingTrivia, mangeledName, a.TrailingTrivia);
        }
    }
}