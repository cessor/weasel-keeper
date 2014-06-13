using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace WeaselKeeper
{
    internal class Options : IEnumerable<Tuple<string, string>>
    {
        private readonly Dictionary<string, Call> _options = new Dictionary<string, Call>();
        private readonly Dictionary<string, IEnumerable<string>> _requirements = new Dictionary<string, IEnumerable<string>>();

        public IEnumerator<Tuple<string, string>> GetEnumerator()
        {
            return _options.Select(pair => Tuple.Create(pair.Key, pair.Value.Method.Name)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Options Add(string option, Call action)
        {
            _options.Add(option, action);
            return this;
        }

        public IEnumerable<Call> Parse(IEnumerable<string> commandLine)
        {
            Func<string, bool> wasConfigured = option => _options.ContainsKey(option);
            var knownOptions = commandLine.Where(wasConfigured);
            var actions = knownOptions.Select(o => _options[o]);
            return actions;
        }
    }
}