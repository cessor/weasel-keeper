using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace WeaselKeeper.Tests
{
    [TestFixture]
    class OptionParserTests
    {
        [Test]
        public void RequirementsNotSpecified()
        {
            var requirements = new[] { "a", "b" };
            var specified = new[] {  "a",  "c" };

            Console.WriteLine(string.Concat(requirements.Except(specified)));
        }

        [Test]
        public void MustAddOptionFirst()
        {
            var o = new Options();
            Assert.Throws<KeyNotFoundException>(() => o.Option("a"));
        }

        [Test]
        public void AddedOptionFirst()
        {
            var o = new Options();
            o.Add("a", (s) => { });
            Assert.DoesNotThrow(() => o.Option("a"));
        }

        [Test]
        public void MustConfigureRequirementsFirst()
        {
            var o = new Options();
            o.Add("a", (s) => { });
            Assert.Throws<KeyNotFoundException>(() => o.Option("a").Requires("b"));
        }

        [Test]
        public void AddedOptionsAndRequirements()
        {
            var o = new Options();
            o.Add("a", (s) => { });
            o.Add("b", (s) => { });
            Assert.DoesNotThrow(() => o.Option("a").Requires("b"));
        }

        [Test]
        public void FailsWhenThereAreOptionsMissing()
        {
            var o = new Options();
            o.Add("a", (s) => { });
            o.Add("b", (s) => { });
            o["a"].Requires("b");

            Assert.Throws<InvalidOperationException>(() => o.Parse(new[] { "a" }));
            Assert.DoesNotThrow(() => o.Parse(new[] { "b" }));
        }
    }
}
