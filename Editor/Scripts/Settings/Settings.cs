using Ludwell.EditorUtilities;
using UnityEditor;
using UnityEngine;

namespace Ludwell.Scene.Editor
{
    public class Settings : ScriptableObject
    {
        public static readonly string GenerateSceneDataKey = "GenerateSceneData";
    }

    [CustomEditor(typeof(Settings))]
    public class SettingsEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            GenerateSceneDataButton();
            base.OnInspectorGUI();
        }

        private static void GenerateSceneDataButton()
        {
            if (GUILayout.Button("Clear all"))
            {
                ResourcesLocator.GetTagContainer().Tags.Clear();
                ResourcesLocator.GetQuickLoadElements().Elements.Clear();

                ResourcesLocator.SaveQuickLoadElementsAndTagContainerDelayed();
                AssetDatabase.Refresh();
            }

            GUILayout.Space(2);

            if (GUILayout.Button("Repopulate Quick Load list"))
            {
                ResourcesLocator.GetTagContainer().Tags.Clear();
                ResourcesLocator.GetQuickLoadElements().Elements.Clear();
                SceneDataGenerator.PopulateQuickLoadElements();
            }

            GUILayout.Space(2);

            if (GUILayout.Button("Generate SceneData assets"))
            {
                EditorPrefs.SetBool(Settings.GenerateSceneDataKey, true);
                SceneDataGenerator.GenerateSceneData();
            }

            EditorPainter.DrawSeparatorLine();
        }
    }
}