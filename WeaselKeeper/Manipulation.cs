using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.Remoting.Messaging;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace WeaselKeeper
{
    internal class Manipulation
    {
        private static readonly Random Random = new Random();
        private static readonly IdentifierSubstitutions Replacements = new IdentifierSubstitutions();
        private static readonly IdentifierBlacklist Blacklist = IdentifierBlacklist.ReadFromFile();

        public static void Print(string str)
        {
            Console.Write(str);
        }

        public static void Normal(Snippet snippet)
        {
            Print(snippet.Root.ToString().Trim());
        }

        public static void Abbrev(Snippet snippet)
        {
            Replacements.Clear();
            var newTree = ReplaceIdentifiersWith(snippet.Root, snippet.Identifiers, AbbreviateIdentifiers);

            CheckTree(newTree, snippet);

            ShowReplacementMap();

            // Print(newTree.ToString().Trim());
        }

        private static void ShowReplacementMap()
        {
            Replacements.Each((key,value)=>Console.WriteLine("{0}: {1}", key, value));
        }

        // Building a Syntax Tree from the root up!
        // http://jacobcarpenter.wordpress.com/2011/10/20/hello-roslyn/
        // http://stackoverflow.com/questions/11351977/building-a-syntaxtree-from-the-ground-up
        // http://blogs.msdn.com/b/kirillosenkov/archive/2012/07/22/roslyn-code-quoter-tool-generating-syntax-tree-api-calls-for-any-c-program.aspx
        private static void CheckTree(SyntaxNode node, Snippet snippet)
        {
            var methodName = snippet.MethodName;
            var actualMethod = (MethodDeclarationSyntax)node.DescendantNodes().First();
            var wrapperClass = SyntaxFactory.ClassDeclaration(methodName + "Wrapper").AddMembers(actualMethod);

            var usings = new[] {
                SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System")),
            };
            var unit = SyntaxFactory.CompilationUnit().AddUsings(usings).AddMembers(wrapperClass);
            
            var assemblyName = methodName + "-assembly.dll";

            var compilation = CSharpCompilation.Create(assemblyName,
                syntaxTrees: new[] {unit.SyntaxTree},
                references: new[]
                {
                    new MetadataFileReference(typeof (object).Assembly.Location),
                    // new MetadataFileReference(typeof (IEnumerable).Assembly.Location),
                    // new MetadataFileReference(typeof (DateTime).Assembly.Location),
            },
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
            );

            Console.WriteLine("Iterating Diagnostics");
            var diagnostics = compilation.GetDiagnostics();
            foreach(var d in diagnostics)
            {
                Console.WriteLine(d);
            }
        }

        public static void Single(Snippet snippet)
        {
            Replacements.Clear();
            var newTree = ReplaceIdentifiersWith(snippet.Root, snippet.Identifiers, MakeSingleCharIdentifiers);
            Print(newTree.ToString().Trim());

        }

        private static SyntaxNode ReplaceIdentifiersWith(SyntaxNode node, IEnumerable<SyntaxToken> identifiers,
            Func<SyntaxToken, SyntaxToken, SyntaxToken> replacement)
        {
            SyntaxNode newTree = node.ReplaceTokens(identifiers, replacement);
            return newTree;
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

        public static bool ShouldNotBeReplaced(string identifierName)
        {
            return Blacklist.Contains(identifierName);
        }
    }
}