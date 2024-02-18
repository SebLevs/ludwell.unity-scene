using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ludwell.Scene
{
    [Serializable]
    public class TagWithSubscribers : Tag
    {
        [SerializeField] private List<TagSubscriber> _subscribers = new();

        public void AddSubscriber(TagSubscriber subscriber)
        {
            if (_subscribers.Contains(subscriber)) return;
            _subscribers.Add(subscriber);
        }

        public void RemoveSubscriber(TagSubscriber subscriber)
        {
            _subscribers.Remove(subscriber);
        }

        public void RemoveFromAllSubscribers()
        {
            foreach (var subscriber in _subscribers)
            {
                ((TagSubscriberWithTags)subscriber).Tags.Remove(this);
            }
        }
    }
}