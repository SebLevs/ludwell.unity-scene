using Ludwell.EditorUtilities.Editor;
using Ludwell.UIToolkitUtilities.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ludwell.Scene.Editor
{
    public class SceneAssetReferenceDrawerController
    {
        private const string SelectInWindowButtonTooltip = "Select in Scene Manager Toolkit window";
        private const string AddToBuildSettingsButtonTooltip = "Add to Build Settings";
        private const string EnableInBuildSettingsButtonTooltip = "Enable in Build Settings";

        public SceneAssetReferenceDrawerController(Rect content, SerializedProperty reference, SerializedProperty guid)
        {
            InitializeButtons(content, reference, guid);
        }

        private void InitializeButtons(Rect content, SerializedProperty reference, SerializedProperty guid)
        {
            var centeredY = content.y + (content.height - EditorButton.Size) * 0.5f;
            var buttonCount = 0;

            var rect = new Rect(content.x - EditorButton.Size, centeredY, EditorButton.Size, EditorButton.Size);

            if (reference?.objectReferenceValue)
            {
                buttonCount++;

                rect.x = content.x - EditorButton.Size;
                new EditorButton(rect, () => SelectInWindow(guid)).WithIcon(SpritesPath.Settings)
                    .WithTooltip(SelectInWindowButtonTooltip).Build();
            }

            if (CanAddToBuildSettings(reference, guid))
            {
                buttonCount++;
                rect.x = content.x - EditorButton.Size * buttonCount - 2;

                new EditorButton(rect, () => AddToBuildSettings(guid)).WithIcon(SpritesPath.AddBuildSettings)
                    .WithTooltip(AddToBuildSettingsButtonTooltip).Build();
            }

            if (CanEnableInBuildSettings(reference, guid))
            {
                buttonCount++;
                rect.x = content.x - EditorButton.Size * buttonCount - 2;

                new EditorButton(rect, () => EnableInBuildSettings(guid))
                    .WithIcon(SpritesPath.EnableInBuildSettings).WithTooltip(EnableInBuildSettingsButtonTooltip)
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
