using System.IO;
using Ludwell.UIToolkitUtilities;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ludwell.Scene.Editor
{
    [CustomPropertyDrawer(typeof(SceneAssetReference))]
    public class SceneAssetReferenceDrawer : PropertyDrawer
    {
        private readonly string UxmlPath = Path.Combine("UI", nameof(SceneAssetReference), "Uxml_" + nameof(SceneAssetReference));
        private readonly string UssPath = Path.Combine("UI", nameof(SceneAssetReference), "Uss_" + nameof(SceneAssetReference));
    
        private SceneAssetReference sceneAssetReference;

        private ObjectField _objectField;
        
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            sceneAssetReference ??= fieldInfo.GetValue(property.serializedObject.targetObject) as SceneAssetReference;
            
            var root = new VisualElement();
            root.AddHierarchyFromUxml(UxmlPath);
            root.AddStyleFromUss(UssPath);

            _objectField = root.Q<ObjectField>();

            if (sceneAssetReference.BuildIndex == 2)
            {
                Debug.LogError("Build index was serialized");
            }
            
            _objectField.RegisterValueChangedCallback(UpdatePropertyCache);
            return root;
        }

        private void UpdatePropertyCache(ChangeEvent<Object> evt)
        {
            var targetAsSceneAsset = evt.newValue as SceneAsset;
            sceneAssetReference.BuildIndex = 2;
        }
    }
}
