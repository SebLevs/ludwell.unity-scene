using System.Collections.Generic;
using UnityEngine;

namespace Ludwell.Scene
{
    // [CreateAssetMenu(fileName = "LoaderSceneData", menuName = "SceneDataManager/LoaderSceneData")]
    public class LoaderSceneData : ScriptableObject
    {
        public List<LoaderListViewElementData> Elements { get; set; } = new();
        
        public int GetIndexOfRequiredScene(SceneData sceneData)
        {
            foreach (var element in Elements)
            {
                if (element.RequiredScenes.Contains(sceneData))
                {
                    return element.GetIndexOfRequiredScene(sceneData);
                }
            }

            return -1;
        }
    }

    public class LoaderListViewElementData
    {
        public string Name { get; set; }
        public bool IsOpen { get; set; }
        public SceneData MainScene { get; set; }
        public List<SceneData> RequiredScenes { get; set; } = new();
        
        public SceneData GetRequiredScene(SceneData sceneData)
        {
            return RequiredScenes.Find(element => element == sceneData);
        }
        
        public int GetIndexOfRequiredScene(SceneData sceneData)
        {
            return RequiredScenes.IndexOf(sceneData);
        }
    }
}