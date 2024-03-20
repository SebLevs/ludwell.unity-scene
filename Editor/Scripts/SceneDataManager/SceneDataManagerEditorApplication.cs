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

        public static void OpenSceneAdditive(SceneData sceneData)
        {
            EditorSceneManager.OpenScene(GetSceneAssetPath(sceneData), OpenSceneMode.Additive);
        }

        private static string GetSceneAssetPath(SceneData sceneData)
        {
            var fullPath = AssetDatabase.GetAssetPath(sceneData);
            var directory = Path.GetDirectoryName(fullPath);
            var name = Path.GetFileNameWithoutExtension(fullPath);
            return Path.Combine(directory, name + ".unity");
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