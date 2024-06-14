using UnityEditor;
using UnityEngine;

namespace Ludwell.Scene.Editor
{
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
            if (GUILayout.Button("Clear"))
            {
                ResourcesFetcher.GetTagContainer().Tags.Clear();
                ResourcesFetcher.GetQuickLoadElements().Elements.Clear();
                var coreScenes = ResourcesFetcher.GetCoreScenes();
                coreScenes.LoadingScene = null;
                coreScenes.StartingScene = null;
                coreScenes.PersistentScene = null;
                
                ResourcesFetcher.SaveQuickLoadElementsAndTagContainerDelayed();
                AssetDatabase.Refresh();
            }

            GUILayout.Space(2);

            if (GUILayout.Button("Repopulate Quick Load"))
            {
                ResourcesFetcher.GetTagContainer().Tags.Clear();
                ResourcesFetcher.GetQuickLoadElements().Elements.Clear();

                var assetGuids = AssetDatabase.FindAssets("t:SceneData");
                foreach (var guid in assetGuids)
                {
                    var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                    var sceneData = AssetDatabase.LoadAssetAtPath<SceneData>(assetPath);
                    var element = ResourcesFetcher.GetQuickLoadElements().Add(sceneData);

                    var path = AssetDatabase.GetAssetPath(sceneData);
                    element.IsOutsideAssetsFolder = !path.Contains("Assets/");
                    Signals.Dispatch<UISignals.RefreshView>();
                }
                
                ResourcesFetcher.SaveQuickLoadElementsAndTagContainerDelayed();
                AssetDatabase.Refresh();
            }

            GUILayout.Space(2);

            if (GUILayout.Button("Generate SceneData assets"))
            {
                baseScript.GenerateSceneData = true;
                SceneDataGenerator.GenerateSceneData();
            }

            EditorPainter.DrawSeparatorLine();
        }
    }
}