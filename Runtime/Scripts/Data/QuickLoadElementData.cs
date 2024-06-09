using System;
using UnityEngine;

namespace Ludwell.Scene
{
    [Serializable]
    public class QuickLoadElementData : TagSubscriberWithTags, IComparable
    {
        [HideInInspector] public SceneData SceneData;
        [HideInInspector] public bool IsOpen = true;
        [HideInInspector] public bool IsOutsideAssetsFolder;

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;

            var otherAsType = (QuickLoadElementData)obj;
            return string.Compare(Name, otherAsType.Name, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}