using System;
using System.Collections.Generic;
using Ludwell.UIToolkitElements.Editor;
using UnityEngine;

namespace Ludwell.Scene.Editor
{
    [Serializable]
    public class TagSubscriberWithTags : IListable
    {
        [field: SerializeField] public List<Tag> Tags { get; set; } = new();
        
        [SerializeField] private string _name;

        public string ID
        {
            get => _name;
            set
            {
                if (_name == value) return;
                _name = value;
            }
        }

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