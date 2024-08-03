using UnityEditor;
using UnityEngine.UIElements;

namespace Ludwell.Scene.Editor
{
    [CustomPropertyDrawer(typeof(SceneAssetReference))]
    public class SceneAssetReferenceDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var serializedProperty = property.FindPropertyRelative("Key");
            var root = new SceneAssetReferenceController(serializedProperty);
            root.SetObjectFieldLabel(property.displayName);

            return root;
        }
    }
}