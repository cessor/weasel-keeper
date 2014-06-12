using System;

namespace WeaselKeeper.Identifiers
{
    internal class SingleCharacter : Abbreviation, IReplace
    {
        public SingleCharacter(Random random) : base(random) { }

        public string Replace(string identifier)
        {
            return Abbreviate(identifier, AbbreviationLength);
        }

        public const int AbbreviationLength = 1;
    }
}