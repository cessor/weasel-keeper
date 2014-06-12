using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace WeaselKeeper
{
    internal class CheckTreeWeasel
    {
        // Building a Syntax Tree from the root up!
        // http://jacobcarpenter.wordpress.com/2011/10/20/hello-roslyn/
        // http://stackoverflow.com/questions/11351977/building-a-syntaxtree-from-the-ground-up
        // http://blogs.msdn.com/b/kirillosenkov/archive/2012/07/22/roslyn-code-quoter-tool-generating-syntax-tree-api-calls-for-any-c-program.aspx
        private static void CheckTree(SyntaxNode node, Snippet snippet)
        {
            string methodName = snippet.MethodName;
            var actualMethod = (MethodDeclarationSyntax) node.DescendantNodes().First();
            ClassDeclarationSyntax wrapperClass =
                SyntaxFactory.ClassDeclaration(methodName + "Wrapper").AddMembers(actualMethod);

            var usings = new[]
            {
                SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System"))
            };
            CompilationUnitSyntax unit = SyntaxFactory.CompilationUnit().AddUsings(usings).AddMembers(wrapperClass);

            string assemblyName = methodName + "-assembly.dll";

            CSharpCompilation compilation = CSharpCompilation.Create(assemblyName, new[] {unit.SyntaxTree}, new[]
            {
                new MetadataFileReference(typeof (object).Assembly.Location)
                // new MetadataFileReference(typeof (IEnumerable).Assembly.Location),
                // new MetadataFileReference(typeof (DateTime).Assembly.Location),
            }, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
                );

            Console.WriteLine("Iterating Diagnostics");
            ImmutableArray<Diagnostic> diagnostics = compilation.GetDiagnostics();
            foreach (Diagnostic d in diagnostics)
            {
                Console.WriteLine(d);
            }
        }
    }
}