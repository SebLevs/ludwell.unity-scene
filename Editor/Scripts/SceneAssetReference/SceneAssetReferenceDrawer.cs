using UnityEditor;
using UnityEngine;

namespace Ludwell.Scene.Editor
{
    [CustomPropertyDrawer(typeof(SceneAssetReference))]
    public class SceneAssetReferenceDrawer : PropertyDrawer
    {
        private const string GuidPropertyName = "_guid";
        private const string SceneAssetPropertyName = "_reference";

        // DO NOT DELETE: This works from the old ways
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var guidProperty = property.FindPropertyRelative(GuidPropertyName);
            var referenceProperty = property.FindPropertyRelative(SceneAssetPropertyName);

            var contentPosition = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            new SceneAssetReferenceDrawerController(contentPosition, referenceProperty, guidProperty);

            var objectFieldRect = new Rect(contentPosition.x + 2, contentPosition.y, contentPosition.width - 4,
                contentPosition.height);
            EditorGUI.PropertyField(objectFieldRect, referenceProperty, GUIContent.none);

            if (GUI.changed && referenceProperty?.objectReferenceValue)
            {
                Debug.LogError(
                    "Remove reference check in parameters so that the guid gets resetted properly if reference is null");
                referenceProperty.objectReferenceValue = referenceProperty.objectReferenceValue as SceneAsset;
                guidProperty.stringValue =
                    AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(referenceProperty.objectReferenceValue));
            }

            EditorGUI.EndProperty();
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