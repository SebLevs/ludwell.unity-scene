using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace Ludwell.Scene.Editor
{
    public static class SceneDataManagerEditorApplication
    {
        public static void OpenScene(string path)
        {
            EditorSceneManager.OpenScene(path);
        }
        
        public static void OpenScene(SceneData sceneData)
        {
            EditorSceneManager.OpenScene(GetSceneAssetPath(sceneData));
        }

        public static void OpenSceneAdditive(SceneData sceneData)
        {
            EditorSceneManager.OpenScene(GetSceneAssetPath(sceneData), OpenSceneMode.Additive);
        }
        
        /// <summary>
        /// Close a scene from the hierarchy.
        /// </summary>
        /// <param name="sceneData">The scene to close.</param>
        /// <param name="isRemove">Should the scene be removed from the hierarchy.</param>
        public static void CloseScene(SceneData sceneData, bool isRemove)
        {
            if (!EditorSceneManager.GetSceneByName(sceneData.Name).isLoaded) return;
            var scene = SceneManager.GetSceneByName(sceneData.Name);
            EditorSceneManager.CloseScene(scene, isRemove);
        }

        public static void RemoveSceneAdditive(SceneData sceneData)
        {
            var scene = SceneManager.GetSceneByName(sceneData.name);
            if (!scene.isLoaded) return;
            EditorSceneManager.CloseScene(scene, true);
        }

        public static string GetSceneAssetPath(SceneData sceneData)
        {
            var fullPath = AssetDatabase.GetAssetPath(sceneData);
            return Path.ChangeExtension(fullPath, ".unity");
        }
        
        public static bool IsSceneInBuildSettings(string sceneAssetPath)
        {
            var buildScenes = EditorBuildSettings.scenes;

            foreach (var buildScene in buildScenes)
            {
                if (!string.Equals(buildScene.path, sceneAssetPath)) continue;
                return true;
            }

            return false;
        }

        public static void AddSceneToBuildSettings(SceneData sceneData)
        {
            var path = GetSceneAssetPath(sceneData);
            if (IsSceneInBuildSettings(path)) return;

            var buildSettingsScenes = EditorBuildSettings.scenes;
            ArrayUtility.Add(ref buildSettingsScenes, new EditorBuildSettingsScene(path, true));
            EditorBuildSettings.scenes = buildSettingsScenes;
        }

        public static void RemoveSceneFromBuildSettings(SceneData sceneData)
        {
            var path = GetSceneAssetPath(sceneData);
            var buildSettingsScenes = EditorBuildSettings.scenes;

            for (var i = buildSettingsScenes.Length - 1; i >= 0; i--)
            {
                if (!string.Equals(buildSettingsScenes[i].path, path)) continue;

                ArrayUtility.RemoveAt(ref buildSettingsScenes, i);
                EditorBuildSettings.scenes = buildSettingsScenes;
                break;
            }
        }

        // todo: optimise
        public static bool IsActiveScene(SceneData sceneData)
        {
            var path = GetSceneAssetPath(sceneData);
            return EditorSceneManager.GetActiveScene().path == path;
        }

        // todo: optimise
        public static bool IsSceneLoaded(SceneData sceneData)
        {
            var path = GetSceneAssetPath(sceneData);
            return EditorSceneManager.GetSceneByPath(path).isLoaded;
        }

        // todo: optimise
        public static void SetActiveScene(SceneData sceneData)
        {
            var path = GetSceneAssetPath(sceneData);
            EditorSceneManager.SetActiveScene(EditorSceneManager.GetSceneByPath(path));
        }
    }
}