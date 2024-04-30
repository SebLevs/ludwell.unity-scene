using UnityEditor;
using UnityEngine;

namespace Ludwell.Scene.Editor
{
    // [CreateAssetMenu(fileName = "SceneDataManagerSettings", menuName = "Scene Manager/SceneDataManagerSettings")]
    public class SceneDataManagerSettings : ScriptableObject
    {
        [HideInInspector] public bool GenerateSceneData = true;
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
            if (GUILayout.Button("Clear Data"))
            {
                DataFetcher.GetTagContainer().Tags.Clear();
                DataFetcher.GetQuickLoadElements().Elements.Clear();
            }

            GUILayout.Space(4);

            if (GUILayout.Button("Generate Scene Data"))
            {
                baseScript.GenerateSceneData = true;
                SceneDataGenerator.GenerateSceneData();
            }

            GUILayout.Space(8);
        }
    }
}