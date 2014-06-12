using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WeaselKeeper.Identifiers
{
    internal class Blacklist
    {
        private Blacklist(){ }

        private readonly List<string> _bannedWords = new List<string>();

        public static Blacklist ReadFromFile(string filename="blacklist.txt")
        {
            var blacklist = new Blacklist();
            var identifiers = File
                .ReadAllLines(filename)
                .Where(l=>!l.Trim().StartsWith("#"))
                .Where(l => !string.IsNullOrWhiteSpace(l))
                .Select(l => l.Trim());

            blacklist._bannedWords.AddRange(identifiers);
            return blacklist;
        }

        public bool Contains(string identifier)
        {
            return _bannedWords.Contains(identifier);
        }
    }
}