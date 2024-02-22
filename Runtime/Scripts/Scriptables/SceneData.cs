using System;
using UnityEditor;
using UnityEngine;

namespace Ludwell.Scene
{
    [Serializable]
    public class SceneData : ScriptableObject
    {
        public string Name => name;
#if UNITY_EDITOR
        // Todo: delete and modify PostProcessor
        public SceneAsset EditorSceneAsset;
#endif
    }
}