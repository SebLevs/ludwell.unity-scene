using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ludwell.Scene
{
    // [CreateAssetMenu(fileName = "TagContainer", menuName = "SceneDataManager/TagContainer")]
    [Serializable]
    public class TagContainer : ScriptableObject
    {
        [field: HideInInspector]
        [field: SerializeField]
        public List<TagWithSubscribers> Tags { get; set; } = new();

        public bool CanTagBeAdded(Tag tag)
        {
            return !string.IsNullOrEmpty(tag.Name) && !IsTagDuplicate(tag);
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