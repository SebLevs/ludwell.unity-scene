using UnityEngine;
using UnityEngine.UIElements;

namespace Ludwell.Scene
{
    public class ViewLoaderController : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<ViewLoaderController, UxmlTraits> { }

        public const string Name = "view__loader";
        public const string ContentName = "content";

        private const string UxmlPath = "Uxml/" + Name;
        private const string UssPath = "Uss/" + Name;

        private const string MainMenuButtonName = "button__main-menu";

        private SceneLoaderListController _sceneLoaderListController;

        public ViewLoaderController()
        {
            this.SetHierarchyFromUxml(UxmlPath);
            this.AddStyleFromUss(UssPath);

            _sceneLoaderListController = this.Q<SceneLoaderListController>();
            InitMainMenuButton();
        }

        private void InitMainMenuButton()
        {
            this.Q(MainMenuButtonName).Q<Button>().clicked += () => { Debug.LogError("todo: load scene from here"); };
        }
    }
}