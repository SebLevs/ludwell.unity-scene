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
        private static List<SceneAssetReferenceDrawer> _drawers = new();

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var serializedProperty = property.FindPropertyRelative("Key");
            var root = new SceneAssetReferenceController(serializedProperty);
            root.SetObjectFieldLabel(property.displayName);

            root.RegisterCallback<DetachFromPanelEvent>(Dispose);

            root.RegisterCallback<AttachToPanelEvent>(AddToDrawers);

            var data = SceneAssetDataContainer.Instance.GetValue(serializedProperty.stringValue);
            if (data != null)
            {
                root.SetObjectFieldValue(AssetDatabase.LoadAssetAtPath<SceneAsset>(data.Path));
                // SolveHelpBox(null); // todo: call directly from here instead of from controller method?
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

        private void Dispose(DetachFromPanelEvent evt)
        {
            RemoveFromDrawers();
            (evt.target as SceneAssetReferenceController)?.Dispose();
        }

        public static void OnBuildSettingsChangedSolveHelpBoxes()
        {
            foreach (var drawer in _drawers)
            {
                Debug.LogError("update");
                // drawer.SolveHelpBox(null);
            }
        }

        private void AddToDrawers(AttachToPanelEvent evt)
        {
            _drawers.Add(this);
        }

        private void RemoveFromDrawers()
        {
            _drawers.Remove(this);
        }
    }
}