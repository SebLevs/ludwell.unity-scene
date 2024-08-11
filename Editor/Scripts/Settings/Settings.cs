using Ludwell.EditorUtilities;
using UnityEditor;
using UnityEngine;

namespace Ludwell.Scene.Editor
{
    public class Settings : ScriptableObject
    {
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
                ResourcesLocator.GetTags().Elements.Clear();
                ResourcesLocator.GetSceneAssetDataBinders().Elements.Clear();

                ResourcesLocator.SaveSceneAssetDataBindersAndTagsDelayed();
                AssetDatabase.Refresh();
            }

            GUILayout.Space(2);

            if (GUILayout.Button($"Repopulate {nameof(SceneAssetDataBinders)}"))
            {
                ResourcesLocator.GetSceneAssetDataBinders().Elements.Clear();
                DataSolver.PopulateSceneAssetDataBinders();
                ResourcesLocator.SaveSceneAssetDataBindersDelayed();
            }

            EditorPainter.DrawSeparatorLine();
        }
    }
}