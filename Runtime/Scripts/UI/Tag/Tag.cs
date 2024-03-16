using System;
using UnityEngine;

namespace Ludwell.Scene
{
    [Serializable]
    public class Tag : IListable, IComparable
    {
        [SerializeField] private string name;

        public Action<string> OnValueChanged;
        
        public string Name
        {
            get => name;
            set
            {
                if (name == value) return;
                name = value;
                OnValueChanged?.Invoke(name);
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