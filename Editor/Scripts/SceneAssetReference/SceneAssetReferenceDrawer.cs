using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine.UIElements;

namespace Ludwell.Scene.Editor
{
    [CustomPropertyDrawer(typeof(SceneAssetReference))]
    public class SceneAssetReferenceDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var serializedProperty = property.FindPropertyRelative("_guid");
            var root = new SceneAssetReferenceController(serializedProperty);

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