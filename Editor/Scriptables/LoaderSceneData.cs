using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ludwell.Scene
{
    // [CreateAssetMenu(fileName = "LoaderSceneData", menuName = "SceneDataManager/LoaderSceneData")]
    public class LoaderSceneData : ScriptableObject
    {
        [field:SerializeField] public List<LoaderListViewElementData> Elements { get; set; } = new();
    }

    [Serializable]
    public class LoaderListViewElementData
    {
        public string Name { get; set; }
        public bool IsOpen { get; set; }
        public SceneData MainScene { get; set; }
        [field:SerializeField] public List<SceneData> RequiredScenes { get; set; } = new();
    }
}