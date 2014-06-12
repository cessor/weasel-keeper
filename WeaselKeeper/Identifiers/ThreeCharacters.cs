using System;

namespace WeaselKeeper.Identifiers
{
    internal class ThreeCharacters : Abbreviation, IReplace
    {
        public const int AbbreviationLength = 3;

        public ThreeCharacters(Random random) : base(random)
        {
        }

        public string Replace(string identifier)
        {
            return Abbreviate(identifier, AbbreviationLength);
        }
    }
}