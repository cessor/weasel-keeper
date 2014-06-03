using System.Collections.Generic;
using System.Linq;
using WeaselKeeper;

namespace WeaselKeeper
{
    internal class Program
    {
        // file:///C:/Users/Johannes/Downloads/Getting%20Started%20-%20Syntax%20Transformation%20(CSharp).pdf
        // http://www.codeplex.com/Download?ProjectName=roslyn&DownloadId=822458
        // http://blogs.msdn.com/b/csharpfaq/archive/2011/11/03/using-the-roslyn-syntax-api.aspx

        private static Options ConfigureOptions()
        {
            var options = new Options();
            options
                .Add("--loc", Analysis.CountLines)
                .Add("--name", Analysis.MethodName)
                .Add("--count-tokens", Analysis.CountTokens)
                .Add("--count-identifiers", Analysis.CountIdenfifiers)
                .Add("--list-tokens", Analysis.ListTokens)
                .Add("--list-identifiers", Analysis.ListIdentifiers)
                .Add("--normal", Manipulation.Normal)
                .Add("--single", Manipulation.Single)
                .Add("--abbrev", Manipulation.Abbrev)
                .Add("--help", t => Help.Print(options));
            return options;
        }

        private static void Main(string[] commandLine)
        {
#if DEBUG
            Snippet snippet = SourceCode.TestCode;
#else 
                var snippet = SourceCode.FromStdin();
            #endif

            Options options = ConfigureOptions();
            List<Call> actions = options.Parse(commandLine).ToList();
            if (NoActionsSpecified(actions))
            {
                Help.Print(options);
                return;
            }
            InvokeAll(actions, snippet);
        }

        private static bool NoActionsSpecified(IEnumerable<Call> actions)
        {
            return !actions.Any();
        }

        private static void InvokeAll(IEnumerable<Call> actions, Snippet snippet)
        {
            foreach (Call action in actions)
            {
                action.Invoke(snippet);
            }
        }
    }
}