using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WeaselKeeper
{
    internal class IdentifierBlacklist
    {
        private IdentifierBlacklist(){ }

        private readonly List<string> _blacklist = new List<string>();

        public static IdentifierBlacklist ReadFromFile(string filename="blacklist.txt")
        {
            var blacklist = new IdentifierBlacklist();
            var identifiers = File
                .ReadAllLines(filename)
                .Where(l=>!l.Trim().StartsWith("#"))
                .Where(l => !string.IsNullOrWhiteSpace(l))
                .Select(l => l.Trim());

            blacklist._blacklist.AddRange(identifiers);
            return blacklist;
        }

        public bool Contains(string identifier)
        {
            return _blacklist.Contains(identifier);
        }
    }
}