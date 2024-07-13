using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ludwell.Scene.Editor
{
    [Serializable]
    public class TagWithSubscribers : Tag
    {
        [field: SerializeField] public List<TagSubscriber> Subscribers { get; private set; } = new();

        public void Clear()
        {
            Subscribers.Clear();
        }

        public void AddSubscriber(TagSubscriber subscriber)
        {
            if (Subscribers.Contains(subscriber)) return;
            Subscribers.Add(subscriber);
        }

        public void RemoveSubscriber(TagSubscriber subscriber)
        {
            Subscribers.Remove(subscriber);
        }

        public void RemoveFromAllSubscribers()
        {
            foreach (var subscriber in Subscribers)
            {
                ((TagSubscriberWithTags)subscriber).Tags.Remove(this);
            }
        }
    }
}