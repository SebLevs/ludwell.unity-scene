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
        private const string AddToBuildSettingsButtonTooltip = "Add to Build Settings";
        private const string EnableInBuildSettingsButtonTooltip = "Enable in Build Settings";

        private readonly SerializedProperty _reference;
        private readonly SerializedProperty _guid;

        private string ThemedIconSelectInWindow =>
            EditorGUIUtility.isProSkin ? SpritesPath.Settings : SpritesPath.SettingsDark;

        private string ThemedIconAddToBuildSettingsIcon =>
            EditorGUIUtility.isProSkin ? SpritesPath.AddToBuildSettings : SpritesPath.AddToBuildSettingsDark;

        private string ThemedIconEnableInBuildSettingsIcon =>
            EditorGUIUtility.isProSkin ? SpritesPath.EnableInBuildSettings : SpritesPath.EnableInBuildSettingsDark;

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
                    .WithIcon(ThemedIconAddToBuildSettingsIcon)
                    .WithTooltip(AddToBuildSettingsButtonTooltip)
                    .Build();
            }

            if (CanEnableInBuildSettings(_reference, _guid))
            {
                buttonCount++;
                rect.x = content.x - EditorButton.Size * buttonCount - 2;

                new EditorButton(rect, () => EnableInBuildSettings(_guid))
                    .WithIcon(ThemedIconEnableInBuildSettingsIcon)
                    .WithTooltip(EnableInBuildSettingsButtonTooltip)
                    .Build();
            }
        }

        private bool CanAddToBuildSettings(SerializedProperty referenceProperty, SerializedProperty guidProperty)
        {
            if (Application.isPlaying || referenceProperty.objectReferenceValue == null) return false;

            var data = SceneAssetDataBinders.Instance.GetDataFromId(guidProperty.stringValue);
            if (data.IsAddressable) return false;

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
    }
}