using Ludwell.EditorUtilities.Editor;
using Ludwell.UIToolkitUtilities.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ludwell.Scene.Editor
{
    [CustomPropertyDrawer(typeof(SceneAssetReference))]
    public class SceneAssetReferenceDrawer : PropertyDrawer
    {
        private const string GuidPropertyName = "_guid";
        private const string SceneAssetPropertyName = "_sceneAsset";

        private const string SelectInWindowButtonTooltip = "Select in Scene Manager Toolkit window";
        private const string AddToBuildSettingsButtonTooltip = "Add to Build Settings";
        private const string EnableInBuildSettingsButtonTooltip = "Enable in Build Settings";

        // DO NOT DELETE: This works from the old ways
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var guidProperty = property.FindPropertyRelative(GuidPropertyName);
            var referenceProperty = property.FindPropertyRelative(SceneAssetPropertyName);

            var contentPosition = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            var buttonCount = 0;
            // Buttons
            if (referenceProperty?.objectReferenceValue)
            {
                buttonCount++;
                var centeredY = contentPosition.y + (contentPosition.height - EditorButton.Size) / 2;
                var settingsButtonPosition = new Rect(contentPosition.x - EditorButton.Size * buttonCount, centeredY,
                    EditorButton.Size, EditorButton.Size);

                var settingsButton = new EditorButton(settingsButtonPosition, () => SelectInWindow(guidProperty));
                settingsButton.WithIcon(SpritesPath.Settings).WithTooltip(SelectInWindowButtonTooltip).Build();
            }

            if (CanAddToBuildSettings(referenceProperty, guidProperty))
            {
                buttonCount++;
                var centeredY = contentPosition.y + (contentPosition.height - EditorButton.Size) / 2;
                var settingsButtonPosition = new Rect(contentPosition.x - EditorButton.Size * buttonCount - 2,
                    centeredY,
                    EditorButton.Size, EditorButton.Size);

                var settingsButton =
                    new EditorButton(settingsButtonPosition, () => AddToBuildSettings(guidProperty));
                settingsButton.WithIcon(SpritesPath.AddBuildSettings).WithTooltip(AddToBuildSettingsButtonTooltip)
                    .Build();
            }

            if (CanEnableInBuildSettings(referenceProperty, guidProperty))
            {
                buttonCount++;
                var centeredY = contentPosition.y + (contentPosition.height - EditorButton.Size) / 2;
                var settingsButtonPosition = new Rect(contentPosition.x - EditorButton.Size * buttonCount - 2,
                    centeredY,
                    EditorButton.Size, EditorButton.Size);

                var settingsButton =
                    new EditorButton(settingsButtonPosition, () => EnableInBuildSettings(guidProperty));
                settingsButton.WithIcon(SpritesPath.EnableInBuildSettings)
                    .WithTooltip(EnableInBuildSettingsButtonTooltip).Build();
            }

            var objectFieldRect = new Rect(contentPosition.x + 2, contentPosition.y, contentPosition.width - 4,
                contentPosition.height);
            EditorGUI.PropertyField(objectFieldRect, referenceProperty, GUIContent.none);

            if (GUI.changed && referenceProperty?.objectReferenceValue)
            {
                referenceProperty.objectReferenceValue = referenceProperty.objectReferenceValue as SceneAsset;
                guidProperty.stringValue =
                    AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(referenceProperty.objectReferenceValue));
            }

            EditorGUI.EndProperty();
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


        // DO NOT DELETE: UI TOOLKIT VERSION
        // public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        // {
        //     EditorGUI.BeginProperty(position, label, property);
        //     // EditorGUILayout.PropertyField(property);
        //     EditorGUI.EndProperty();
        // }

        // DO NOT DELETE: UI TOOLKIT VERSION
        // public override VisualElement CreatePropertyGUI(SerializedProperty property)
        // {
        //     var root = new SceneAssetReferenceController(property);
        //     SetDisplayName(property, root);
        //     return root;
        // }

        // DO NOT DELETE: UI TOOLKIT VERSION
        // private static void SetDisplayName(SerializedProperty property, SceneAssetReferenceController root)
        // {
        //     var displayName = property.displayName;
        //
        //     var match = Regex.Match(property.propertyPath, @"\[(\d+)\]");
        //     if (match.Success)
        //     {
        //         var index = match.Groups[1].Value;
        //         displayName = $"Element {index}";
        //     }
        //
        //     root.SetObjectFieldLabel(displayName);
        // }
    }
}