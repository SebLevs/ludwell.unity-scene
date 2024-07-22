using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ludwell.Scene.Editor
{
    [Serializable]
    public class TagContainer : ScriptableObject
    {
        [field: HideInInspector]
        [field: SerializeField]
        public List<Tag> Tags { get; set; } = new();

        public bool ContainsTagWithName(string value)
        {
            foreach (var tag in Tags)
            {
                if (!string.Equals(tag.ID, value, StringComparison.CurrentCultureIgnoreCase)) continue;
                Debug.LogWarning($"Tag name already exists | {tag}");
                return true;
            }

            return false;
        }

        public void Add(Tag tag)
        {
            Tags.Add(tag);
        }

        public void Remove(Tag tag)
        {
            Tags.Remove(tag);
        }
    }
}