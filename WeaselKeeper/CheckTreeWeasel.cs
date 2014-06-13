using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace WeaselKeeper
{
    /// <summary>
    /// Check that the renaming did not break anything.
    /// </summary>
    internal class SanityCheck
    {
        public ImmutableArray<Diagnostic> Diagnostics { get; private set; }

        // Building a Syntax Tree from the root up!
        // http://jacobcarpenter.wordpress.com/2011/10/20/hello-roslyn/
        // http://stackoverflow.com/questions/11351977/building-a-syntaxtree-from-the-ground-up
        // http://blogs.msdn.com/b/kirillosenkov/archive/2012/07/22/roslyn-code-quoter-tool-generating-syntax-tree-api-calls-for-any-c-program.aspx
        public void CheckTree(SyntaxNode node, Snippet snippet)
        {
            string methodName = snippet.MethodName;
            var @class = WrapMethodInClassDeclaration(node, methodName);
            string assemblyName = methodName + "-assembly.dll";

            var compilation = 
                CSharpCompilation
                    .Create(assemblyName, 
                        Tree(@class),
                        References(), 
                        ToDll());
            Diagnose(compilation);
        }

        private void Diagnose(Compilation compilation)
        {
            Diagnostics = compilation.GetDiagnostics();
            if (CompilationHadErrors())
            {
                throw new InvalidCompilationException(Diagnostics);
            }
        }

        private bool CompilationHadErrors()
        {
            return Errors().Any();
        }

        public IEnumerable<Diagnostic> Errors()
        {
            return Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error);
        }

        public IEnumerable<Diagnostic> Warnings()
        {
            return Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Warning);
        }

        private static IEnumerable<SyntaxTree> Tree(CompilationUnitSyntax unit)
        {
            return new[] {unit.SyntaxTree};
        }

        private static CSharpCompilationOptions ToDll()
        {
            return new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);
        }

        private static IEnumerable<MetadataFileReference> References()
        {
            return new[]
            {
                new MetadataFileReference(typeof (object).Assembly.Location)
                // new MetadataFileReference(typeof (IEnumerable).Assembly.Location),
                // new MetadataFileReference(typeof (DateTime).Assembly.Location),
            };
        }
        
        /// <summary>
        /// Wrap method class decl; Otherwise this produces a specific compiletime error, which says
        /// that a namespace can not contain functions
        /// </summary>
        /// <param name="method"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private static CompilationUnitSyntax WrapMethodInClassDeclaration(SyntaxNode method, string name)
        {
            return
                SyntaxFactory
                    .CompilationUnit()
                    .AddUsings(Usings())
                    .AddMembers(WrapperClass(method, name));
        }

        private static UsingDirectiveSyntax[] Usings()
        {
            return new[]
            {
                SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System"))
            };
        }

        private static ClassDeclarationSyntax WrapperClass(SyntaxNode node, string methodName)
        {
            var actualMethod = (MethodDeclarationSyntax) node.DescendantNodes().First();
            var className = methodName + "Wrapper";
            return 
                SyntaxFactory
                    .ClassDeclaration(className)
                    .AddMembers(actualMethod);
        }
    }

    [Serializable]
    public class InvalidCompilationException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public InvalidCompilationException()
        {
        }

        public InvalidCompilationException(ImmutableArray<Diagnostic> diags)
        {
            var messages = new StringBuilder("There were errors when compiling the source code");
            foreach (var diagnostic in diags)
            {
                messages.AppendLine("\t").Append(diagnostic.GetMessage());
            }
            var message = messages.ToString();
        }

        public InvalidCompilationException(string message, Exception inner) : base(message, inner)
        {
        }

        protected InvalidCompilationException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}