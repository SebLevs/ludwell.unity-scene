using UnityEngine;

namespace Ludwell.Scene
{
    public class SceneData : ScriptableObject
    {
#if UNITY_EDITOR
        public UnityEditor.SceneAsset EditorSceneAsset;
#endif
    }
}
