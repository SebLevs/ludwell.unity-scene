using System;
using System.Collections.Generic;
using System.Linq;
using Ludwell.UIToolkitUtilities;
using Ludwell.UIToolkitUtilities.Editor;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace Ludwell.Scene.Editor
{
    public class SceneAssetReferenceController : VisualElement, IDisposable
    {
        private static readonly HashSet<SceneAssetReferenceController> _controllers = new();
        private static string _copyBuffer;

        private readonly SceneAssetReferenceView _view;
        private readonly SerializedProperty _rootProperty;
        private readonly SerializedProperty _objectProperty;
        private readonly SerializedProperty _guidProperty;

        private ContextualMenuManipulator _contextualMenuManipulator;

        public SceneAssetReferenceController(SerializedProperty rootProperty)
        {
            _view = new SceneAssetReferenceView(this);
            _view.HideBuildSettingsButton();
            _view.HideSelectInWindowButton();

            _rootProperty = rootProperty;
            _objectProperty = rootProperty.FindPropertyRelative("_sceneAsset");
            _view.ObjectField.BindProperty(_objectProperty);
            _guidProperty = rootProperty.FindPropertyRelative("_guid");

            EditorApplication.update += SolveButtonsOnMissingReference;

            _view.ObjectField.FindFirstChildWhereNameContains(string.Empty)
                .Insert(0, _view.EnableInBuildSettingsButton);
            _view.ObjectField.FindFirstChildWhereNameContains(string.Empty).Insert(0, _view.BuildSettingsButton);
            _view.ObjectField.FindFirstChildWhereNameContains(string.Empty).Insert(0, _view.SelectInWindowButton);

            _view.SelectInWindowButton.clicked += SelectInWindow;

            RegisterCallback<AttachToPanelEvent>(AddToDrawers);
            RegisterCallback<DetachFromPanelEvent>(Dispose);

            var sceneAsset = _objectProperty.objectReferenceValue as SceneAsset;
            if (sceneAsset != null) _view.ObjectField.value = sceneAsset;

            _view.BuildSettingsButton.clicked -= AddToBuildSettings;
            _view.BuildSettingsButton.clicked += AddToBuildSettings;

            _view.EnableInBuildSettingsButton.clicked -= EnableInBuildSettings;
            _view.EnableInBuildSettingsButton.clicked += EnableInBuildSettings;

            _view.ObjectField.RegisterValueChangedCallback(OnValueChanged);
            _view.ObjectField.RegisterValueChangedCallback(SolveBuildSettingsButton);
            _view.ObjectField.RegisterValueChangedCallback(SolveEnableInBuildSettingsButton);
            _view.ObjectField.RegisterValueChangedCallback(SolveSelectInWindowButton);

            SolveBuildSettingsButton(null);
            SolveEnableInBuildSettingsButton(null);
            SolveSelectInWindowButton(null);

            // _contextualMenuManipulator = BuildContextualMenuManipulator();
            // _view.ObjectField.AddManipulator(_contextualMenuManipulator);
            RegisterCallback<KeyDownEvent>(ExecuteKeyEvents);
        }

        public static void SolveAllBuildSettingsButtonVisibleState()
        {
            foreach (var controller in _controllers)
            {
                controller.SolveBuildSettingsButton(null);
            }

            foreach (var controller in _controllers)
            {
                controller.SolveEnableInBuildSettingsButton(null);
            }
        }

        public void Dispose() => Dispose(null);

        public void SetObjectFieldLabel(string value)
        {
            _view.ObjectFieldLabel.text = value;
        }

        private static bool IsCopyBufferASceneAssetPath()
        {
            return EditorGUIUtility.systemCopyBuffer.Contains(".unity");
        }

        private void SolveBuildSettingsButton(ChangeEvent<Object> _)
        {
            if (Application.isPlaying || _view.ObjectField.value == null)
            {
                _view.HideBuildSettingsButton();
                return;
            }

            var data = SceneAssetDataBinders.Instance.GetDataFromId(_guidProperty.stringValue);

            if (data.IsAddressable)
            {
                _view.HideBuildSettingsButton();
                return;
            }

            var isInBuildSetting = EditorSceneManagerHelper.IsSceneInBuildSettings(data.Path);
            if (!isInBuildSetting)
            {
                _view.ShowBuildSettingsButton();
                return;
            }

            _view.HideBuildSettingsButton();
        }

        private void SolveEnableInBuildSettingsButton(ChangeEvent<Object> _)
        {
            if (Application.isPlaying || _view.ObjectField.value == null)
            {
                _view.HideEnableInBuildSettingsButton();
                return;
            }

            var data = SceneAssetDataBinders.Instance.GetDataFromId(_guidProperty.stringValue);
            var isInBuildSetting = EditorSceneManagerHelper.IsSceneInBuildSettings(data.Path);
            var isEnabled = EditorSceneManagerHelper.IsSceneEnabledInBuildSettings(data.Path);

            if (!data.IsAddressable && isInBuildSetting && !isEnabled)
            {
                _view.ShowEnableInBuildSettingsButton();
                return;
            }

            _view.HideEnableInBuildSettingsButton();
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

        private void SolveButtonsOnMissingReference()
        {
            if (_view.ObjectField.value != null) return;
            if (_view.AreButtonsHidden()) return;
            _view.HideBuildSettingsButton();
            _view.HideSelectInWindowButton();
        }

        private void AddToBuildSettings()
        {
            var data = SceneAssetDataBinders.Instance.GetDataFromId(_guidProperty.stringValue);
            EditorSceneManagerHelper.AddSceneToBuildSettings(data.Path);
        }

        private void EnableInBuildSettings()
        {
            var data = SceneAssetDataBinders.Instance.GetDataFromId(_guidProperty.stringValue);
            EditorSceneManagerHelper.EnableSceneInBuildSettings(data.Path, true);
        }

        private void OnValueChanged(ChangeEvent<Object> evt)
        {
            var assetPath = AssetDatabase.GetAssetPath(_view.ObjectField.value);
            var key = AssetDatabase.AssetPathToGUID(assetPath);
            Debug.LogError($"guid property: {_guidProperty}");
            UpdateGuid(key);
        }

        private void UpdateGuid(string value)
        {
            _guidProperty.stringValue = value;
            _guidProperty.serializedObject.ApplyModifiedProperties();
        }

        private void SelectInWindow()
        {
            var binderToSelect = ResourcesLocator.GetSceneAssetDataBinders()
                .GetBinderFromId(_guidProperty.stringValue);
            var index = SceneAssetDataBinders.Instance.IndexOf(binderToSelect);
            var window = EditorWindow.GetWindow<SceneManagerToolkitWindow>();
            window.Focus();

            var viewManager = window.rootVisualElement.Q<ViewManager>();
            viewManager.TransitionToFirstViewOfType<SceneElementsController>();

            window.rootVisualElement.schedule.Execute(() =>
            {
                window.SceneElementsController.ScrollToItemIndex(index);
            });
        }

        private ContextualMenuManipulator BuildContextualMenuManipulator()
        {
            return new ContextualMenuManipulator(evt =>
            {
                evt.menu.AppendAction("Copy Property Path", CopyPropertyPath);
                evt.menu.AppendAction("Apply to Prefab: Foo", ApplyToPrefab);
                evt.menu.AppendAction("Copy", CopyGuid, GetStatus());
                evt.menu.AppendSeparator();
                evt.menu.AppendAction("Copy Path", CopyPath, GetStatus());
                evt.menu.AppendAction("Copy GUID", CopyGuid, GetStatus());
                evt.menu.AppendAction("Paste", Paste, GetStatusFromBufferData());
            });
        }

        private void ApplyToPrefab(DropdownMenuAction obj)
        {
            Debug.LogError("implement");
        }

        private bool HasBinderAtValue()
        {
            return ResourcesLocator.GetSceneAssetDataBinders().GetBinderFromId(_guidProperty.stringValue) == null;
        }

        private DropdownMenuAction.Status GetStatus()
        {
            return HasBinderAtValue() ? DropdownMenuAction.Status.Disabled : DropdownMenuAction.Status.Normal;
        }

        private DropdownMenuAction.Status GetStatusFromBufferData()
        {
            var clipboardContent = EditorGUIUtility.systemCopyBuffer;

            if (string.IsNullOrEmpty(clipboardContent)) return DropdownMenuAction.Status.Disabled;

            SceneAsset sceneAsset;

            if (IsCopyBufferASceneAssetPath())
            {
                sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(clipboardContent);
            }
            else
            {
                var path = AssetDatabase.GUIDToAssetPath(clipboardContent);
                sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
            }

            return sceneAsset == null ? DropdownMenuAction.Status.Disabled : DropdownMenuAction.Status.Normal;
        }

        private void CopyPropertyPath(DropdownMenuAction _)
        {
            EditorGUIUtility.systemCopyBuffer = _view.ObjectFieldLabel.text;
        }

        private void CopyPath(DropdownMenuAction _)
        {
            var data = ResourcesLocator.GetSceneAssetDataBinders().GetDataFromId(_guidProperty.stringValue);
            EditorGUIUtility.systemCopyBuffer = data.Path;
            _copyBuffer = _guidProperty.stringValue;
        }

        private void CopyGuid(DropdownMenuAction _)
        {
            EditorGUIUtility.systemCopyBuffer = _guidProperty.stringValue;
            _copyBuffer = _guidProperty.stringValue;
        }

        private void Paste(DropdownMenuAction _)
        {
            var clipboardContent = EditorGUIUtility.systemCopyBuffer;

            if (string.IsNullOrEmpty(clipboardContent)) return;

            if (IsCopyBufferASceneAssetPath())
            {
                _view.ObjectField.value = AssetDatabase.LoadAssetAtPath<SceneAsset>(clipboardContent);
                return;
            }

            var path = AssetDatabase.GUIDToAssetPath(clipboardContent);
            _view.ObjectField.value = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
        }

        private void ExecuteKeyEvents(KeyDownEvent evt)
        {
            switch (evt.keyCode)
            {
                case KeyCode.C when evt.ctrlKey:
                    EditorGUIUtility.systemCopyBuffer = _guidProperty.stringValue;
                    _copyBuffer = _guidProperty.stringValue;
                    break;
                case KeyCode.V when evt.ctrlKey:
                    var path = AssetDatabase.GUIDToAssetPath(_copyBuffer);
                    var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
                    if (!sceneAsset) return;
                    _view.ObjectField.value = sceneAsset;
                    break;
                case KeyCode.Delete or KeyCode.Backspace:
                    if (_view.ObjectField == null) break;
                    _view.ObjectField.value = null;
                    _guidProperty.stringValue = string.Empty;
                    break;
            }
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
            _copyBuffer = string.Empty;
            RemoveFromDrawers();

            EditorApplication.update -= SolveButtonsOnMissingReference;

            _view.SelectInWindowButton.clicked -= SelectInWindow;

            _view.ObjectField.UnregisterValueChangedCallback(OnValueChanged);
            _view.ObjectField.UnregisterValueChangedCallback(SolveBuildSettingsButton);
            _view.ObjectField.UnregisterValueChangedCallback(SolveEnableInBuildSettingsButton);
            _view.ObjectField.UnregisterValueChangedCallback(SolveSelectInWindowButton);

            _view.ObjectField.RemoveManipulator(_contextualMenuManipulator);
            _contextualMenuManipulator = null;
            UnregisterCallback<KeyDownEvent>(ExecuteKeyEvents);

            _view.Dispose(null);
        }
    }
}