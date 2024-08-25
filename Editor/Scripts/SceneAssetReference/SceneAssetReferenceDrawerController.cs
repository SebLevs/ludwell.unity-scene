using Ludwell.EditorUtilities.Editor;
using Ludwell.UIToolkitUtilities.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ludwell.Scene.Editor
{
    public class SceneAssetReferenceDrawerController
    {
        private const string SceneAssetPropertyName = "_reference";
        private const string GuidPropertyName = "_guid";

        private const string SelectInWindowButtonTooltip = "Select in Scene Manager Toolkit window";

        private readonly SerializedProperty _reference;
        private readonly SerializedProperty _guid;

        private string ThemedIconSelectInWindow =>
            EditorGUIUtility.isProSkin ? SpritesPath.Settings : SpritesPath.SettingsDark;

        private string ThemedIconAddToBuildSettings =>
            EditorGUIUtility.isProSkin ? SpritesPath.AddToBuildSettings : SpritesPath.AddToBuildSettingsDark;

        private string ThemedIconEnableInBuildSettings =>
            EditorGUIUtility.isProSkin ? SpritesPath.EnableInBuildSettings : SpritesPath.EnableInBuildSettingsDark;

        private string ThemedIconAddToAddressables =>
            EditorGUIUtility.isProSkin ? SpritesPath.AddToAddressables : SpritesPath.AddToAddressablesDark;

        private string ThemedIconRemoveFromAddressables =>
            EditorGUIUtility.isProSkin ? SpritesPath.RemoveFromAddressables : SpritesPath.RemoveFromAddressablesDark;

        public SceneAssetReferenceDrawerController(Rect content, SerializedProperty rootProperty)
        {
            _reference = rootProperty.FindPropertyRelative(SceneAssetPropertyName);
            _guid = rootProperty.FindPropertyRelative(GuidPropertyName);
            InitializeButtons(content);
        }

        private void InitializeButtons(Rect content)
        {
            var centeredY = content.y + (content.height - EditorButton.Size) * 0.5f;
            var buttonCount = 0;

            var rect = new Rect(content.x - EditorButton.Size, centeredY, EditorButton.Size, EditorButton.Size);
            
            var data = SceneAssetDataBinders.Instance.GetDataFromId(_guid.stringValue);

            if (_reference?.objectReferenceValue)
            {
                buttonCount++;

                rect.x = content.x - EditorButton.Size;
                new EditorButton(rect, () => SelectInWindow(_guid))
                    .WithIcon(ThemedIconSelectInWindow)
                    .WithTooltip(SelectInWindowButtonTooltip)
                    .Build();
            }

            if (CanAddToBuildSettings(data))
            {
                buttonCount++;
                rect.x = content.x - EditorButton.Size * buttonCount - 2;

                new EditorButton(rect, () => AddToBuildSettings(_guid))
                    .WithIcon(ThemedIconAddToBuildSettings)
                    .WithTooltip(SceneElementView.AddBuildSettingsTooltip)
                    .Build();
            }

            if (CanEnableInBuildSettings(data))
            {
                buttonCount++;
                rect.x = content.x - EditorButton.Size * buttonCount - 2;

                new EditorButton(rect, () => EnableInBuildSettings(_guid))
                    .WithIcon(ThemedIconEnableInBuildSettings)
                    .WithTooltip(SceneElementView.EnableInBuildSettingsTooltip)
                    .Build();
            }

#if USE_ADDRESSABLES_EDITOR
            if (CanAddToAddressables(data))
            {
                buttonCount++;
                rect.x = content.x - EditorButton.Size * buttonCount - 2;

                new EditorButton(rect, () => AddToAddressables(_guid))
                    .WithIcon(ThemedIconAddToAddressables)
                    .WithTooltip(SceneElementView.AddtoAddressablesTooltip)
                    .Build();
            }
            else if (CanRemoveFromAddressables(data))
            {
                buttonCount++;
                rect.x = content.x - EditorButton.Size * buttonCount - 2;

                new EditorButton(rect, () => RemoveFromAddressables(_guid))
                    .WithIcon(ThemedIconRemoveFromAddressables)
                    .WithTooltip(SceneElementView.RemoveFromAddressablesTooltip)
                    .Build();
            }
#endif
        }

        private bool CanAddToBuildSettings(SceneAssetData data)
        {
            if (Application.isPlaying || data == null) return false;

            return !EditorSceneManagerHelper.IsSceneInBuildSettings(data.Path);
        }

        private bool CanEnableInBuildSettings(SceneAssetData data)
        {
            if (Application.isPlaying || data == null) return false;

            var isInBuildSetting = EditorSceneManagerHelper.IsSceneInBuildSettings(data.Path);
            var isEnabled = EditorSceneManagerHelper.IsSceneEnabledInBuildSettings(data.Path);

            return !data.IsAddressable && isInBuildSetting && !isEnabled;
        }

        private bool CanAddToAddressables(SceneAssetData data)
        {
            return data is { IsAddressable: false };
        }

        private bool CanRemoveFromAddressables(SceneAssetData data)
        {
            return data is { IsAddressable: true };
        }

        private void SelectInWindow(SerializedProperty guidProperty)
        {
            var binderToSelect = ResourcesLocator.GetSceneAssetDataBinders().GetBinderFromId(guidProperty.stringValue);
            var index = SceneAssetDataBinders.Instance.IndexOf(binderToSelect);
            var window = EditorWindow.GetWindow<SceneManagerToolkitWindow>();
            window.Focus();

            var viewManager = window.rootVisualElement.Q<ViewManager>();
            viewManager.TransitionToFirstViewOfType<SceneElementsController>();

            window.rootVisualElement.schedule.Execute(() =>
            {
                window.SceneElementsController.ScrollToItemIndex(index);
            });
        }

        private void AddToBuildSettings(SerializedProperty guidProperty)
        {
            var data = SceneAssetDataBinders.Instance.GetDataFromId(guidProperty.stringValue);
            EditorSceneManagerHelper.AddSceneToBuildSettings(data.Path);
        }

        private void EnableInBuildSettings(SerializedProperty guidProperty)
        {
            var data = SceneAssetDataBinders.Instance.GetDataFromId(guidProperty.stringValue);
            EditorSceneManagerHelper.EnableSceneInBuildSettings(data.Path, true);
        }

#if USE_ADDRESSABLES_EDITOR
        private void AddToAddressables(SerializedProperty guidProperty)
        {
            AddressablesProcessor.AddToAddressables(guidProperty.stringValue);
        }

        public void RemoveFromAddressables(SerializedProperty guidProperty)
        {
            var data = SceneAssetDataBinders.Instance.GetDataFromId(guidProperty.stringValue);
            AddressablesProcessor.RemoveFromAddressables(data.AddressableID);
        }
#endif
    }
}