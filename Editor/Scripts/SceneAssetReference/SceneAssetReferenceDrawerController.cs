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

        // todo: make dark version & change path of AddToAddressablesDark
        private string ThemedIconAddToAddressables =>
            EditorGUIUtility.isProSkin ? SpritesPath.AddToAddressables : SpritesPath.AddToAddressablesDark;

        // todo: make dark version & change path of AddToAddressablesDark
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

            if (_reference?.objectReferenceValue)
            {
                buttonCount++;

                rect.x = content.x - EditorButton.Size;
                new EditorButton(rect, () => SelectInWindow(_guid))
                    .WithIcon(ThemedIconSelectInWindow)
                    .WithTooltip(SelectInWindowButtonTooltip)
                    .Build();
            }

            if (CanAddToBuildSettings(_reference, _guid))
            {
                buttonCount++;
                rect.x = content.x - EditorButton.Size * buttonCount - 2;

                new EditorButton(rect, () => AddToBuildSettings(_guid))
                    .WithIcon(ThemedIconAddToBuildSettings)
                    .WithTooltip(SceneElementView.AddBuildSettingsTooltip)
                    .Build();
            }

            if (CanEnableInBuildSettings(_reference, _guid))
            {
                buttonCount++;
                rect.x = content.x - EditorButton.Size * buttonCount - 2;

                new EditorButton(rect, () => EnableInBuildSettings(_guid))
                    .WithIcon(ThemedIconEnableInBuildSettings)
                    .WithTooltip(SceneElementView.EnableInBuildSettingsTooltip)
                    .Build();
            }

#if USE_ADDRESSABLES_EDITOR
            if (CanAddToAddressables(_reference, _guid))
            {
                buttonCount++;
                rect.x = content.x - EditorButton.Size * buttonCount - 2;

                new EditorButton(rect, () => AddToAddressables(_guid))
                    .WithIcon(ThemedIconAddToAddressables)
                    .WithTooltip(SceneElementView.AddtoAddressablesTooltip)
                    .Build();
            }
            else if (CanRemoveFromAddressables(_reference, _guid))
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

        private bool CanAddToBuildSettings(SerializedProperty referenceProperty, SerializedProperty guidProperty)
        {
            if (Application.isPlaying || referenceProperty.objectReferenceValue == null) return false;

            var data = SceneAssetDataBinders.Instance.GetDataFromId(guidProperty.stringValue);
            return !EditorSceneManagerHelper.IsSceneInBuildSettings(data.Path);
        }

        private bool CanEnableInBuildSettings(SerializedProperty referenceProperty, SerializedProperty guidProperty)
        {
            if (Application.isPlaying || referenceProperty.objectReferenceValue == null) return false;

            var data = SceneAssetDataBinders.Instance.GetDataFromId(guidProperty.stringValue);
            var isInBuildSetting = EditorSceneManagerHelper.IsSceneInBuildSettings(data.Path);
            var isEnabled = EditorSceneManagerHelper.IsSceneEnabledInBuildSettings(data.Path);

            return !data.IsAddressable && isInBuildSetting && !isEnabled;
        }

        private bool CanAddToAddressables(SerializedProperty referenceProperty, SerializedProperty guidProperty)
        {
            if (referenceProperty.objectReferenceValue == null) return false;

            var data = SceneAssetDataBinders.Instance.GetDataFromId(guidProperty.stringValue);
            return !data.IsAddressable;
        }

        private bool CanRemoveFromAddressables(SerializedProperty referenceProperty, SerializedProperty guidProperty)
        {
            if (referenceProperty.objectReferenceValue == null) return false;

            var data = SceneAssetDataBinders.Instance.GetDataFromId(guidProperty.stringValue);
            return data.IsAddressable;
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