using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace WeaselKeeper
{
    internal class Manipulation
    {
        private static readonly Random Random = new Random();
        private static readonly IdentifierSubstitutions Replacements = new IdentifierSubstitutions();

        public static void Normal(Snippet snippet)
        {
            Console.WriteLine(snippet.Root.ToString().Trim());
        }

        public static void Abbrev(Snippet snippet)
        {
            Replacements.Clear();
            ReplaceIdentifiersWith(snippet.Root, snippet.Identifiers, AbbreviateIdentifiers);
        }

        public static void Single(Snippet snippet)
        {
            Replacements.Clear();
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
            var identifier = a.ValueText.Trim();
            var abbreviatedIdentifier = IfNecessary(Abbreviate, identifier);
            return SyntaxFactory.Identifier(a.LeadingTrivia, abbreviatedIdentifier, a.TrailingTrivia);
        }
        
        private static SyntaxToken MakeSingleCharIdentifiers(SyntaxToken a, SyntaxToken b)
        {
            var identifier = a.ValueText;
            var singleCharacterIdentifier = IfNecessary(CutToSingleCharacter, identifier);
            return SyntaxFactory.Identifier(a.LeadingTrivia, singleCharacterIdentifier, a.TrailingTrivia);
        }

        private static string IfNecessary(Func<string, string> replace, string identifier)
        {
            if (ShouldNotBeReplaced(identifier))
            {
                return identifier;
            }
            if (Replacements.WasAlreadyReplaced(identifier))
            {
                return Replacements.For(identifier);
            }
            var replacement = Replacements.EnsureUniquenes(replace, identifier);
            Replacements.Remember(identifier, replacement);
            return replacement;
        }

        private static string CutToSingleCharacter(string identifier)
        {
            return RandomCharacter(identifier).ToString();
        }

        // Read in blacklist from the outside so that you don't have to recompile shit  
        private static readonly IEnumerable<string> Blacklist = new[] { "List", "var", "Add" };

        private static bool ShouldNotBeReplaced(string identifierName)
        {
            return Blacklist.Contains(identifierName);
        }

        private static string Abbreviate(string identifier)
        {
            // return ''.join(random.choice(identifier) for _ in range(3))... Sigh...
            const int length = 3;
            string abbreviation = String.Concat(Enumerable.Range(0, length).Select(_ => RandomCharacter(identifier).ToString().ToLower()));
            return abbreviation;
        }

        private static char RandomCharacter(string identifier)
        {
            return identifier[Random.Next(identifier.Length)];
        }
    }
}