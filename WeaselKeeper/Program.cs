using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using WeaselKeeper.Identifiers;

namespace WeaselKeeper
{
    public static class AnnounceStringExtension
    {
        public static void Announce(this string message)
        {
            message = string.Format("{0}:", message);
            var line = string.Concat(message.Select(c => "-").ToArray());
            Console.WriteLine(message);
            Console.WriteLine(line);
        }
    }

    internal class Program
    {
        // file:///C:/Users/Johannes/Downloads/Getting%20Started%20-%20Syntax%20Transformation%20(CSharp).pdf
        // http://www.codeplex.com/Download?ProjectName=roslyn&DownloadId=822458
        // http://blogs.msdn.com/b/csharpfaq/archive/2011/11/03/using-the-roslyn-syntax-api.aspx
        public static Options ConfigureOptions()
        {
            var condition = new Condition(new Random(), Blacklist.ReadFromFile());
            var options = new Options();
            options
                .Add("--loc", Report.CountLines)
                .Add("--name", Report.MethodName)
                .Add("--count-tokens", Report.CountTokens)
                .Add("--count-identifiers", Report.CountIdenfifiers)
                .Add("--list-tokens", Report.ListTokens)
                .Add("--list-identifiers", Report.ListIdentifiers)
                .Add("--normal", condition.Normal)
                .Add("--single", condition.Single)
                .Add("--abbrev", condition.Abbrev)
                .Add("--map", condition.Map)
                .Add("--sane", condition.Sane)
                .Add("--show-warnings", condition.Warnings)
                .Add("--help", t => Help.Print(options));
            return options;
        }

        private static void Main(string[] commandLine)
        {
#if DEBUG
            commandLine = new[] {"--abbrev", "--map", "--sane", "--show-warnings"};
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