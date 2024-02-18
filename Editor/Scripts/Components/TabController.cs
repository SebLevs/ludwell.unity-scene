using System.IO;
using UnityEngine.UIElements;

namespace Ludwell.Scene
{
    public class TabController : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<TabController, UxmlTraits> { }

        private static readonly string UxmlPath = Path.Combine("Uxml", nameof(SceneDataEditorWindow), nameof(TabController));
        private static readonly string UssPath = Path.Combine("Uss", nameof(SceneDataEditorWindow), nameof(TabController));

        private VisualElement _previousView;
        private VisualElement _currentView;
        
        public TabController()
        {
            this.AddHierarchyFromUxml(UxmlPath);
            this.AddStyleFromUss(UssPath);

            RegisterCallback<AttachToPanelEvent>(_ =>
            {
                var root = this.Root();
                _currentView = BindTabToLoaderView(root);

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

        private VisualElement BindTabToLoaderView(VisualElement queryFrom)
        {
            var view = queryFrom.Q<LoaderController>();
            return view;
        }

        private void SetStartingView(VisualElement view)
        {
            SwitchView(view);
        }
    }
}