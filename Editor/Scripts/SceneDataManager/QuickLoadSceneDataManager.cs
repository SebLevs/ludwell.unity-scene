using UnityEditor;
using UnityEditor.SceneManagement;

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
            SceneDataManagerEditorApplication.OpenScene(_sceneData);

            var persistentScene = DataFetcher.GetCoreScenes().PersistentScene;
            SceneDataManagerEditorApplication.OpenSceneAdditive(persistentScene);
            EditorApplication.EnterPlaymode();

            EditorApplication.playModeStateChanged += OnEnteredEditModeRemovePersistent;
        }

        private static void OnEnteredEditModeRemovePersistent(PlayModeStateChange obj)
        {
            if (obj != PlayModeStateChange.EnteredEditMode) return;
            RemovePersistentScene();
        }

        private static void RemovePersistentScene()
        {
            EditorApplication.playModeStateChanged -= OnEnteredEditModeRemovePersistent;

            if (EditorSceneManager.sceneCount == 1) return;

            var persistentScene = DataFetcher.GetCoreScenes().PersistentScene;
            if (EditorSceneManager.GetActiveScene().name == persistentScene.Name) return;

            SceneDataManagerEditorApplication.CloseScene(persistentScene, true);
        }
    }
}