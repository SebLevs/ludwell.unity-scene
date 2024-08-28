using System;
using System.Collections.Generic;

namespace Ludwell.Architecture
{
    public static class Signals
    {
        private static readonly Dictionary<string, List<Action>> _signals = new();

        public static void Clear<T>()
        {
            const string type = nameof(T);
            if (!_signals.ContainsKey(type)) return;
            _signals.Remove(type);
        }

        public static void Add<T>(Action action) where T : ISignal
        {
            const string type = nameof(T);
            if (!_signals.ContainsKey(type))
            {
                _signals.Add(type, new List<Action>());
            }

            if (_signals[type].Count > 0)
            {
                foreach (var evt in _signals[type])
                {
                    if (evt.Method.Name != action.Method.Name) continue;
                    return;
                }
            }

            _signals[type].Add(action);
        }

        public static void Remove<T>(Action action) where T : ISignal
        {
            const string type = nameof(T);
            _signals[type].Remove(action);

            if (_signals[type].Count > 0) return;
            _signals.Remove(type);
        }

        public static void Dispatch<T>() where T : ISignal
        {
            const string type = nameof(T);
            if (!_signals.TryGetValue(type, out var actions)) return;
            foreach (var action in actions)
            {
                action?.Invoke();
            }
        }
    }
}
