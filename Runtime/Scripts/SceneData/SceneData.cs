using UnityEngine;

namespace Ludwell.Scene
{
    public class SceneData : ScriptableObject
    {
        public string Name { get; private set; }
#if UNITY_EDITOR
        [SerializeField] private UnityEditor.SceneAsset editorSceneAsset;
        public UnityEditor.SceneAsset EditorSceneAsset
        {
            get => editorSceneAsset;
            set
            {
                editorSceneAsset = value;
                Name = value.name;
            }
        }
#endif
    }
}
