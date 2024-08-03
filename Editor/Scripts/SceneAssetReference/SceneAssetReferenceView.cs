using System;
using System.IO;
using Ludwell.UIToolkitUtilities;
using Ludwell.UIToolkitUtilities.Editor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Ludwell.Scene.Editor
{
    public class SceneAssetReferenceView: IDisposable
    {
        private readonly string UxmlPath =
            Path.Combine("UI", nameof(SceneAssetReferenceView), "Uxml_" + nameof(SceneAssetReferenceView));

        private readonly string UssPath =
            Path.Combine("UI", nameof(SceneAssetReferenceView), "Uss_" + nameof(SceneAssetReferenceView));

        private const string HelpBoxName = "help-box";
        
        private ThemeManagerEditor _themeManagerEditor;

        private readonly Label _objectFieldLabel;

        public VisualElement HelpBox { get; }
        public Button HelpBoxButton { get; private set; }
        public ObjectField ObjectField { get; }

        public SceneAssetReferenceView(VisualElement root)
        {
            root.AddHierarchyFromUxml(UxmlPath);
            root.AddStyleFromUss(UssPath);

            ObjectField = root.Q<ObjectField>();
            _objectFieldLabel = ObjectField.Q<Label>();
            
            HelpBox = root.Q<VisualElement>(HelpBoxName);
            HideHelpBox();
            
            HelpBoxButton = HelpBox.Q<Button>();
            
            root.RegisterCallback<DetachFromPanelEvent>(Dispose);
            root.RegisterCallback<AttachToPanelEvent>(CacheThemeManager);
        }

        public void Dispose() => Dispose(null);

        public void Dispose(DetachFromPanelEvent _)
        {
            _themeManagerEditor.Dispose();
        }
        
        public void ShowHelpBox()
        {
            HelpBox.style.display = DisplayStyle.Flex;
        }

        public void HideHelpBox()
        {
            HelpBox.style.display = DisplayStyle.None;
        }
        
        public void SetObjectFieldLabel(string value)
        {
            _objectFieldLabel.text = value;
        }
        
        private void CacheThemeManager(AttachToPanelEvent evt)
        {
            var lightTheme = DefaultThemes.GetLightThemeStyleSheet();
            var darkTheme = DefaultThemes.GetDarkThemeStyleSheet();
            _themeManagerEditor = new ThemeManagerEditor(evt.target as VisualElement, darkTheme, lightTheme);
        }
    }
}