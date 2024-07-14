using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ludwell.Scene.Editor
{
    [Serializable]
    public class TagSubscriberWithTags : TagSubscriber
    {
        [field: SerializeField] public List<Tag> Tags { get; set; } = new();

        public void Clear()
        {
            Tags.Clear();
        }

        public void Add(TagWithSubscribers tag)
        {
            if (Tags.Contains(tag)) return;

            Tags.Add(tag);

            if (tag.Subscribers.Contains(this)) return;
            tag.Add(this);
        }

        public void Remove(TagWithSubscribers tag)
        {
            if (!Tags.Contains(tag)) return;

            Tags.Remove(tag);
            
            if (!tag.Subscribers.Contains(this)) return;
            tag.Remove(this);
        }

        public void RemoveFromAllTags()
        {
            foreach (var tag in Tags)
            {
                (tag as TagWithSubscribers)?.Remove(this);
            }

            Tags.Clear();
        }
    }
}