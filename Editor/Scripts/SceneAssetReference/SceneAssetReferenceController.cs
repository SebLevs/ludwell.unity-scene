using System;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace Ludwell.Scene.Editor
{
    public class SceneAssetReferenceController : VisualElement, IDisposable
    {
        private readonly SceneAssetReferenceView _view;
        private readonly SerializedProperty _model;
        
        
        public SceneAssetReferenceController(SerializedProperty model)
        {
            _model = model;
            
            _view = new SceneAssetReferenceView(this);

            _view.ObjectField.tooltip = _model.stringValue;
            _view.ObjectField.RegisterValueChangedCallback(SolveHelpBox);
            
            _view.HelpBoxButton.clicked -= AddToBuildSettings;
            _view.HelpBoxButton.clicked += AddToBuildSettings;
            
            _view.ObjectField.UnregisterValueChangedCallback(UpdatePropertyCache);
            _view.ObjectField.RegisterValueChangedCallback(UpdatePropertyCache);
        
            _view.ObjectField.UnregisterValueChangedCallback(SolveHelpBox);
            _view.ObjectField.RegisterValueChangedCallback(SolveHelpBox);
        }
        
        public void Dispose()
        {
            _view.Dispose(null);
        }
        
        public void SetObjectFieldLabel(string value)
        {
            _view.SetObjectFieldLabel(value);
        }
        
        public void SetObjectFieldValue(SceneAsset sceneAsset)
        {
            _view.ObjectField.value = sceneAsset;
            SolveHelpBox(null);
        }
        
        private void SolveHelpBox(ChangeEvent<Object> _)
        {
            if (string.IsNullOrEmpty(_model.stringValue))
            {
                _view.HideHelpBox();
                return;
            }
            
            var data = SceneAssetDataContainer.Instance.GetValue(_model.stringValue);
            
            var isInBuildSetting = EditorBuildSettings.scenes.Any(scene => scene.path == data.Path);
            if (!isInBuildSetting)
            {
                Debug.LogError("todo: if scene asset is addressable, do not show the panel");
            
                // _helpBoxButton.text = $"Add {data.Name} to Build Settings";
                _view.ShowHelpBox();
                return;
            }
            
            _view.HideHelpBox();
        }
        
        private void AddToBuildSettings()
        {
            var data = SceneAssetDataContainer.Instance.GetValue(_model.stringValue);
            EditorSceneManagerHelper.AddSceneToBuildSettings(data.Path);
        }
        
        private void UpdatePropertyCache(ChangeEvent<Object> evt)
        {
            var targetAsSceneAsset = evt.newValue as SceneAsset;

            if (targetAsSceneAsset == null)
            {
                if (!string.IsNullOrEmpty(_model.stringValue))
                {
                    EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
                }
            
                _model.stringValue = string.Empty;
                _view.ObjectField.tooltip = _model.stringValue;
                _model.serializedObject.ApplyModifiedProperties();
            
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

            _model.stringValue = key;
            _view.ObjectField.tooltip = _model.stringValue;
            _model.serializedObject.ApplyModifiedProperties();
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }
    }
}
