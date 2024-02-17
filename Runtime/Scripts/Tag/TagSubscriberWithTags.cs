using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ludwell.Scene
{
    [Serializable]
    public class TagSubscriberWithTags : TagSubscriber
    {
        [field: SerializeField] public List<Tag> Tags { get; set; } = new();
    }
}