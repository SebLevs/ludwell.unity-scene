using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ludwell.Scene
{
    public class LoaderController : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<LoaderController, UxmlTraits> { }

        private const string UxmlPath = "Uxml/" + nameof(LoaderController) + "/" + nameof(LoaderController);
        private const string UssPath = "Uss/" + nameof(LoaderController) + "/" + nameof(LoaderController);

        private const string MainMenuButtonName = "button__main-menu";

        private SceneLoaderListController _sceneLoaderListController;

        public LoaderController()
        {
            this.SetHierarchyFromUxml(UxmlPath);
            this.AddStyleFromUss(UssPath);

            _sceneLoaderListController = this.Q<SceneLoaderListController>();
            InitMainMenuButton();
        }

        private void InitMainMenuButton()
        {
            this.Q(MainMenuButtonName).Q<Button>().clicked += () =>
            {
                Debug.LogError("todo: load scene from here");
                var objectField = this.Q("launcher__main-menu").Q<ObjectField>();
                // new SceneLoader().OpenScene(objectField.value as SceneData);
                new SceneLoader().LoadScene(objectField.value as SceneData);
            };
        }
    }

    public class SceneLoader
    {
        public void OpenScene(SceneData sceneData)
        {
            var path = AssetDatabase.GetAssetPath(sceneData.EditorSceneAsset);
            Debug.LogError(path);
            EditorSceneManager.OpenScene(path);
        }

        public void LoadScene(SceneData sceneData)
        {
            bool wasIsBuildSettings = true;
            if (!SceneIsInBuildSettings(sceneData.EditorSceneAsset))
            {
                AddSceneToBuildSettings(sceneData.EditorSceneAsset);
                wasIsBuildSettings = false;
            }

            OpenScene(sceneData);
            EditorApplication.isPlaying = true;

            if (wasIsBuildSettings) return;
            RemoveSceneFromBuildSettings(sceneData.EditorSceneAsset);
        }

        private bool SceneIsInBuildSettings(Object sceneAsset)
        {
            var scenePath = AssetDatabase.GetAssetPath(sceneAsset);
            var buildScenes = EditorBuildSettings.scenes;

            foreach (var buildScene in buildScenes)
            {
                if (string.Equals(buildScene.path, scenePath))
                {
                    return true;
                }
            }

            return false;
        }

        private void AddSceneToBuildSettings(Object sceneAsset)
        {
            var scenePath = AssetDatabase.GetAssetPath(sceneAsset);
            var buildScenes = EditorBuildSettings.scenes;

            ArrayUtility.Add(ref buildScenes, new EditorBuildSettingsScene(scenePath, true));
            EditorBuildSettings.scenes = buildScenes;
        }

        private void RemoveSceneFromBuildSettings(Object sceneAsset)
        {
            var scenePath = AssetDatabase.GetAssetPath(sceneAsset);
            var buildScenes = EditorBuildSettings.scenes;

            for (var i = 0; i < buildScenes.Length; i++)
            {
                if (!string.Equals(buildScenes[i].path, scenePath)) continue;

                ArrayUtility.RemoveAt(ref buildScenes, i);
                EditorBuildSettings.scenes = buildScenes;
                break;
            }
        }
    }
}