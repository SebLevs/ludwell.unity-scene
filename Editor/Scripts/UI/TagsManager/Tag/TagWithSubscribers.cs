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

        public void Add(TagSubscriberWithTags subscriber)
        {
            if (Subscribers.Contains(subscriber)) return;

            Subscribers.Add(subscriber);
            
            if (subscriber.Tags.Contains(this)) return;
            subscriber.Add(this);
        }

        public void Remove(TagSubscriberWithTags subscriber)
        {
            if (!Subscribers.Contains(subscriber)) return;

            Subscribers.Remove(subscriber);
            
            if (!subscriber.Tags.Contains(this)) return;
            subscriber.Tags.Remove(this);
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