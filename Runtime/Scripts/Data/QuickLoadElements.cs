using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ludwell.Scene
{
    // [CreateAssetMenu(fileName = "QuickLoadData", menuName = "SceneDataManager/QuickLoadData")]
    [Serializable]
    public class QuickLoadElements : ScriptableObject
    {
        [field: HideInInspector]
        [field: SerializeField]
        public List<QuickLoadElementData> Elements { get; set; } = new();

        public QuickLoadElementData Add(SceneData sceneData)
        {
            var element = AddWithoutNotify(sceneData);
            Signals.Dispatch<UISignals.RefreshView>();
            return element;
        }

        public QuickLoadElementData AddWithoutNotify(SceneData sceneData)
        {
            var element = new QuickLoadElementData
            {
                Name = sceneData.Name,
                SceneData = sceneData
            };

            Elements.Add(element);
            Elements.Sort();
            return element;
        }

        public void Remove(SceneData sceneData)
        {
            for (var index = Elements.Count - 1; index >= 0; index--)
            {
                var element = Elements[index];
                if (element.SceneData != sceneData) continue;
                element.RemoveFromAllTags();
                Elements.Remove(element);
            }

            Signals.Dispatch<UISignals.RefreshView>();
        }

        public void UpdateElement(string oldName, string newName)
        {
            foreach (var element in Elements)
            {
                if (element.Name != oldName) continue;
                if (element.SceneData.name != newName) continue;
                element.Name = newName;
            }

            Signals.Dispatch<UISignals.RefreshView>();
        }
    }
}