using System;
using System.Collections.Generic;

namespace WeaselKeeper
{
    internal class IdentifierSubstitutions
    {
        private readonly Dictionary<string, string> _replacements = new Dictionary<string, string>();

        public void Remember(string identifier, string replacement)
        {
            _replacements.Add(identifier, replacement);
        }

        public bool WasAlreadyReplaced(string identifier)
        {
            return _replacements.ContainsKey(identifier);
        }

        public string For(string identifier)
        {
            return _replacements[identifier];
        }

        public void Clear()
        {
            _replacements.Clear();
        }

        private bool IsNotUnique(string ident)
        {
            return _replacements.ContainsValue(ident);
        }

        public string EnsureUniquenes(Func<string, string> replace, string identifier)
        {
            var trials = 0;
            string replacement;
            do
            {
                if (trials > 10)
                {
                    throw new Exception("Can't find a unique replacement!");
                }
                replacement = replace(identifier);
                trials++;
            }
            while (IsNotUnique(replacement));
            return replacement;
        }
    }
}