using System;
using System.Collections.Generic;
using Ludwell.Architecture;
using UnityEngine;

namespace Ludwell.Scene.Editor
{
    [Serializable]
    public class SceneManagerElements : ScriptableObject
    {
        [field: HideInInspector]
        [field: SerializeField]
        public List<SceneManagerElementData> Elements { get; set; } = new();

        public bool Contains(SceneData sceneData)
        {
            foreach (var element in Elements)
            {
                if (element.SceneData != sceneData) continue;
                return true;
            }

            return false;
        }
        
        public SceneManagerElementData Add(SceneData sceneData)
        {
            var element = AddWithoutNotify(sceneData);
            // Signals.Dispatch<UISignals.RefreshView>();
            return element;
        }

        public SceneManagerElementData AddWithoutNotify(SceneData sceneData)
        {
            var element = new SceneManagerElementData
            {
                ID = sceneData.Name,
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
                Elements.Remove(element);
            }

            Signals.Dispatch<UISignals.RefreshView>();
        }

        public void UpdateElement(string oldName, string newName)
        {
            foreach (var element in Elements)
            {
                if (element.ID != oldName) continue;
                if (element.SceneData.name != newName) continue;
                element.ID = newName;
            }

            // Signals.Dispatch<UISignals.RefreshView>();
        }
    }
}