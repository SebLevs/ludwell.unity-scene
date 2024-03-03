using System.IO;
using Ludwell.Scene.Editor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ludwell.Scene
{
    public class LoaderController : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<LoaderController, UxmlTraits>
        {
        }

        private static readonly string UxmlPath =
            Path.Combine("Uxml", nameof(LoaderController), nameof(LoaderController));

        private static readonly string
            UssPath = Path.Combine("Uss", nameof(LoaderController), nameof(LoaderController));

        private const string MainMenuButtonsName = "main-menu__buttons";
        private const string MainMenuObjectFieldName = "launcher__main-menu";
        private const string PersistentObjectFieldName = "core-scene__persistent";
        private const string LoadingObjectFieldName = "core-scene__loading";

        public LoaderController()
        {
            this.AddHierarchyFromUxml(UxmlPath);
            this.AddStyleFromUss(UssPath);
            BindCoreScenesToData();
            InitMainMenuButtons();
        }

        private void BindCoreScenesToData()
        {
            var coreScenes = DataFetcher.GetCoreScenes();

            var mainMenuObjectField = this.Q(MainMenuObjectFieldName).Q<ObjectField>();
            if (coreScenes.LaunchScene != null)
            {
                mainMenuObjectField.value = coreScenes.LaunchScene;
            }

            mainMenuObjectField.RegisterValueChangedCallback(evt =>
            {
                coreScenes.LaunchScene = evt.newValue as SceneData;
                DataFetcher.SaveEveryScriptable();
            });

            var persistentSceneObjectField = this.Q(PersistentObjectFieldName).Q<ObjectField>();
            if (coreScenes.PersistentScene != null)
            {
                persistentSceneObjectField.value = coreScenes.PersistentScene;
            }

            persistentSceneObjectField.RegisterValueChangedCallback(evt =>
            {
                coreScenes.PersistentScene = evt.newValue as SceneData;
                DataFetcher.SaveEveryScriptable();
            });

            var loadingObjectField = this.Q(LoadingObjectFieldName).Q<ObjectField>();
            if (coreScenes.LoadingScene != null)
            {
                loadingObjectField.value = coreScenes.LoadingScene;
            }

            loadingObjectField.RegisterValueChangedCallback(evt =>
            {
                coreScenes.LoadingScene = evt.newValue as SceneData;
                DataFetcher.SaveEveryScriptable();
            });
        }

        private void InitMainMenuButtons()
        {
            var mainMenuButtons = this.Q<EditorSceneDataButtons>(MainMenuButtonsName);
            var objectField = this.Q(MainMenuObjectFieldName).Q<ObjectField>();

            mainMenuButtons.AddAction(ButtonType.Load, () =>
            {
                if (objectField.value == null) return;
                SceneDataManagerEditorApplication.LoadScene(objectField.value as SceneData);
            });

            mainMenuButtons.AddAction(ButtonType.Open, () =>
            {
                if (objectField.value == null) return;
                SceneDataManagerEditorApplication.OpenScene(objectField.value as SceneData);
            });
        }
    }
}