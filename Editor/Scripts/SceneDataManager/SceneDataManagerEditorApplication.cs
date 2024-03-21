using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;

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

        private static string GetSceneAssetPath(SceneData sceneData)
        {
            var fullPath = AssetDatabase.GetAssetPath(sceneData);
            return Path.ChangeExtension(fullPath, ".unity");
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
    }
}