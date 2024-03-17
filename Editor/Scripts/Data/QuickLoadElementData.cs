using System;
using UnityEngine;

namespace Ludwell.Scene.Editor
{
    [Serializable]
    public class QuickLoadElementData : TagSubscriberWithTags, IComparable
    {
        [SerializeField] private bool isOpen = true;
        [SerializeField] private SceneData sceneData;

        public bool IsOpen
        {
            get => isOpen;
            set
            {
                if (isOpen == value) return;
                isOpen = value;
                DataFetcher.SaveEveryScriptableDelayed();
            }
        }

        public SceneData SceneData
        {
            get => sceneData;
            set
            {
                sceneData = value;
                DataFetcher.SaveEveryScriptableDelayed();
            }
        }

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;

            var otherAsType = obj as QuickLoadElementData;
            return string.Compare(Name, otherAsType.Name, StringComparison.Ordinal);
        }
    }
}
