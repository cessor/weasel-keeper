using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace WeaselKeeper.Identifiers
{
    // The Replacement map is called map as in landscape.
    // Parts of it are later used to create the documentation overview
    internal class ReplacementMap
    {
        private readonly Blacklist _blacklist;
        private readonly Replace _replace;
        private readonly Dictionary<string, string> _replacements = new Dictionary<string, string>();

        public ReplacementMap(Blacklist blacklist, Replace replace)
        {
            _blacklist = blacklist;
            _replace = replace;
        }

        public Dictionary<string, string> Replacements
        {
            get { return _replacements; }
        }

        public SyntaxToken Replace(SyntaxToken token)
        {
            string identifier = token.IdentifierName();
            string replacedIdentifier = ReplaceIfNecessary(identifier);
            return token.RenameWith(replacedIdentifier);
        }

        private string ReplaceIfNecessary(string identifier)
        {
            if (MustNotBeReplaced(identifier))
            {
                return identifier;
            }
            if (WasAlreadyReplaced(identifier))
            {
                return OldReplacementFor(identifier);
            }
            return ReplaceAndRemember(identifier);
        }

        private bool MustNotBeReplaced(string identifier)
        {
            return _blacklist.Contains(identifier);
        }

        private bool WasAlreadyReplaced(string identifier)
        {
            return _replacements.ContainsKey(identifier);
        }

        private string OldReplacementFor(string identifierName)
        {
            return _replacements[identifierName];
        }

        private string ReplaceAndRemember(string identifier)
        {
            string replacement = EnsureUniqueness(identifier);
            Remember(identifier, replacement);
            return replacement;
        }

        private string EnsureUniqueness(string identifier)
        {
            int trials = 0;
            string replacement;
            do
            {
                if (trials > 10)
                {
                    throw new Exception("Can't find a unique replacement!");
                }
                replacement = _replace(identifier);
                trials++;
            } while (IsNotUnique(replacement));
            return replacement;
        }

        private bool IsNotUnique(string replacement)
        {
            return _replacements.ContainsValue(replacement);
        }

        private void Remember(string originalIdentifier, string replacement)
        {
            _replacements.Add(originalIdentifier.Trim(), replacement.Trim());
        }

        public void Each(Action<string, string> action)
        {
            foreach (var replacement in _replacements)
            {
                action(replacement.Key, replacement.Value);
            }
        }
    }
}