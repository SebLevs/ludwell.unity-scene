using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Ludwell.Scene.Editor
{
    // [CreateAssetMenu(fileName = "LoaderSceneData", menuName = "SceneDataManager/LoaderSceneData")]
    [Serializable]
    public class LoaderSceneData : ScriptableObject
    {
        [HideInInspector] [SerializeField] private SceneData mainMenuScene;
        [HideInInspector] [SerializeField] private SceneData persistentScene;
        [HideInInspector] [SerializeField] private SceneData loadingScene;

        [field: HideInInspector]
        [field: SerializeField]
        public List<LoaderListViewElementData> Elements { get; set; } = new();

        public SceneData MainMenuScene
        {
            get => mainMenuScene;
            set
            {
                mainMenuScene = value;
                LoaderSceneDataHelper.SaveChange();
            }
        }

        public SceneData PersistentScene
        {
            get => persistentScene;
            set
            {
                persistentScene = value;
                LoaderSceneDataHelper.SaveChange();
            }
        }

        public SceneData LoadingScene
        {
            get => loadingScene;
            set
            {
                loadingScene = value;
                LoaderSceneDataHelper.SaveChange();
            }
        }

        public void RemoveElementWithMainScene(string assetName)
        {
            var hasRemoved = false;
            for (var index = Elements.Count - 1; index >= 0; index--)
            {
                var element = Elements[index];
                if (element.MainScene.Name != assetName) continue;
                Elements.Remove(element);
                hasRemoved = true;
            }

            if (!hasRemoved) return;
            LoaderSceneDataHelper.SaveChange();
        }

        public (SceneAsset, SceneData) GetScenesWithName(string assetName)
        {
            var sceneData = Elements.FirstOrDefault(element => element.MainScene.Name == assetName)?.MainScene;
            var sceneAsset = sceneData?.EditorSceneAsset;
            return (sceneAsset, sceneData);
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
                LoaderSceneDataHelper.SaveChangeDelayed();
            }
        }

        public SceneData MainScene
        {
            get => mainScene;
            set
            {
                mainScene = value;
                LoaderSceneDataHelper.SaveChange();
            }
        }

        public LoaderListViewElementData()
        {
            Name = LoaderListViewElement.DefaultHeaderTextValue;
        }
    }
}