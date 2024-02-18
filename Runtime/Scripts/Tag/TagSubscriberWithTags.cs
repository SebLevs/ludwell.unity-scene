using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ludwell.Scene
{
    [Serializable]
    public class TagSubscriberWithTags : TagSubscriber
    {
        [field: SerializeField] public List<Tag> Tags { get; set; } = new();

        public void RemoveFromAllTags()
        {
            foreach (var tag in Tags)
            {
                (tag as TagWithSubscribers).RemoveSubscriber(this);
            }
            Tags.Clear();
        }
    }
}