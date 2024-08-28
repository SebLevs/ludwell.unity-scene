using UnityEditor;
using UnityEngine.SceneManagement;

namespace Ludwell.SceneManagerToolkit.Editor
{
    public class SceneManagerHelper
    {
        private static SceneAssetDataBinder _sceneAssetDataBinder;

        public static void LoadScene(SceneAssetDataBinder sceneData)
        {
            _sceneAssetDataBinder = sceneData;
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
            if (SceneManager.GetActiveScene() != SceneManager.GetSceneByPath(_sceneAssetDataBinder.Data.Path))
            {
                EditorSceneManagerHelper.OpenScene(_sceneAssetDataBinder.Data.Path);
            }

            EditorApplication.EnterPlaymode();
        }
    }
}