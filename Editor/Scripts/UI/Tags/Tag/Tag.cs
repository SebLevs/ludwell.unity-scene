using System;
using Ludwell.UIToolkitElements.Editor;
using UnityEngine;

namespace Ludwell.Scene.Editor
{
    [Serializable]
    public sealed class Tag : IComparable, IListable
    {
        [field: SerializeField] public string ID { get; set; }

        public int CompareTo(object obj)
        {
            if (obj is not Tag other) return 1;
            return string.Compare(ID, other.ID, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}