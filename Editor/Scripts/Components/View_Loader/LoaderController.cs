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

        private const string MainMenuButtonName = "button__main-menu__";
        private const string MainMenuButtonLoadName = MainMenuButtonName + "load";
        private const string MainMenuButtonOpenName = MainMenuButtonName + "open";

        private SceneLoaderListController _sceneLoaderListController;

        public LoaderController()
        {
            this.SetHierarchyFromUxml(UxmlPath);
            this.AddStyleFromUss(UssPath);

            _sceneLoaderListController = this.Q<SceneLoaderListController>();
            InitMainMenuButtons();
        }

        private void InitMainMenuButtons()
        {
            this.Q(MainMenuButtonLoadName).Q<Button>().clicked += () =>
            {
                Debug.LogError("todo: load scene from here");
                var objectField = this.Q("launcher__main-menu").Q<ObjectField>();
                if (objectField.value == null) return;
                SceneDataManager.LoadScene(objectField.value as SceneData);
            };

            this.Q(MainMenuButtonOpenName).Q<Button>().clicked += () =>
            {
                Debug.LogError("todo: open scene from here");
                var objectField = this.Q("launcher__main-menu").Q<ObjectField>();
                if (objectField.value == null) return;
                SceneDataManager.OpenScene(objectField.value as SceneData);
            };
        }
    }
}