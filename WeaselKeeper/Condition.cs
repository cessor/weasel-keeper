using System;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using WeaselKeeper.Identifiers;

namespace WeaselKeeper
{
    /// <summary>
    ///     Condition is my internal mnemonic for an "Within Subjects Factor".
    ///     It is essentially an alteration of Identifier Type, or "Quality". Depending on the context.
    /// </summary>
    internal class Condition
    {
        private readonly Blacklist _blacklist;
        private readonly Random _random;
        private ReplacementMap _map;
        private CompilationUnitSyntax _replacedCode;
        
        public Condition(Random random, Blacklist blacklist)
        {
            _random = random;
            _blacklist = blacklist;
        }

        public void Normal(Snippet snippet)
        {
            ReplaceIdentifiers(snippet, new Normal().Replace);
        }

        public void Single(Snippet snippet)
        {
            ReplaceIdentifiers(snippet, new SingleCharacter(_random).Replace);
        }

        public void Abbrev(Snippet snippet)
        {
            ReplaceIdentifiers(snippet, new ThreeCharacters(_random).Replace);
        }

        private void ReplaceIdentifiers(Snippet snippet, Replace identifierReplacement)
        {
            // The identifierReplacement explains what the new identifier will be.
            // The map calls this method to create replacements, but also tracks for collisions or
            // whether or not an identifier should acutlly be replaced
            _map = new ReplacementMap(_blacklist, identifierReplacement);
            _replacedCode = snippet.RenameIdentifiers(_map.Replace);
            Console.WriteLine(_replacedCode);
        }

        public void Map(Snippet snippet)
        {
            "Map".Announce();
            _map.Each((key, value) => Console.WriteLine("{0}: {1}", key, value));
        }

        private SanityCheck _check;

        public void Sane(Snippet snippet)
        {
            _check = new SanityCheck();
            _check.CheckTree(_replacedCode, snippet);
        }

        public void Warnings(Snippet snippet)
        {
            "Warnings".Announce();
            
            foreach (var diagnostic in _check.Warnings())
            {
                Console.WriteLine(diagnostic.GetMessage());
            }
        }
    }
}