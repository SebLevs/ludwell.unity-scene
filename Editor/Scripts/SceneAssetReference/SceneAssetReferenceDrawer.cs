using System.Linq;
using Ludwell.EditorUtilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Ludwell.Scene.Editor
{
    [CustomPropertyDrawer(typeof(SceneAssetReference))]
    public class SceneAssetReferenceDrawer : PropertyDrawer
    {
        private const string GuidPropertyName = "_guid";
        private const string SceneAssetPropertyName = "_reference";
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var guidProperty = property.FindPropertyRelative(GuidPropertyName);
            var referenceProperty = property.FindPropertyRelative(SceneAssetPropertyName);

            var contentPosition = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Keyboard), label);
            
            var controller = new SceneAssetReferenceDrawerController(contentPosition, property);

            var positionX = contentPosition.x + EditorButton.Size + 2;
            var width = contentPosition.width - EditorButton.Size - 4;
            var objectFieldRect = new Rect(positionX, contentPosition.y, width, contentPosition.height);
            EditorGUI.PropertyField(objectFieldRect, referenceProperty, GUIContent.none);

            if (GUI.changed)
            {
                referenceProperty.objectReferenceValue = referenceProperty.objectReferenceValue as SceneAsset;
                
                guidProperty.stringValue = referenceProperty.objectReferenceValue == null ? 
                    string.Empty : 
                    AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(referenceProperty.objectReferenceValue));
            }

            EditorGUI.EndProperty();
        }
        
        public static void RepaintInspectorWindows()
        {
            var inspectorWindows = Resources.FindObjectsOfTypeAll<EditorWindow>()
                .Where(w => w.GetType().Name == "InspectorWindow" || w.GetType().Name == "PropertyEditor");

            foreach (var window in inspectorWindows)
            {
                window.Repaint();
            }
        }
    }
}