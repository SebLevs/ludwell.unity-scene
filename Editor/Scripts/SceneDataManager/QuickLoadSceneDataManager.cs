using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Ludwell.Scene.Editor
{
    public class QuickLoadSceneDataManager
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
            var path = SceneDataManagerEditorApplication.GetSceneAssetPath(_sceneData);
            if (SceneManager.GetActiveScene() != SceneManager.GetSceneByPath(path))
            {
                SceneDataManagerEditorApplication.OpenScene(_sceneData);
            }

            EditorApplication.EnterPlaymode();
        }
    }
}