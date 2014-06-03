using System;
using System.Text.RegularExpressions;

namespace WeaselKeeper
{
    internal class Help
    {
        public static void Print(Options options)
        {
            Console.WriteLine("Pass me the source code of a C# function via stdin!");
            foreach (var option in options)
            {
                string optionName = option.Item1;
                string methodName = option.Item2;
                string sentence = MakeSentenceFrom(methodName);
                Console.WriteLine("{0}: {1}", optionName, sentence);
            }
        }

        private static string MakeSentenceFrom(string methodName)
        {
            return Regex.Replace(methodName, "(\\B[A-Z])", " $1");
        }
    }
}