using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
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

            var data = SceneAssetDataContainer.Instance.GetValue(serializedProperty.stringValue);
            if (data != null)
            {
                root.SetObjectFieldValue(AssetDatabase.LoadAssetAtPath<SceneAsset>(data.Path));
                root.SolveButtonVisibleState();
            }
            else if (!string.IsNullOrEmpty(serializedProperty.stringValue))
            {
                Debug.LogError("Suspicious data | Key has a value, but no binding could be found | Key will be reset");
                serializedProperty.stringValue = string.Empty;

                var activeScene = SceneManager.GetActiveScene();
                EditorSceneManager.MarkSceneDirty(activeScene);
            }

            return root;
        }
    }
}