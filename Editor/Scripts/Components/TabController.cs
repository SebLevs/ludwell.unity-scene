using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Ludwell.Scene
{
    public class TabController : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<TabController, UxmlTraits> { }

        private const string UxmlPath = "Uxml/" + nameof(SceneDataEditorWindow) + "/" + nameof(TabController);
        private const string UssPath = "Uss/" + nameof(SceneDataEditorWindow) + "/" + nameof(TabController);

        private VisualElement _previousView;
        private VisualElement _currentView;
        
        private const string TabManagerName = "tab__manager";
        // private const string ViewManagerName = "view__manager";
        private const string TabLoaderName = "tab__loader";
        // private const string ViewLoaderName = "view__loader";
        private const string TabSettingsName = "tab__settings";
        // private const string ViewSettingsName = "view__settings";
        
        public TabController()
        {
            this.AddHierarchyFromUxml(UxmlPath);
            this.AddStyleFromUss(UssPath);

            RegisterCallback<AttachToPanelEvent>(_ =>
            {
                var root = this.Root();
                BindTabToManagerView(root);
                _currentView = BindTabToLoaderView(root);
                BindTabToSettingsView(root);

                if (_currentView == null) return;
                SetStartingView(_currentView);
            });
        }
        
        public void SwitchView(VisualElement view)
        {
            if (_currentView == view) return;

            _previousView = _currentView;

            if (_currentView != null)
            {
                _currentView.style.display = DisplayStyle.None;
            }

            _currentView = view;
            _currentView.style.display = DisplayStyle.Flex;
        }

        public void ReturnToPreviousView()
        {
            if (_previousView == null) return;
            
            SwitchView(_previousView);
            _previousView = null;
        }

        private VisualElement BindTabToManagerView(VisualElement queryFrom)
        {
            var view = queryFrom.Q<ManagerController>(nameof(ManagerController));
            var tab = this.Q<ToolbarButton>(TabManagerName);
            tab.clicked += () => { SwitchView(view); };
            return view;
        }

        private VisualElement BindTabToLoaderView(VisualElement queryFrom)
        {
            var view = queryFrom.Q<LoaderController>();
            var tab = this.Q<ToolbarButton>(TabLoaderName);
            tab.clicked += () => { SwitchView(view); };
            return view;
        }

        private VisualElement BindTabToSettingsView(VisualElement queryFrom)
        {
            var view = queryFrom.Q<SettingsController>();
            var tab = this.Q<ToolbarButton>(TabSettingsName);
            tab.clicked += () => { SwitchView(view); };
            return view;
        }
        
        private void SetStartingView(VisualElement view)
        {
            SwitchView(view);
        }
    }
}