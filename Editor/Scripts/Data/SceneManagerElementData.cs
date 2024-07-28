using System;
using UnityEngine;

namespace Ludwell.Scene.Editor
{
    [Serializable]
    public class SceneManagerElementData : TagSubscriberWithTags, IComparable
    {
        [HideInInspector] public SceneData SceneData;
        [HideInInspector] public bool IsOutsideAssetsFolder;

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;

            var otherAsType = (SceneManagerElementData)obj;
            return string.Compare(ID, otherAsType.ID, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}