namespace WeaselKeeper.Identifiers
{
    internal class Normal : IReplace
    {
        /// <summary>
        ///  Doesn't rename. Polymorphism, bitches.
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        public string Replace(string identifier)
        {
            return identifier;
        }
    }
}