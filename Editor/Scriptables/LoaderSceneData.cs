using System.Collections.Generic;
using UnityEngine;

namespace Ludwell.Scene
{
    // [CreateAssetMenu(fileName = "LoaderSceneData", menuName = "SceneDataManager/LoaderSceneData")]
    public class LoaderSceneData : ScriptableObject
    {
        public List<LoaderListViewElementData> Elements { get; set; } = new();
    }

    public class LoaderListViewElementData
    {
        public string Name { get; set; } = LoaderListViewElement.DefaultHeaderTextValue;
        public bool IsOpen { get; set; } = true;
        public SceneData MainScene { get; set; }
        public List<SceneDataReference> RequiredScenes { get; set; } = new();
    }

    public class SceneDataReference
    {
        public SceneData SceneData { get; set; }
    }
}