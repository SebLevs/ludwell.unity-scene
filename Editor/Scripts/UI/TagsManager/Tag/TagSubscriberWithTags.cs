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

        public void RemoveFromAllTags()
        {
            foreach (var tag in Tags)
            {
                (tag as TagWithSubscribers)?.RemoveSubscriber(this);
            }

            Tags.Clear();
        }
    }
}