using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Ludwell.Scene
{
    public class LoaderController : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<LoaderController, UxmlTraits> { }

        private const string UxmlPath = "Uxml/" + nameof(LoaderController) + "/" + nameof(LoaderController);
        private const string UssPath = "Uss/" + nameof(LoaderController) + "/" + nameof(LoaderController);

        private const string MainMenuButtonsName = "main-menu__buttons";

        private SceneLoaderListController _sceneLoaderListController;
        private EditorSceneDataButtons _mainMenuButtons;

        public LoaderController()
        {
            this.AddHierarchyFromUxml(UxmlPath);
            this.AddStyleFromUss(UssPath);
            _sceneLoaderListController = this.Q<SceneLoaderListController>();
            InitMainMenuButtons();
        }

        private void InitMainMenuButtons()
        {
            _mainMenuButtons = this.Q<EditorSceneDataButtons>(MainMenuButtonsName);
            var objectField = this.Q("launcher__main-menu").Q<ObjectField>();
            _mainMenuButtons.AddAction(ButtonType.Load, () =>
            {
                if (objectField.value == null) return;
                SceneDataManager.LoadScene(objectField.value as SceneData);
            });

            _mainMenuButtons.AddAction(ButtonType.Open, () =>
            {
                if (objectField.value == null) return;
                SceneDataManager.OpenScene(objectField.value as SceneData);
            });
        }
    }
}