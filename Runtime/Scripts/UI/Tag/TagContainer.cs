using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ludwell.Scene
{
    [Serializable]
    public class TagContainer : ScriptableObject
    {
        [field: HideInInspector]
        [field: SerializeField]
        public List<TagWithSubscribers> Tags { get; set; } = new();

        public Action<TagWithSubscribers> OnAdd;
        public Action<TagWithSubscribers> OnRemove;

        public void HandleUpdatedTag(TagWithSubscribers tag)
        {
            if (!Tags.Contains(tag)) return;
            if (IsTagValid(tag)) return;
            RemoveTag(tag);
        }

        public bool IsTagValid(Tag tag)
        {
            return !string.IsNullOrEmpty(tag.Name) && !IsTagDuplicate(tag);
        }

        public void AddTag(TagWithSubscribers tag)
        {
            if (!IsTagValid(tag)) return;
            Tags.Add(tag);
            OnAdd?.Invoke(tag);
        }

        public void RemoveTag(TagWithSubscribers tag)
        {
            if (!Tags.Contains(tag)) return;
            Tags.Remove(tag);
            tag.RemoveFromAllSubscribers();
            OnRemove?.Invoke(tag);
        }

        private bool IsTagDuplicate(Tag evaluatedTag)
        {
            foreach (var tag in Tags)
            {
                if (tag == evaluatedTag) continue;
                if (string.Equals(tag.Name, evaluatedTag.Name, StringComparison.CurrentCultureIgnoreCase)) return true;
            }

            return false;
        }
    }
}