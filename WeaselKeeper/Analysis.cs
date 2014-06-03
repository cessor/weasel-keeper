using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace WeaselKeeper
{
    internal class Analysis
    {
        public static void MethodName(Snippet snippet)
        {
            Console.WriteLine("Method Name: {0}", snippet.MethodName);
        }

        public static void ListIdentifiers(Snippet snippet)
        {
            ListTokens(snippet.Identifiers);
        }

        public static void CountTokens(Snippet snippet)
        {
            Console.WriteLine("Tokens: {0}", snippet.Tokens.Count());
        }

        public static void CountIdenfifiers(Snippet snippet)
        {
            Console.WriteLine("Identifiers: {0}", snippet.Identifiers.Count());
        }

        public static void CountLines(Snippet snippet)
        {
            Console.WriteLine("LOC: {0}", snippet.LinesOfCodeCount);
        }

        public static void ListTokens(Snippet snippet)
        {
            ListTokens(snippet.Tokens);
        }

        private static void ListTokens(IEnumerable<SyntaxToken> tokens)
        {
            int index = 1;
            foreach (SyntaxToken syntaxToken in tokens)
            {
                string content = syntaxToken.ToString();
                string token = syntaxToken.CSharpKind().ToString();
                token = RemoveTokenSuffix(token);
                Console.WriteLine("{0}: {1} ({2})", index, content, token);
                index++;
            }
        }

        private static string RemoveTokenSuffix(string token)
        {
            if (token.Contains("Token"))
            {
                token = token.Remove(token.IndexOf("Token", StringComparison.Ordinal), "Token".Length);
            }
            return token;
        }
    }
}