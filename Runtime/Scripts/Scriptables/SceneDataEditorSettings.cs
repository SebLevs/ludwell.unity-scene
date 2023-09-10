using UnityEngine;

namespace Ludwell.Scene
{
    [CreateAssetMenu(fileName = "SceneDataEditorSettings", menuName = "Scene Manager/SceneDataEditorSettings")]
    public class SceneDataEditorSettings : ScriptableObject
    {
        public string SceneFolder = "Assets/Scenes/";
        public string[] GetSceneFolders => new[] { SceneFolder };
        public string SceneDataFolder = "Assets/Scriptables/SceneDatas";
        public string[] GetSceneDataFolders => new[] { SceneDataFolder };
    }
}

