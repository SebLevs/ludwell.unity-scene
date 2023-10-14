using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ludwell.Scene
{
    public class LoaderViewController
    {
        private const string MainMenuButtonName = "button__main-menu";
        private const string CloseAllButtonName = "button__close-all";

        private ScenesListViewController _scenesListViewController;

        public LoaderViewController(VisualElement queryFrom)
        {
            _scenesListViewController = new ScenesListViewController(queryFrom);
            InitMainMenuButton(queryFrom);
            InitCloseAllButton(queryFrom);
        }

        private void InitMainMenuButton(VisualElement queryFrom)
        {
            queryFrom.Q(MainMenuButtonName).Q<Button>().clicked += () =>
            {
                Debug.LogError("todo: load scene from here");
            };
        }

        private void InitCloseAllButton(VisualElement queryFrom)
        {
            queryFrom.Q<ToolbarButton>(CloseAllButtonName).clicked += () => { _scenesListViewController.CloseAll(); };
        }
    }
}