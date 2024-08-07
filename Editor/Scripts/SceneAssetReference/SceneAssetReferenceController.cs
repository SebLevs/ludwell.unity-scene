using System;
using System.Collections.Generic;
using System.Linq;
using Ludwell.UIToolkitUtilities;
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
            _view.HideBuildSettingsButton();
            _view.HideSelectInWindowButton();

            EditorApplication.update += SolveButtonOnMissingReference;

            _view.ObjectField.FindFirstChildWhereNameContains(string.Empty).Insert(0, _view.BuildSettingsButton);
            _view.ObjectField.FindFirstChildWhereNameContains(string.Empty).Insert(0, _view.SelectInWindowButton);

            _view.SelectInWindowButton.clicked += SelectInWindow;

            RegisterCallback<AttachToPanelEvent>(AddToDrawers);
            RegisterCallback<DetachFromPanelEvent>(Dispose);

            var data = SceneAssetDataBinders.Instance.GetDataFromId(_model.stringValue);
            if (data != null)
            {
                _view.ObjectField.value = AssetDatabase.LoadAssetAtPath<SceneAsset>(data.Path);
                if (!Application.isPlaying) SolveBuildSettingsButton(null);
            }

            _view.BuildSettingsButton.clicked -= AddToBuildSettings;
            _view.BuildSettingsButton.clicked += AddToBuildSettings;

            _view.ObjectField.RegisterValueChangedCallback(OnValueChanged);
            _view.ObjectField.RegisterValueChangedCallback(SolveBuildSettingsButton);
            _view.ObjectField.RegisterValueChangedCallback(SolveSelectInWindowButton);

            SolveBuildSettingsButton(null);
            SolveSelectInWindowButton(null);
        }

        public static void SolveButtonsVisibleState()
        {
            foreach (var controller in _controllers)
            {
                controller.SolveBuildSettingsButton(null);
            }
        }

        public void Dispose() => Dispose(null);

        public void SetObjectFieldLabel(string value)
        {
            _view.SetObjectFieldLabel(value);
        }

        private void SolveBuildSettingsButton(ChangeEvent<Object> _)
        {
            if (Application.isPlaying || _view.ObjectField.value == null)
            {
                _view.HideBuildSettingsButton();
                return;
            }

            var data = SceneAssetDataBinders.Instance.GetDataFromId(_model.stringValue);

            var isInBuildSetting = EditorBuildSettings.scenes.Any(scene => scene.path == data.Path);
            if (!isInBuildSetting)
            {
                Debug.LogError("todo: if scene asset is addressable, do not show the button");
                _view.ShowBuildSettingsButton();
                return;
            }

            _view.HideBuildSettingsButton();
        }

        private void SolveSelectInWindowButton(ChangeEvent<Object> _)
        {
            if (_view.ObjectField.value == null)
            {
                _view.HideSelectInWindowButton();
                return;
            }

            _view.ShowSelectInWindowButton();
        }

        private void SolveButtonOnMissingReference()
        {
            if (_view.ObjectField.value != null) return;
            if (_view.AreButtonsHidden()) return;
            _view.HideBuildSettingsButton();
            _view.HideSelectInWindowButton();
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

        private void SelectInWindow()
        {
            var binderToSelect = ResourcesLocator.GetSceneAssetDataBinders()
                .GetBinderFromId(_model.stringValue);
            var index = SceneAssetDataBinders.Instance.IndexOf(binderToSelect);
            var window = EditorWindow.GetWindow<SceneManagerToolkitWindow>();
            window.Focus();
            window.SceneElementsController.ScrollToItemIndex(index);
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

            EditorApplication.update -= SolveButtonOnMissingReference;

            _view.SelectInWindowButton.clicked -= SelectInWindow;

            _view.ObjectField.UnregisterValueChangedCallback(OnValueChanged);
            _view.ObjectField.UnregisterValueChangedCallback(SolveBuildSettingsButton);
            _view.ObjectField.UnregisterValueChangedCallback(SolveSelectInWindowButton);

            _view.Dispose(null);
        }
    }
}