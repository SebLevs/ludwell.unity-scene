using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ludwell.Scene
{
    [Serializable]
    public class Tags : ScriptableObject
    {
        [field: HideInInspector]
        [field: SerializeField]
        public List<Tag> Elements { get; set; } = new();

        public bool ContainsTagWithName(string value)
        {
            foreach (var tag in Elements)
            {
                if (!string.Equals(tag.ID, value, StringComparison.CurrentCultureIgnoreCase)) continue;
                Debug.LogWarning($"Tag name already exists | {tag}");
                return true;
            }

            return false;
        }

        public void Add(Tag tag)
        {
            Elements.Add(tag);
        }

        public void Remove(Tag tag)
        {
            Elements.Remove(tag);
        }
    }
}
