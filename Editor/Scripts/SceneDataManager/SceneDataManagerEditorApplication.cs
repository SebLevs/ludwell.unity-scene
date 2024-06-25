using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Ludwell.Scene.Editor
{
    public static class SceneDataManagerEditorApplication
    {
        public static void OpenScene(SceneData sceneData)
        {
            EditorSceneManager.OpenScene(GetSceneAssetPath(sceneData));
        }

        public static void OpenScene(string path)
        {
            EditorSceneManager.OpenScene(path);
        }

        public static void OpenSceneAdditive(SceneData sceneData)
        {
            EditorSceneManager.OpenScene(GetSceneAssetPath(sceneData), OpenSceneMode.Additive);
        }

        public static void RemoveSceneAdditive(SceneData sceneData)
        {
            var scene = SceneManager.GetSceneByName(sceneData.name);
            if (!scene.isLoaded) return;
            EditorSceneManager.CloseScene(scene, true);
        }

        /// <summary>
        /// Close a scene from the hierarchy.
        /// </summary>
        /// <param name="sceneData">The scene to close.</param>
        /// <param name="isRemove">Should the scene be removed from the hierarchy.</param>
        public static void CloseScene(SceneData sceneData, bool isRemove)
        {
            if (!sceneData) return;
            if (!EditorSceneManager.GetSceneByName(sceneData.Name).isLoaded) return;
            var scene = SceneManager.GetSceneByName(sceneData.Name);
            EditorSceneManager.CloseScene(scene, isRemove);
        }

        public static string GetSceneAssetPath(SceneData sceneData)
        {
            var fullPath = AssetDatabase.GetAssetPath(sceneData);
            return Path.ChangeExtension(fullPath, ".unity");
        }

        public static void AddSceneToBuildSettings(SceneData sceneData)
        {
            var path = GetSceneAssetPath(sceneData);
            if (SceneIsInBuildSettings(path)) return;
            
            var buildSettingsScenes = EditorBuildSettings.scenes;
            ArrayUtility.Add(ref buildSettingsScenes, new EditorBuildSettingsScene(path, true));
            EditorBuildSettings.scenes = buildSettingsScenes;
        }
        
        public static void RemoveSceneFromBuildSettings(SceneData sceneData)
        {
            var path = GetSceneAssetPath(sceneData);
            Debug.LogError(path);
            var buildSettingsScenes = EditorBuildSettings.scenes;

            for (var i = buildSettingsScenes.Length - 1; i >= 0; i--)
            {
                if (!string.Equals(buildSettingsScenes[i].path, path)) continue;

                ArrayUtility.RemoveAt(ref buildSettingsScenes, i);
                EditorBuildSettings.scenes = buildSettingsScenes;
                break;
            }
        }
        
        private static bool SceneIsInBuildSettings(string sceneAssetPath)
        {
            var buildScenes = EditorBuildSettings.scenes;

            foreach (var buildScene in buildScenes)
            {
                if (!string.Equals(buildScene.path, sceneAssetPath)) continue; 
                return true;
            }

            return false;
        }
    }
}