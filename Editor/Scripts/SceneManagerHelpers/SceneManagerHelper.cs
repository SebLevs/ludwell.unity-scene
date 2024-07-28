using UnityEditor;
using UnityEngine.SceneManagement;

namespace Ludwell.Scene.Editor
{
    public class SceneManagerHelper
    {
        private static SceneData _sceneData;

        public static void LoadScene(SceneData sceneData)
        {
            _sceneData = sceneData;
            if (EditorApplication.isPlaying)
            {
                EditorApplication.playModeStateChanged += OnEnteredEditModeLoadScene;
                EditorApplication.ExitPlaymode();
                return;
            }

            LoadScene();
        }

        private static void OnEnteredEditModeLoadScene(PlayModeStateChange obj)
        {
            if (obj != PlayModeStateChange.EnteredEditMode) return;
            EditorApplication.playModeStateChanged -= OnEnteredEditModeLoadScene;
            LoadScene();
        }

        private static void LoadScene()
        {
            var path = EditorSceneManagerHelper.GetSceneAssetPath(_sceneData);
            if (SceneManager.GetActiveScene() != SceneManager.GetSceneByPath(path))
            {
                EditorSceneManagerHelper.OpenScene(_sceneData);
            }

            EditorApplication.EnterPlaymode();
        }
    }
}