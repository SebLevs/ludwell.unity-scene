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

            var data = SceneAssetDataBinders.Instance.GetDataFromId(_model.stringValue);
            if (data != null)
            {
                _view.ObjectField.value = AssetDatabase.LoadAssetAtPath<SceneAsset>(data.Path);
                if (!Application.isPlaying) SolveButtonVisibleState(null);
            }
            else if (!string.IsNullOrEmpty(_model.stringValue))
            {
                _model.stringValue = string.Empty;
                if (!Application.isPlaying) EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            }

            _view.ObjectField.tooltip = _model.stringValue;
            _view.ObjectField.RegisterValueChangedCallback(SolveButtonVisibleState);

            _view.BuildSettingsButton.clicked -= AddToBuildSettings;
            _view.BuildSettingsButton.clicked += AddToBuildSettings;

            _view.ObjectField.UnregisterValueChangedCallback(OnValueChanged);
            _view.ObjectField.RegisterValueChangedCallback(OnValueChanged);

            _view.ObjectField.UnregisterValueChangedCallback(SolveButtonVisibleState);
            _view.ObjectField.RegisterValueChangedCallback(SolveButtonVisibleState);
        }

        public static void SolveHelpBoxes()
        {
            foreach (var controller in _controllers)
            {
                controller.SolveButtonVisibleState(null);
            }
        }

        public void Dispose() => Dispose(null);

        public void SetObjectFieldLabel(string value)
        {
            _view.SetObjectFieldLabel(value);
        }

        private void SolveButtonVisibleState(ChangeEvent<Object> _)
        {
            if (Application.isPlaying) return;
            if (string.IsNullOrEmpty(_model.stringValue))
            {
                _view.HideButton();
                return;
            }

            var data = SceneAssetDataBinders.Instance.GetDataFromId(_model.stringValue);

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
            var data = SceneAssetDataBinders.Instance.GetDataFromId(_model.stringValue);
            EditorSceneManagerHelper.AddSceneToBuildSettings(data.Path);
        }

        private void OnValueChanged(ChangeEvent<Object> evt)
        {
            var targetAsSceneAsset = evt.newValue as SceneAsset;

            if (targetAsSceneAsset == null)
            {
                if (!string.IsNullOrEmpty(_model.stringValue))
                {
                    EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
                }

                UpdateModel(string.Empty);

                return;
            }

            var assetPath = AssetDatabase.GetAssetPath(targetAsSceneAsset);
            var key = AssetDatabase.AssetPathToGUID(assetPath);
            UpdateModel(key);
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }

        private void UpdateModel(string value)
        {
            _model.stringValue = value;
            _view.ObjectField.tooltip = _model.stringValue;
            _model.serializedObject.ApplyModifiedProperties();
        }

        private void AddToDrawers(AttachToPanelEvent evt)
        {
            _controllers.Add(this);
        }

        private void RemoveFromDrawers()
        {
            _controllers.Remove(this);
        }

        private void Dispose(DetachFromPanelEvent evt)
        {
            RemoveFromDrawers();
            _view.Dispose(null);
        }
    }
}