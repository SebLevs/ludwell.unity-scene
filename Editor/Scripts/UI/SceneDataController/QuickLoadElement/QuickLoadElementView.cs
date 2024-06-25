using System.IO;
using UnityEngine.UIElements;

namespace Ludwell.Scene.Editor
{
    public class QuickLoadElementView
    {
        private static readonly string UxmlPath =
            Path.Combine("UI", "Foldout", "Uxml_" + "Foldout");

        private static readonly string UssPath =
            Path.Combine("UI", "Foldout", "Uss_" + "Foldout");

        private const string AddBuildSettingsTooltip = "Add to build settings";
        private const string RemoveBuildSettingsTooltip = "Remove from build settings";
        private const string OpenAdditiveTooltip = "Open additive";
        private const string RemoveAdditiveTooltip = "Remove additive";

        private const string BuildSettingsButtonName = "button__build-settings";
        private const string OpenSceneAdditiveButtonName = "button__open-additive";
        private const string OpenSceneButtonName = "button__open";
        private const string LoadButtonName = "button__load";
        private const string PingButtonName = "button__ping";
        private const string DirectoryChangeButtonName = "button__directory-path";
        private const string IconAssetOutsideAssetsName = "icon__package-scene";

        private VisualElement _iconAssetOutsideAssets;

        public readonly DualStateButton BuildSettingsButton;
        public readonly DualStateButton OpenAdditiveButton;
        public readonly ButtonWithIcon OpenButton;
        public readonly DualStateButton LoadButton;
        public readonly ButtonWithIcon PingButton;
        public readonly ButtonWithIcon DirectoryChangeButton;

        private QuickLoadElementController _root;

        public void SetIconAssetOutsideAssets(bool state) =>
            _iconAssetOutsideAssets.style.display = state ? DisplayStyle.Flex : DisplayStyle.None;

        public void SetPathTooltip(string path) => DirectoryChangeButton.tooltip = path;

        public void SetDirectoryChangeButtonEnable(bool state) => DirectoryChangeButton.SetEnabled(state);

        public void SetOpenButtonEnable(bool state) => OpenButton.SetEnabled(state);

        public void SetOpenAdditiveButtonEnable(bool state) => OpenAdditiveButton.SetEnabled(state);

        public QuickLoadElementView(QuickLoadElementController root)
        {
            _root = root;
            _root.AddHierarchyFromUxml(UxmlPath);
            _root.AddStyleFromUss(UssPath);

            BuildSettingsButton = _root.Q<DualStateButton>(BuildSettingsButtonName);
            OpenAdditiveButton = _root.Q<DualStateButton>(OpenSceneAdditiveButtonName);
            OpenButton = _root.Q<ButtonWithIcon>(OpenSceneButtonName);
            LoadButton = _root.Q<DualStateButton>(LoadButtonName);
            PingButton = _root.Q<ButtonWithIcon>(PingButtonName);
            DirectoryChangeButton = _root.Q<ButtonWithIcon>(DirectoryChangeButtonName);

            _iconAssetOutsideAssets = _root.Q<VisualElement>(IconAssetOutsideAssetsName);
        }

        public void SwitchOpenAdditiveButtonState(bool state)
        {
            OpenAdditiveButton.SwitchState(state ? OpenAdditiveButton.StateTwo : OpenAdditiveButton.StateOne);
            OpenAdditiveButton.tooltip = state ? RemoveAdditiveTooltip : OpenAdditiveTooltip;
        }

        public void SwitchLoadButtonState(bool state)
        {
            LoadButton.SwitchState(state ? LoadButton.StateTwo : LoadButton.StateOne);
        }
    }
}