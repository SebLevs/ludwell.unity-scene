using System;
using System.Collections.Generic;
using Ludwell.Scene.Editor;
using UnityEditor;
using UnityEngine;

namespace Ludwell.Scene
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
    }

    [Serializable]
    public class LoaderListViewElementData
    {
        
        [SerializeField] private string name = LoaderListViewElement.DefaultHeaderTextValue;
        [SerializeField] private bool isOpen = true;
        [SerializeField] private SceneData mainScene;
        
        [field: SerializeField] public List<SceneDataReference> RequiredScenes { get; set; } = new();
        
        public string Name
        {
            get => name;
            set
            {
                if (name == value) return;
                name = value;
                LoaderSceneDataHelper.DelayedSaveChange();
            }
        }

        public bool IsOpen
        {
            get => isOpen;
            set
            {
                if (isOpen == value) return;
                isOpen = value;
                LoaderSceneDataHelper.SaveChange();
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
    }

    [Serializable]
    public class SceneDataReference
    {
        private SceneData _sceneData;

        public SceneData SceneData
        {
            get => _sceneData;
            set
            {
                _sceneData = value;
                LoaderSceneDataHelper.SaveChange();
            }
        }
    }

    public static class LoaderSceneDataHelper
    {
        private static LoaderSceneData _loaderSceneData;
        private static DelayedEditorUpdateAction _delayedEditorUpdateAction;

        public static void SaveChange()
        {
            if (!_loaderSceneData)
            {
                _loaderSceneData = Resources.Load<LoaderSceneData>("Scriptables/" + nameof(LoaderSceneData));
            }

            EditorUtility.SetDirty(_loaderSceneData);
            AssetDatabase.SaveAssetIfDirty(_loaderSceneData);
        }

        public static void DelayedSaveChange()
        {
            _delayedEditorUpdateAction ??= new DelayedEditorUpdateAction(0.5f, SaveChange);
            _delayedEditorUpdateAction.StartOrRefresh();
        }
    }
}
