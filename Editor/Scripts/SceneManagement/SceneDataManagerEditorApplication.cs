using UnityEditor;
using UnityEditor.SceneManagement;

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
    }
}