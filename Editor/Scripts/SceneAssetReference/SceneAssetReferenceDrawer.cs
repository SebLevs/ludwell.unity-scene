using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ludwell.Scene.Editor
{
    [CustomPropertyDrawer(typeof(SceneAssetReference))]
    public class SceneAssetReferenceDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUILayout.PropertyField(property);
        }

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = new SceneAssetReferenceController(property);
            SetDisplayName(property, root);
            return root;
        }

        private static void SetDisplayName(SerializedProperty property, SceneAssetReferenceController root)
        {
            var displayName = property.displayName;

            var match = Regex.Match(property.propertyPath, @"\[(\d+)\]");
            if (match.Success)
            {
                var index = match.Groups[1].Value;
                displayName = $"Element {index}";
            }

            root.SetObjectFieldLabel(displayName);
        }
    }
}