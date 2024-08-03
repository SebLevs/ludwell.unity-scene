using System;
using System.Collections.Generic;
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
        private static HashSet<SceneAssetReferenceController> _controllers = new();
        
        private readonly SceneAssetReferenceView _view;
        private readonly SerializedProperty _model;

        public SceneAssetReferenceController(SerializedProperty model)
        {
            _model = model;

            _view = new SceneAssetReferenceView(this);
            _view.HideButton();
            
            RegisterCallback<DetachFromPanelEvent>(Dispose);
            RegisterCallback<AttachToPanelEvent>(AddToDrawers);

            _view.ObjectField.tooltip = _model.stringValue;
            _view.ObjectField.RegisterValueChangedCallback(SolveButtonVisibleState);

            _view.BuildSettingsButton.clicked -= AddToBuildSettings;
            _view.BuildSettingsButton.clicked += AddToBuildSettings;

            _view.ObjectField.UnregisterValueChangedCallback(UpdatePropertyCache);
            _view.ObjectField.RegisterValueChangedCallback(UpdatePropertyCache);

            _view.ObjectField.UnregisterValueChangedCallback(SolveButtonVisibleState);
            _view.ObjectField.RegisterValueChangedCallback(SolveButtonVisibleState);
        }
        
        public static void OnBuildSettingsChangedSolveHelpBoxes()
        {
            foreach (var controller in _controllers)
            {
                Debug.LogError("update");
                controller.SolveButtonVisibleState(null);
            }
        }
        
        public void Dispose() => Dispose(null);

        public void Dispose(DetachFromPanelEvent evt)
        {
            RemoveFromDrawers();
            _view.Dispose(null);
        }

        public void SetObjectFieldLabel(string value)
        {
            _view.SetObjectFieldLabel(value);
        }

        public void SetObjectFieldValue(SceneAsset sceneAsset)
        {
            _view.ObjectField.value = sceneAsset;
            SolveButtonVisibleState(null);
        }

        public void SolveButtonVisibleState() => SolveButtonVisibleState(null);
        
        private void SolveButtonVisibleState(ChangeEvent<Object> _)
        {
            if (string.IsNullOrEmpty(_model.stringValue))
            {
                _view.HideButton();
                return;
            }

            var data = SceneAssetDataContainer.Instance.GetValue(_model.stringValue);

            var isInBuildSetting = EditorBuildSettings.scenes.Any(scene => scene.path == data.Path);
            if (!isInBuildSetting)
            {
                Debug.LogError("todo: if scene asset is addressable, do not show the panel");
                _view.ShowButton();
                return;
            }

            _view.HideButton();
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
        
        private void AddToDrawers(AttachToPanelEvent evt)
        {
            _controllers.Add(this);
        }

        private void RemoveFromDrawers()
        {
            _controllers.Remove(this);
        }
    }
}