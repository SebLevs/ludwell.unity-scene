using Ludwell.UIToolkitElements.Editor;
using Ludwell.UIToolkitUtilities;
using UnityEngine.UIElements;

namespace Ludwell.Scene.Editor
{
    public class QuickLoadElementView
    {
        private readonly string UxmlPath = FoldoutView.UxmlPath + "_SceneManagerToolkit";
        private readonly string UssPath = FoldoutView.UssPath + "_SceneManagerToolkit";
        
        private const string AddBuildSettingsTooltip = "Add to build settings";
        private const string RemoveBuildSettingsTooltip = "Remove from build settings";
        private const string OpenAdditiveTooltip = "Open additive";
        private const string RemoveAdditiveTooltip = "Remove additive";

        private const string SetActiveButtonName = "button__set-active";
        private const string BuildSettingsButtonName = "button__build-settings";
        private const string OpenSceneAdditiveButtonName = "button__open-additive";
        private const string OpenSceneButtonName = "button__open";
        private const string LoadButtonName = "button__load";
        private const string PingButtonName = "button__ping";
        private const string DirectoryChangeButtonName = "button__directory-path";
        private const string IconAssetOutsideAssetsName = "icon__package-scene";

        private readonly VisualElement _iconAssetOutsideAssets;

        private readonly QuickLoadElementController _root;

        public ButtonWithIcon SetActiveButton { get; }
        public DualStateButton BuildSettingsButton { get; }
        public DualStateButton OpenAdditiveButton { get; }
        public ButtonWithIcon OpenButton { get; }
        public DualStateButton LoadButton { get; }
        public ButtonWithIcon PingButton { get; private set; }
        public ButtonWithIcon DirectoryChangeButton { get; }

        public void SetSetActiveButtonEnable(bool state) => SetActiveButton.SetEnabled(state);

        public void SetOpenAdditiveButtonEnable(bool state) => OpenAdditiveButton.SetEnabled(state);

        public void SetOpenButtonEnable(bool state) => OpenButton.SetEnabled(state);

        public void SetDirectoryChangeButtonEnable(bool state) => DirectoryChangeButton.SetEnabled(state);

        public void SetBuildSettingsButtonButtonEnable(bool state) => BuildSettingsButton.SetEnabled(state);

        public void SetPathTooltip(string path) => DirectoryChangeButton.tooltip = path;

        public void SetIconAssetOutsideAssets(bool state) =>
            _iconAssetOutsideAssets.style.display = state ? DisplayStyle.Flex : DisplayStyle.None;

        public QuickLoadElementView(QuickLoadElementController root)
        {
            _root = root;
            _root.AddHierarchyFromUxml(UxmlPath);
            _root.AddStyleFromUss(UssPath);

            SetActiveButton = _root.Q<ButtonWithIcon>(SetActiveButtonName);
            OpenAdditiveButton = _root.Q<DualStateButton>(OpenSceneAdditiveButtonName);
            OpenButton = _root.Q<ButtonWithIcon>(OpenSceneButtonName);
            LoadButton = _root.Q<DualStateButton>(LoadButtonName);
            PingButton = _root.Q<ButtonWithIcon>(PingButtonName);
            DirectoryChangeButton = _root.Q<ButtonWithIcon>(DirectoryChangeButtonName);
            BuildSettingsButton = _root.Q<DualStateButton>(BuildSettingsButtonName);

            _iconAssetOutsideAssets = _root.Q<VisualElement>(IconAssetOutsideAssetsName);
        }

        public void SwitchBuildSettingsButtonState(bool state)
        {
            BuildSettingsButton.SwitchState(state ? BuildSettingsButton.StateTwo : BuildSettingsButton.StateOne);
            BuildSettingsButton.tooltip = state ? RemoveBuildSettingsTooltip : AddBuildSettingsTooltip;
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