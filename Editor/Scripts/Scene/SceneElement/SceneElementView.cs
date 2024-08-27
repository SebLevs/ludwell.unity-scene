using System;
using Ludwell.UIToolkitElements.Editor;
using Ludwell.UIToolkitUtilities;
using UnityEngine.UIElements;

namespace Ludwell.Scene.Editor
{
    internal class SceneElementView : IDisposable
    {
        private readonly string _uxmlPath = FoldoutView.UxmlPath + "_SceneManagerToolkit";
        private readonly string _ussPath = FoldoutView.UssPath + "_SceneManagerToolkit";

        private const string OpenAdditiveTooltip = "Open additive";
        private const string RemoveAdditiveTooltip = "Remove additive";
        internal const string AddBuildSettingsTooltip = "Add to build settings";
        private const string RemoveBuildSettingsTooltip = "Remove from build settings";
        internal const string EnableInBuildSettingsTooltip = "Enable in build settings";
        private const string DisableInBuildSettingsTooltip = "Disable in build settings";
        internal const string AddtoAddressablesTooltip = "Add to default addressable group";
        internal const string RemoveFromAddressablesTooltip = "Remove from addressables";
        private const string AddressablesNotInstalledTooltip = "Addressables package not installed";

        private const string SetActiveButtonName = "button__set-active";
        private const string OpenSceneAdditiveButtonName = "button__open-additive";
        private const string OpenSceneButtonName = "button__open";
        private const string LoadButtonName = "button__load";
        private const string PingButtonName = "button__ping";
        private const string DirectoryChangeButtonName = "button__directory-path";
        private const string BuildSettingsButtonName = "button__build-settings";
        private const string EnabledInBuildSettingsButtonName = "button__build-settings__enabled";
        private const string AddressablesButtonName = "button__addressables";
        private const string IconAssetOutsideAssetsName = "icon__package-scene";

        private readonly VisualElement _iconAssetOutsideAssets;

        private readonly SceneElementController _root;

        public ButtonWithIcon SetActiveButton { get; }
        public DualStateButton OpenAdditiveButton { get; }
        public ButtonWithIcon OpenButton { get; }
        public DualStateButton LoadButton { get; }
        public ButtonWithIcon PingButton { get; private set; }
        public ButtonWithIcon DirectoryChangeButton { get; }
        public DualStateButton BuildSettingsButton { get; }
        public DualStateButton EnabledInBuildSettingsButton { get; }
        public DualStateButton AddressablesButton { get; }

        public void SetSetActiveButtonEnable(bool state) => SetActiveButton.SetEnabled(state);

        public void SetOpenAdditiveButtonEnable(bool state) => OpenAdditiveButton.SetEnabled(state);

        public void SetOpenButtonEnable(bool state) => OpenButton.SetEnabled(state);

        public void SetDirectoryChangeButtonEnable(bool state) => DirectoryChangeButton.SetEnabled(state);

        public void SetBuildSettingsButtonButtonEnable(bool state) => BuildSettingsButton.SetEnabled(state);

        public void SetEnabledInBuildSettingsButtonEnable(bool state) => EnabledInBuildSettingsButton.SetEnabled(state);

        public void SetAddressablesButtonEnable(bool state) => AddressablesButton.SetEnabled(state);

        public void SetPathTooltip(string path) => DirectoryChangeButton.tooltip = path;

        public void SetIconAssetOutsideAssets(bool state) =>
            _iconAssetOutsideAssets.style.display = state ? DisplayStyle.Flex : DisplayStyle.None;

        public SceneElementView(SceneElementController root)
        {
            _root = root;
            _root.AddHierarchyFromUxml(_uxmlPath);
            _root.AddStyleFromUss(_ussPath);

            SetActiveButton = _root.Q<ButtonWithIcon>(SetActiveButtonName);
            OpenAdditiveButton = _root.Q<DualStateButton>(OpenSceneAdditiveButtonName);
            OpenButton = _root.Q<ButtonWithIcon>(OpenSceneButtonName);
            LoadButton = _root.Q<DualStateButton>(LoadButtonName);
            PingButton = _root.Q<ButtonWithIcon>(PingButtonName);
            DirectoryChangeButton = _root.Q<ButtonWithIcon>(DirectoryChangeButtonName);
            BuildSettingsButton = _root.Q<DualStateButton>(BuildSettingsButtonName);
            EnabledInBuildSettingsButton = _root.Q<DualStateButton>(EnabledInBuildSettingsButtonName);
            AddressablesButton = _root.Q<DualStateButton>(AddressablesButtonName);

            _iconAssetOutsideAssets = _root.Q<VisualElement>(IconAssetOutsideAssetsName);
        }

        public void Dispose()
        {
            BuildSettingsButton.Dispose();
            OpenAdditiveButton.Dispose();
            LoadButton.Dispose();
        }

        public void SwitchOpenAdditiveButtonState(bool state)
        {
            OpenAdditiveButton.SwitchState(state ? OpenAdditiveButton.StateTwo : OpenAdditiveButton.StateOne);
            OpenAdditiveButton.tooltip = state ? RemoveAdditiveTooltip : OpenAdditiveTooltip;
        }

        public void SwitchAddressablesButtonState(bool state)
        {
            AddressablesButton.SwitchState(state ? AddressablesButton.StateTwo : AddressablesButton.StateOne);
            AddressablesButton.tooltip = state ? RemoveFromAddressablesTooltip : AddtoAddressablesTooltip;
        }

        public void SwitchBuildSettingsButtonState(bool state)
        {
            BuildSettingsButton.SwitchState(state ? BuildSettingsButton.StateTwo : BuildSettingsButton.StateOne);
            BuildSettingsButton.tooltip = state ? RemoveBuildSettingsTooltip : AddBuildSettingsTooltip;
        }

        public void SwitchEnabledInBuildSettingsButtonState(bool state)
        {
            EnabledInBuildSettingsButton.SwitchState(state
                ? EnabledInBuildSettingsButton.StateTwo
                : EnabledInBuildSettingsButton.StateOne);
            EnabledInBuildSettingsButton.tooltip = state ? DisableInBuildSettingsTooltip : EnableInBuildSettingsTooltip;
        }

        public void SetAddressableButtonTooltipWithoutPackage()
        {
            AddressablesButton.tooltip = AddressablesNotInstalledTooltip;
        }

        public void SwitchLoadButtonState(bool state)
        {
            LoadButton.SwitchState(state ? LoadButton.StateTwo : LoadButton.StateOne);
        }
    }
}
