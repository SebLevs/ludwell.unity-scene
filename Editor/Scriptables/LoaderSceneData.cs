using System;
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
        public string Name { get; set; }
        public bool IsOpen { get; set; }
        public SceneData MainScene { get; set; }
        public List<SceneData> RequiredScenes { get; set; } = new();
    }
}