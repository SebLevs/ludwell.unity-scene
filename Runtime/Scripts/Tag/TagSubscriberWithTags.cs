using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ludwell.Scene
{
    [Serializable]
    public abstract class TagSubscriberWithTags
    {
        [field: SerializeField] public List<Tag> Tags { get; set; } = new();

        public abstract string GetTagSubscriberWithTagID();

        public void Clear()
        {
            Tags.Clear();
        }

        public void Add(Tag tag)
        {
            if (Tags.Contains(tag)) return;

            Tags.Add(tag);
        }

        public void Remove(Tag tag)
        {
            if (!Tags.Contains(tag)) return;

            Tags.Remove(tag);
        }
    }
}
