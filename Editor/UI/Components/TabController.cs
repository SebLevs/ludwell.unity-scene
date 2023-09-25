using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Ludwell.Scene
{
    public class TabController
    {
        private VisualElement _currentElement;
        private const string TabManagerName = "tab__manager";
        private const string ViewManagerName = "view__manager";
        private const string TabLoaderName = "tab__loader";
        private const string ViewLoaderName = "view__loader";
        private const string TabSettingsName = "tab__settings";
        private const string ViewSettingsName = "view__settings";
        
        public void Init(VisualElement queryFrom)
        {
            InitManager(queryFrom);
            InitLoader(queryFrom);
            InitSettings(queryFrom);
        }

        private void InitManager(VisualElement queryFrom)
        {
            var view = queryFrom.Q<VisualElement>(ViewManagerName);
            // todo: set up load view class
            var tab = queryFrom.Q<ToolbarButton>(TabManagerName);
            tab.RegisterCallback<ClickEvent>(_ => { SwitchView(view); });
        }
        
        private void InitLoader(VisualElement queryFrom)
        {
            var view = queryFrom.Q<VisualElement>(ViewLoaderName);
            // todo: set up load view class
            var tab = queryFrom.Q<ToolbarButton>(TabLoaderName);
            tab.RegisterCallback<ClickEvent>(_ => { SwitchView(view); });
            
            SwitchView(view);
        }
        
        private void InitSettings(VisualElement queryFrom)
        {
            var view = queryFrom.Q<VisualElement>(ViewSettingsName);
            // todo: set up settings view class
            var tab = queryFrom.Q<ToolbarButton>(TabSettingsName);
            tab.RegisterCallback<ClickEvent>(_ => { SwitchView(view); });
        }

        private void SwitchView(VisualElement view)
        {
            if (_currentElement == view) return;

            if (_currentElement != null)
            {
                _currentElement.style.display = DisplayStyle.None;
                // todo: cleanup old view class here
            }

            _currentElement = view;
            _currentElement.style.display = DisplayStyle.Flex;
            // todo: set up new view class here
        }
    }
}