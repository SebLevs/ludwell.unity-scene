using System;
using Ludwell.UIToolkitElements;
using UnityEngine;

namespace Ludwell.SceneManagerToolkit
{
    [Serializable]
    public class Tag : IComparable, IListable
    {
        [field: SerializeField] public string ID { get; set; }

        public string GetListableId() => ID;

        public override int GetHashCode()
        {
            return StringComparer.InvariantCultureIgnoreCase.GetHashCode(ID);
        }

        public override bool Equals(object obj)
        {
            if (obj is not Tag other) return false;
            return string.Equals(ID, other.ID, StringComparison.InvariantCultureIgnoreCase);
        }

        public int CompareTo(object obj)
        {
            if (obj is not Tag other) return 1;
            return string.Compare(ID, other.ID, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
