using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ludwell.Scene
{
    // [CreateAssetMenu(fileName = "LoaderSceneData", menuName = "SceneDataManager/LoaderSceneData")]
    [Serializable]
    public class LoaderSceneData : ScriptableObject
    {
        public SceneData MainMenuScene;
        public SceneData PersistentScene;
        public SceneData LoadingScene;
        [field:SerializeField] public List<LoaderListViewElementData> Elements { get; set; } = new();
    }

    [Serializable]
    public class LoaderListViewElementData
    {
        [field:SerializeField] public string Name { get; set; } = LoaderListViewElement.DefaultHeaderTextValue;
        [field:SerializeField] public bool IsOpen { get; set; } = true;
        [field:SerializeField] public SceneData MainScene { get; set; }
        [field:SerializeField] public List<SceneDataReference> RequiredScenes { get; set; } = new();
    }

    [Serializable]
    public class SceneDataReference
    {
        [field:SerializeField] public SceneData SceneData { get; set; }
    }
}