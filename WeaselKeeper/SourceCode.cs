using System;
using System.Text;
using WeaselKeeper;

namespace WeaselKeeper
{
    internal class SourceCode
    {
        private const string Code = @"public int CustomerPoints(DateTime due, DateTime returned, int customerPoints)
{
    const int fine = 10;
    const int reward = 5;
    if (due < returned)
    {
        int points = fine * (due - returned).Days;
        customerPoints += points;
    }
    else if (due > returned)
    {
        int points = fine * (returned - due).Days;
        customerPoints -= points;
    }
    if (customerPoints < 0)
    {
        return 0;
    }
    return customerPoints;
}"; 

        public static Snippet TestCode
        {
            get { return Snippet.Parse(Code); }
        }

        public static Snippet FromStdin()
        {
            string code = ReadCodeFromStdIn();
            return Snippet.Parse(code);
        }

        private static string ReadCodeFromStdIn()
        {
            var code = new StringBuilder();
            string currentLine;
            while ((currentLine = Console.ReadLine()) != null)
            {
                code.AppendLine(currentLine);
            }
            return code.ToString();
        }
    }
}