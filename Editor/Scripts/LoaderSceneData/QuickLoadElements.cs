using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ludwell.Scene.Editor
{
    // [CreateAssetMenu(fileName = "QuickLoadData", menuName = "SceneDataManager/QuickLoadData")]
    [Serializable]
    public class QuickLoadElements : ScriptableObject
    {
        [field: HideInInspector]
        [field: SerializeField]
        public List<LoaderListViewElementData> Elements { get; set; } = new();

        public void AddElement(SceneData sceneData)
        {
            Elements.Add(new LoaderListViewElementData()
            {
                Name = sceneData.Name,
                MainScene = sceneData
            });

            DataFetcher.SaveEveryScriptableDelayed();
            Signals.Dispatch<UISignals.RefreshQuickLoadListView>();
        }

        public void RemoveElement(SceneData sceneData)
        {
            for (var index = Elements.Count - 1; index >= 0; index--)
            {
                var element = Elements[index];
                if (element.MainScene != sceneData) continue;
                Elements.Remove(element);
            }

            DataFetcher.SaveEveryScriptableDelayed();
            Signals.Dispatch<UISignals.RefreshQuickLoadListView>();
        }

        public void UpdateElement(string oldName, string newName)
        {
            foreach (var element in Elements)
            {
                if (element.Name != oldName) continue;
                if (element.MainScene.name != newName) continue;
                element.Name = newName;
            }

            DataFetcher.SaveEveryScriptableDelayed();
            Signals.Dispatch<UISignals.RefreshQuickLoadListView>();
        }
    }

    [Serializable]
    public class LoaderListViewElementData : TagSubscriberWithTags
    {
        [SerializeField] private bool isOpen = true;
        [SerializeField] private SceneData mainScene;

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

        public SceneData MainScene
        {
            get => mainScene;
            set
            {
                mainScene = value;
                DataFetcher.SaveEveryScriptableDelayed();
            }
        }

        public LoaderListViewElementData()
        {
            Name = LoaderListViewVisualElement.DefaultHeaderTextValue;
        }
    }
}