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

        private const string LoaderSceneDataPath = "Scriptables/" + nameof(LoaderSceneData);
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
            var loaderSceneData = Resources.Load<LoaderSceneData>(LoaderSceneDataPath);

            var mainMenuObjectField = this.Q(MainMenuObjectFieldName).Q<ObjectField>();
            if (loaderSceneData.MainMenuScene != null && loaderSceneData.MainMenuScene.EditorSceneAsset != null)
            {
                mainMenuObjectField.value = loaderSceneData.MainMenuScene;
            }

            mainMenuObjectField.RegisterValueChangedCallback(evt =>
            {
                loaderSceneData.MainMenuScene = evt.newValue as SceneData;
            });

            var persistentSceneObjectField = this.Q(PersistentObjectFieldName).Q<ObjectField>();
            if (loaderSceneData.PersistentScene != null && loaderSceneData.PersistentScene.EditorSceneAsset != null)
            {
                persistentSceneObjectField.value = loaderSceneData.PersistentScene;
            }

            persistentSceneObjectField.RegisterValueChangedCallback(evt =>
            {
                loaderSceneData.PersistentScene = evt.newValue as SceneData;
            });

            var loadingObjectField = this.Q(LoadingObjectFieldName).Q<ObjectField>();
            if (loaderSceneData.LoadingScene != null && loaderSceneData.LoadingScene.EditorSceneAsset != null)
            {
                loadingObjectField.value = loaderSceneData.LoadingScene;
            }

            loadingObjectField.RegisterValueChangedCallback(evt =>
            {
                loaderSceneData.LoadingScene = evt.newValue as SceneData;
            });
        }

        private void InitMainMenuButtons()
        {
            var mainMenuButtons = this.Q<EditorSceneDataButtons>(MainMenuButtonsName);
            var objectField = this.Q(MainMenuObjectFieldName).Q<ObjectField>();

            mainMenuButtons.AddAction(ButtonType.Load, () =>
            {
                if (objectField.value == null) return;
                SceneDataManager.LoadScene(objectField.value as SceneData);
            });

            mainMenuButtons.AddAction(ButtonType.Open, () =>
            {
                if (objectField.value == null) return;
                SceneDataManager.OpenScene(objectField.value as SceneData);
            });
        }
    }
}