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
        public List<TagWithSubscribers> Tags { get; set; } = new();

        public Action<TagWithSubscribers> OnAdd;
        public Action<TagWithSubscribers> OnRemove;

        public bool ContainsTagWithName(string value)
        {
            foreach (var tag in Tags)
            {
                if (!string.Equals(tag.Name, value, StringComparison.CurrentCultureIgnoreCase)) continue;
                Debug.LogWarning($"Tag name already exists | {tag.Name}");
                return true;
            }

            return false;
        }

        public void AddTag(TagWithSubscribers tag)
        {
            Tags.Add(tag);
            OnAdd?.Invoke(tag);
        }

        public void RemoveTag(TagWithSubscribers tag)
        {
            if (!Tags.Remove(tag)) return;
            tag.RemoveFromAllSubscribers();
            OnRemove?.Invoke(tag);
        }
    }
}