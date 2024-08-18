using System;
using System.IO;
using Ludwell.UIToolkitUtilities;
using Ludwell.UIToolkitUtilities.Editor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Ludwell.Scene.Editor
{
    public class SceneAssetReferenceView : IDisposable
    {
        public Label ObjectFieldLabel;

        private readonly string UxmlPath =
            Path.Combine("UI", nameof(SceneAssetReferenceView), "Uxml_" + nameof(SceneAssetReferenceView));

        private readonly string UssPath =
            Path.Combine("UI", nameof(SceneAssetReferenceView), "Uss_" + nameof(SceneAssetReferenceView));

        // private const string BuildSettingsButtonName = "build-settings";
        // private const string EnableInBuildSettingsButtonName = "build-settings__enabled";
        private const string SelectInWindowButtonName = "select-in-window";

        private ThemeManagerEditor _themeManagerEditor;

        public ObjectField ObjectField { get; }

        public Button BuildSettingsButton { get; }
        public Button EnableInBuildSettingsButton { get; }

        public Button SelectInWindowButton { get; }

        public SceneAssetReferenceView(VisualElement root)
        {
            root.AddHierarchyFromUxml(UxmlPath);
            root.AddStyleFromUss(UssPath);

            ObjectField = root.Q<ObjectField>();
            ObjectField.AddToClassList(ObjectField.alignedFieldUssClassName);

            ObjectFieldLabel = ObjectField.Q<Label>();

            BuildSettingsButton = root.Q<Button>(SceneElementView.BuildSettingsButtonName);
            BuildSettingsButton.tooltip = SceneElementView.AddBuildSettingsTooltip;
            EnableInBuildSettingsButton = root.Q<Button>(SceneElementView.EnabledInBuildSettingsButtonName);
            EnableInBuildSettingsButton.tooltip = SceneElementView.EnableInBuildSettingsTooltip;
            SelectInWindowButton = root.Q<Button>(SelectInWindowButtonName);

            root.RegisterCallback<DetachFromPanelEvent>(Dispose);
            root.RegisterCallback<AttachToPanelEvent>(CacheThemeManager);
        }

        public void Dispose() => Dispose(null);

        public void Dispose(DetachFromPanelEvent _)
        {
            _themeManagerEditor.Dispose();
        }

        public bool AreButtonsHidden()
        {
            return BuildSettingsButton.style.display == DisplayStyle.None &&
                   SelectInWindowButton.style.display == DisplayStyle.None;
        }

        public void ShowBuildSettingsButton()
        {
            BuildSettingsButton.style.display = DisplayStyle.Flex;
        }

        public void HideBuildSettingsButton()
        {
            BuildSettingsButton.style.display = DisplayStyle.None;
        }

        public void ShowEnableInBuildSettingsButton()
        {
            EnableInBuildSettingsButton.style.display = DisplayStyle.Flex;
        }

        public void HideEnableInBuildSettingsButton()
        {
            EnableInBuildSettingsButton.style.display = DisplayStyle.None;
        }

        public void ShowSelectInWindowButton()
        {
            SelectInWindowButton.style.display = DisplayStyle.Flex;
        }

        public void HideSelectInWindowButton()
        {
            SelectInWindowButton.style.display = DisplayStyle.None;
        }

        private void CacheThemeManager(AttachToPanelEvent evt)
        {
            var lightTheme = DefaultThemes.GetLightThemeStyleSheet();
            var darkTheme = DefaultThemes.GetDarkThemeStyleSheet();
            _themeManagerEditor = new ThemeManagerEditor(evt.target as VisualElement, darkTheme, lightTheme);
        }
    }
}