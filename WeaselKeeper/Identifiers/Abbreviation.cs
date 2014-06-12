using System;
using System.Linq;

namespace WeaselKeeper.Identifiers
{
    internal class Abbreviation
    {
        private readonly Random _random;

        public Abbreviation(Random random)
        {
            _random = random;
        }

        private char RandomCharacter(string identifier)
        {
            return identifier[_random.Next(identifier.Length)];
        }

        public string Abbreviate(string identifier, int length)
        {
            return String.Concat(Enumerable.Range(0, length).Select(_ => RandomCharacter(identifier).ToString().ToLower()));
        }
    }
}