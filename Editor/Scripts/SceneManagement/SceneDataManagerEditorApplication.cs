using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Ludwell.Scene
{
    public class SceneDataManagerEditorApplication
    {
        public static void OpenScene(SceneData sceneData)
        {
            if (EditorApplication.isPlaying) return;
            var path = AssetDatabase.GetAssetPath(sceneData.EditorSceneAsset);
            EditorSceneManager.OpenScene(path);
        }

        public static void LoadScene(SceneData sceneData)
        {
            if (EditorApplication.isPlaying)
            {
                EditorApplication.isPlaying = false;
            }

            EditorApplication.delayCall += () =>
            {
                OpenScene(sceneData);
                EditorApplication.isPlaying = true;
            };
        }

        private static bool TryAddSceneToBuildSettings(SceneData sceneData)
        {
            if (SceneIsInBuildSettings(sceneData.EditorSceneAsset)) return true;
            AddSceneToBuildSettings(sceneData.EditorSceneAsset);
            return false;

        }

        private static bool SceneIsInBuildSettings(Object sceneAsset)
        {
            var scenePath = AssetDatabase.GetAssetPath(sceneAsset);
            var buildScenes = EditorBuildSettings.scenes;

            foreach (var buildScene in buildScenes)
            {
                if (string.Equals(buildScene.path, scenePath))
                {
                    return true;
                }
            }

            return false;
        }

        private static void AddSceneToBuildSettings(Object sceneAsset)
        {
            var scenePath = AssetDatabase.GetAssetPath(sceneAsset);
            var buildSettingsScenes = EditorBuildSettings.scenes;

            ArrayUtility.Add(ref buildSettingsScenes, new EditorBuildSettingsScene(scenePath, true));
            EditorBuildSettings.scenes = buildSettingsScenes;
        }

        private static void RemoveSceneFromBuildSettings(Object sceneAsset)
        {
            var scenePath = AssetDatabase.GetAssetPath(sceneAsset);
            var buildSettingsScenes = EditorBuildSettings.scenes;

            for (var i = buildSettingsScenes.Length; i >= 0; i--)
            {
                if (!string.Equals(buildSettingsScenes[i].path, scenePath)) continue;

                ArrayUtility.RemoveAt(ref buildSettingsScenes, i);
                EditorBuildSettings.scenes = buildSettingsScenes;
                break;
            }
        }
    }
}