using System.IO;
using Ludwell.UIToolkitUtilities;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Ludwell.Scene.Editor
{
    [CustomPropertyDrawer(typeof(SceneAssetReference))]
    public class SceneAssetReferenceDrawer : PropertyDrawer
    {
        private readonly string UxmlPath =
            Path.Combine("UI", nameof(SceneAssetReference), "Uxml_" + nameof(SceneAssetReference));

        private readonly string UssPath =
            Path.Combine("UI", nameof(SceneAssetReference), "Uss_" + nameof(SceneAssetReference));

        private SceneAssetReference _sceneAssetReference;

        private ObjectField _objectField;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = new VisualElement();
            root.AddHierarchyFromUxml(UxmlPath);
            root.AddStyleFromUss(UssPath);

            _objectField = root.Q<ObjectField>();

            _sceneAssetReference ??= fieldInfo.GetValue(property.serializedObject.targetObject) as SceneAssetReference;

            var data = _sceneAssetReference?.Value();
            if (data != null)
            {
                _objectField.value = AssetDatabase.LoadAssetAtPath<SceneAsset>(data.Path);
            }
            else if (!_sceneAssetReference.IsKeyEmpty)
            {
                Debug.LogError("Suspicious data | Key has a value, but no binding could be found | Key will be reset");
                _sceneAssetReference.SetKey(string.Empty);

                var activeScene = SceneManager.GetActiveScene();
                EditorSceneManager.MarkSceneDirty(activeScene);
            }

            _objectField.RegisterValueChangedCallback(UpdatePropertyCache);

            return root;
        }

        private void UpdatePropertyCache(ChangeEvent<Object> evt)
        {
            var targetAsSceneAsset = evt.newValue as SceneAsset;

            var activeScene = SceneManager.GetActiveScene();

            if (targetAsSceneAsset == null)
            {
                if (_sceneAssetReference.IsValid)
                {
                    EditorSceneManager.MarkSceneDirty(activeScene);
                }

                _sceneAssetReference.SetKey(string.Empty);

                return;
            }

            var instance = SceneAssetDataContainer.Instance;

            if (instance == null)
            {
                instance = (SceneAssetDataContainer)ResourcesSolver.EnsureAssetExistence(
                    typeof(SceneAssetDataContainer), out _);
            }

            Debug.LogError("Fill addressable ID");
            var assetPath = AssetDatabase.GetAssetPath(targetAsSceneAsset);
            var key = AssetDatabase.AssetPathToGUID(assetPath);
            if (!instance.Contains(key))
            {
                instance.Add(new SceneAssetDataBinder
                {
                    Key = key,
                    Data = new SceneAssetData
                    {
                        BuildIndex = SceneUtility.GetBuildIndexByScenePath(assetPath),
                        Name = targetAsSceneAsset.name,
                        Path = assetPath,
                        AddressableID = "TO BE FILLED"
                    }
                });
                EditorUtility.SetDirty(instance);
                AssetDatabase.SaveAssetIfDirty(instance);
            }

            _sceneAssetReference.SetKey(key);
            EditorSceneManager.MarkSceneDirty(activeScene);
        }
    }
}