using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ludwell.Scene
{
    public class LoaderViewController
    {
        private const string MainMenuButtonName = "button__main-menu";
        private const string CloseAllButtonName = "button__close-all";
        
        private ScenesListView _scenesListView;

        public void Init(VisualElement queryFrom)
        {
            InitScenesListView(queryFrom);
            InitMainMenuButton(queryFrom);
            InitCloseAllButton(queryFrom);
        }

        private void InitScenesListView(VisualElement queryFrom)
        {
            _scenesListView = new ScenesListView();
            _scenesListView.Init(queryFrom);
        }

        private void InitMainMenuButton(VisualElement queryFrom)
        {
            queryFrom.Q<Button>(MainMenuButtonName).clicked += () =>
            {
                Debug.LogError("todo: load scene from here");
            };
        }

        private void InitCloseAllButton(VisualElement queryFrom)
        {
            queryFrom.Q<ToolbarButton>(CloseAllButtonName).clicked += () =>
            {
                Debug.LogError("todo: close all elements from listview");
                _scenesListView.CloseAll();
            };
        }
    }
}