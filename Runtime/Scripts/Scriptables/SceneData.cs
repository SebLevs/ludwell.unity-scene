using System;
using UnityEditor;
using UnityEngine;

namespace Ludwell.Scene
{
    [Serializable]
    public class SceneData : ScriptableObject
    {
        public string Name { get; private set; }
#if UNITY_EDITOR
        private SceneAsset _editorSceneAsset;
        public SceneAsset EditorSceneAsset
        {
            get => _editorSceneAsset;
            set
            {
                _editorSceneAsset = value;
                Name = value.name;
            }
        }
#endif
    }
}