using UnityEditor;
using UnityEngine;

namespace Ludwell.Scene.Editor
{
    [CreateAssetMenu(fileName = "SceneDataManagerSettings", menuName = "Scene Manager/SceneDataManagerSettings")]
    public class SceneDataManagerSettings : ScriptableObject
    {
        [HideInInspector] public bool GenerateSceneData = true;

        public string SceneFolder = "Assets/Scenes/";
        public string[] GetSceneFolders => new[] { SceneFolder };
        public string SceneDataFolder = "Assets/Scriptables/SceneDatas";
        public string[] GetSceneDataFolders => new[] { SceneDataFolder };
    }

    [CustomEditor(typeof(SceneDataManagerSettings))]
    public class SceneDataManagerSettingsEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var baseScript = (SceneDataManagerSettings)target;

            GenerateSceneDataButton(baseScript);

            base.OnInspectorGUI();
        }

        private static void GenerateSceneDataButton(SceneDataManagerSettings baseScript)
        {
            if (GUILayout.Button("Generate Scene Data"))
            {
                baseScript.GenerateSceneData = true;
                SceneDataGenerator.GenerateSceneData();
            }

            GUILayout.Space(8);
        }
    }
}