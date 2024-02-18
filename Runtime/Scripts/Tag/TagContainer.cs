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
    }
}