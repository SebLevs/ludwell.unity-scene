using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace Ludwell.Scene.Editor
{
    public class SceneDataManagerEditorApplication
    {
        public static void OpenScene(SceneData sceneData)
        {
            if (EditorApplication.isPlaying) return;
            EditorSceneManager.OpenScene(GetSceneAssetPath(sceneData));
        }

        public static void OpenScene(string path)
        {
            if (EditorApplication.isPlaying) return;
            EditorSceneManager.OpenScene(path);
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
        
        private static string GetSceneAssetPath(SceneData sceneData)
        {
            var fullPath = AssetDatabase.GetAssetPath(sceneData);
            return Path.ChangeExtension(fullPath, ".unity");
        }
    }
}