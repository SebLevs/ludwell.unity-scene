using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace Ludwell.Scene.Editor
{
    public static class EditorSceneManagerHelper
    {
        public static bool IsPathOutsideAssets(string path)
        {
            return !path.Contains("Assets/");
        }

        public static void OpenScene(string path)
        {
            EditorSceneManager.OpenScene(path);
        }

        public static void OpenSceneAdditive(string path)
        {
            EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);
        }

        /// <summary>
        /// Close a scene from the hierarchy.
        /// </summary>
        /// <param name="path">The scene to close.</param>
        /// <param name="isRemove">Should the scene be removed from the hierarchy.</param>
        public static void CloseScene(string path, bool isRemove)
        {
            if (!EditorSceneManager.GetSceneByPath(path).isLoaded) return;
            var scene = SceneManager.GetSceneByPath(path);
            EditorSceneManager.CloseScene(scene, isRemove);
        }

        public static void RemoveSceneAdditive(string path)
        {
            var scene = SceneManager.GetSceneByPath(path);
            if (!scene.isLoaded) return;
            EditorSceneManager.CloseScene(scene, true);
        }

        public static bool IsSceneInBuildSettings(string path)
        {
            var buildScenes = EditorBuildSettings.scenes;

            foreach (var buildScene in buildScenes)
            {
                if (!string.Equals(buildScene.path, path)) continue;
                return true;
            }

            return false;
        }

        public static void AddSceneToBuildSettings(string path)
        {
            if (IsSceneInBuildSettings(path)) return;

            var buildSettingsScenes = EditorBuildSettings.scenes;
            ArrayUtility.Add(ref buildSettingsScenes, new EditorBuildSettingsScene(path, true));
            EditorBuildSettings.scenes = buildSettingsScenes;
        }

        public static void RemoveSceneFromBuildSettings(string path)
        {
            var buildSettingsScenes = EditorBuildSettings.scenes;

            for (var i = buildSettingsScenes.Length - 1; i >= 0; i--)
            {
                if (!string.Equals(buildSettingsScenes[i].path, path)) continue;

                ArrayUtility.RemoveAt(ref buildSettingsScenes, i);
                EditorBuildSettings.scenes = buildSettingsScenes;
                break;
            }
        }

        public static bool IsActiveScene(string path)
        {
            return EditorSceneManager.GetActiveScene().path == path;
        }

        public static bool IsSceneLoaded(string path)
        {
            return EditorSceneManager.GetSceneByPath(path).isLoaded;
        }

        public static void SetActiveScene(string path)
        {
            EditorSceneManager.SetActiveScene(EditorSceneManager.GetSceneByPath(path));
        }
    }
}