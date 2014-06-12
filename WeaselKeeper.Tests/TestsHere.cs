using System.Linq;
using NUnit.Framework;
using WeaselKeeper.Identifiers;

namespace WeaselKeeper.Tests
{
    /// <summary>
    ///     Place use cases here. If something goes wrong, place it here.
    /// </summary>
    [TestFixture]
    public class TestsHere
    {
        [SetUp]
        public void Init()
        {
            _blacklist = new Blacklist();
            _reverse = i => string.Concat(i.Reverse());
            _map = new ReplacementMap(_blacklist, _reverse);
        }

        private Blacklist _blacklist;
        private Replace _reverse;
        private ReplacementMap _map;

        public string Go(string code)
        {
            return Snippet.Parse(code).RenameIdentifiers(_map.Replace).ToString();
        }

        [Test]
        public void ShouldNotRenameBlacklistedIdentifiers()
        {
            _blacklist.BannedWords.Add("Main");
            const string source = "public void Main(int hello){ int hello = hello; }";
            string transformed = Go(source);
            string target = "public void Main(int olleh){ int olleh = olleh; }";
            Assert.That(transformed, Is.EqualTo(target));
        }

        [Test]
        public void ShouldRenameAllIdentifiers()
        {
            string source = "public void Main(int hello){ int hello = hello; }";
            string transformed = Go(source);
            string target = "public void niaM(int olleh){ int olleh = olleh; }";
            Assert.That(transformed, Is.EqualTo(target));
        }
    }
}