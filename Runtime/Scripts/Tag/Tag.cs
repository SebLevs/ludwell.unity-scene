using System;
using UnityEngine;

namespace Ludwell.Scene
{
    [Serializable]
    public class Tag : IListable, IComparable
    {
        [SerializeField] private string name;

        private Action<string> _onValueChanged;

        public void AddValueChangedCallback(Action<string> callback)
        {
            _onValueChanged += callback;
        }

        public void RemoveValueChangedCallback(Action<string> callback)
        {
            _onValueChanged -= callback;
        }

        public string Name
        {
            get => name;
            set
            {
                if (name == value) return;
                name = value;
                _onValueChanged?.Invoke(name);
            }
        }

        public string GetName()
        {
            return name;
        }

        public int CompareTo(object obj)
        {
            if (obj is not Tag other) return 1;
            return string.Compare(Name, other.Name, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}