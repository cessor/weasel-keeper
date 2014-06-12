using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace WeaselKeeper
{
    internal interface IRequire
    {
        IHaveOptions Requires(params string[] others);
    }

    internal interface IHaveOptions
    {
        IRequire Option(string key);
    }

    internal class Options : IEnumerable<Tuple<string, string>>, IHaveOptions, IRequire
    {
        private readonly Dictionary<string, Call> _options = new Dictionary<string, Call>();
        private readonly Dictionary<string, IEnumerable<string>> _requirements = new Dictionary<string, IEnumerable<string>>();
        private string _currentOption = string.Empty;

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

            var options = commandLine.Where(wasConfigured);


            var optionsWithRequirements = options.Intersect(_requirements.Keys);
            var requirements = optionsWithRequirements.SelectMany(o => _requirements[o]);


            var requirementsNotSpecified = requirements.Except(options);

            bool thereAreUnmetRequiorements = !requirementsNotSpecified.Any();
            if (!thereAreUnmetRequiorements)
            {
                throw new InvalidOperationException("TITTE");
            }

            var actions = options.Select(o => _options[o]);
            return actions;
        }

        public IRequire this[string key]
        {
            get { return Option(key); }
        }

        public IRequire Option(string key)
        {
            if (HasNotBeenConfigured(key))
            {
                throw new KeyNotFoundException(string.Format("The flag '{0}' has not yet been added and can't have requirements.", key));
            }
            _currentOption = key;
            return this;
        }

        public IHaveOptions Requires(params string[] others)
        {
            if (others.Any(HasNotBeenConfigured))
            {
                throw new KeyNotFoundException(string.Format("The flags [{0}] have not yet been added and can't be configured as requirements for other flags.", string.Join(", ", others.Where(HasNotBeenConfigured))));
            }
            _requirements.Add(_currentOption, others.ToList());
            return this;
        }

        private bool HasNotBeenConfigured(string key)
        {
            return !_options.ContainsKey(key);
        }
    }
}