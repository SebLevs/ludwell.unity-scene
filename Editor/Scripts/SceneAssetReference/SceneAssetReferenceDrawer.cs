using System;
using Ludwell.UIToolkitUtilities.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ludwell.Scene.Editor
{
    public class InspectorButton
    {
        public const float Size = 20f;
        private const float _iconSize = 8f;
        private Rect _rect;

        private Action _behaviour;

        public InspectorButton(Rect position, Action behaviour)
        {
            _rect = new Rect(position.x, position.y, Size, Size);
            _behaviour = behaviour;
            if (GUI.Button(_rect, string.Empty))
            {
                behaviour?.Invoke();
            }
        }

        public void WithIcon(string path, float iconSize = _iconSize)
        {
            var buttonTexture = Resources.Load<Texture2D>(path);
            var textureRect = new Rect(
                _rect.x + (_rect.width - iconSize) / 2,
                _rect.y + (_rect.height - iconSize) / 2,
                iconSize,
                iconSize
            );
            GUI.DrawTexture(textureRect, buttonTexture);
        }
    }

    [CustomPropertyDrawer(typeof(SceneAssetReference))]
    public class SceneAssetReferenceDrawer : PropertyDrawer
    {
        // DO NOT DELETE: This kindof works from the old ways
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var sceneReference = fieldInfo.GetValue(property.serializedObject.targetObject) as SceneAssetReference;
            var guidProp = property.FindPropertyRelative("_guid");
            var sceneAssetProp = property.FindPropertyRelative("_sceneAsset");

            var contentPosition = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            var buttonCount = 0;
            // Buttons
            if (sceneReference?._sceneAsset)
            {
                buttonCount++;
                var settingsButton = new InspectorButton(contentPosition, () => SelectInWindow(guidProp));
                settingsButton.WithIcon(SpritesPath.Settings);
            }

            var objectFieldRect = new Rect(contentPosition.x + InspectorButton.Size * buttonCount + 2, contentPosition.y,
                contentPosition.width - InspectorButton.Size * buttonCount - 4, contentPosition.height);
            EditorGUI.PropertyField(objectFieldRect, sceneAssetProp, GUIContent.none);

            if (GUI.changed && sceneReference != null)
            {
                // todo: in list and array, sceneReference is not registered properly?
                sceneReference._sceneAsset = sceneAssetProp.objectReferenceValue as SceneAsset;
                sceneReference._guid =
                    AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(sceneReference._sceneAsset));
                guidProp.stringValue = sceneReference._guid;
            }

            EditorGUI.EndProperty();
        }
        
        private void SelectInWindow(SerializedProperty guidProperty)
        {
            var binderToSelect = ResourcesLocator.GetSceneAssetDataBinders()
                .GetBinderFromId(guidProperty.stringValue);
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