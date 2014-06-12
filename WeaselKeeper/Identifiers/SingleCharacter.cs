using System;

namespace WeaselKeeper.Identifiers
{
    internal class SingleCharacter : Abbreviation, IReplace
    {
        public const int AbbreviationLength = 1;

        public SingleCharacter(Random random) : base(random)
        {
        }

        public string Replace(string identifier)
        {
            return Abbreviate(identifier, AbbreviationLength);
        }
    }
}