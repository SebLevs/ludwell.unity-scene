using System;
using System.IO;
using Ludwell.UIToolkitElements.Editor;
using Ludwell.UIToolkitUtilities;
using Ludwell.UIToolkitUtilities.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Ludwell.Scene.Editor
{
    public class SceneElementController : VisualElement, IListViewVisualElement<SceneManagerElementData>, IDisposable
    {
        private static SceneElementController _currentSceneElement;

        private const string CurrentActiveScene = "currentActiveScene";

        private readonly SceneElementView _view;
        private SceneManagerElementData _model;

        private readonly FoldoutController _foldout;
        private readonly TagsShelfController _tagsShelfController;
        private ViewManager _viewManager;

        public void SetOpenState(bool state) => _foldout.IsOpen = state;

        public void FocusTextField() => _foldout.FocusTextField();

        public bool IsActiveScene() => EditorSceneManagerHelper.IsActiveScene(_model.SceneData);

        public SceneElementController()
        {
            _view = new SceneElementView(this);
            _view.SetActiveButton.clicked += SetAsActiveScene;
            InitializeOpenAdditiveButton();
            _view.OpenButton.clicked += OpenScene;
            InitializeLoadButton();
            _view.PingButton.clicked += SelectSceneDataInProject;
            _view.DirectoryChangeButton.clicked += ChangeFolder;
            InitializeBuildSettingsButton();

            _foldout = new FoldoutController(this, false);
            _foldout.SetOnPreventHeaderClick(target => target is Button);
            _foldout.TitleTextField.RegisterCallback<BlurEvent>(RenameAsset);

            _tagsShelfController = new TagsShelfController(this, TransitionViewToTagsManager);

            RegisterCallback<AttachToPanelEvent>(BindViewManager);

            RegisterCallback<ClickEvent>(SessionStateCacheFoldoutValue);

            Services.Get<Disposer>().Add(this);
        }

        public void Dispose()
        {
            _currentSceneElement = null;

            _view.SetActiveButton.clicked -= SetAsActiveScene;
            _view.OpenButton.clicked -= OpenScene;
            _view.PingButton.clicked -= SelectSceneDataInProject;
            _view.DirectoryChangeButton.clicked -= ChangeFolder;

            _foldout.TitleTextField.UnregisterCallback<BlurEvent>(RenameAsset);

            UnregisterCallback<AttachToPanelEvent>(BindViewManager);
            UnregisterCallback<ClickEvent>(SessionStateCacheFoldoutValue);

            _view.Dispose();
            _foldout.Dispose();
        }

        public void CacheData(SceneManagerElementData data)
        {
            _model = data;
        }

        public void BindElementToCachedData()
        {
        }

        public void SetElementFromCachedData()
        {
            SetFoldoutValueFromSavedState();
            _foldout.Title = _model.SceneData.Name;
            _view.SetIconAssetOutsideAssets(_model.IsOutsideAssetsFolder);

            _view.SetPathTooltip(AssetDatabase.GetAssetPath(_model.SceneData));

            SetTagsContainer();

            SolveSetActiveButton();
            SolveOpenAdditiveButton();
            SolveOpenButton();
            SetLoadButtonState();
            _view.SetDirectoryChangeButtonEnable(!EditorApplication.isPlaying);
            SolveBuildSettingsButton();
        }

        public void AddToBuildSettings()
        {
            EditorSceneManagerHelper.AddSceneToBuildSettings(_model.SceneData);
        }

        public void RemoveFromBuildSettings()
        {
            EditorSceneManagerHelper.RemoveSceneFromBuildSettings(_model.SceneData);
        }

        public void OpenSceneAdditive()
        {
            EditorSceneManagerHelper.OpenSceneAdditive(_model.SceneData);
        }

        public void RemoveSceneAdditive()
        {
            EditorSceneManagerHelper.RemoveSceneAdditive(_model.SceneData);
        }

        private void SolveSetActiveButton()
        {
            var isSceneLoaded = EditorSceneManagerHelper.IsSceneLoaded(_model.SceneData);
            _view.SetSetActiveButtonEnable(isSceneLoaded && !IsActiveScene());
        }

        private void SolveOpenAdditiveButton()
        {
            _view.SetOpenAdditiveButtonEnable(!EditorApplication.isPlaying);

            // todo: optimize
            var asset = EditorSceneManagerHelper.GetSceneAssetPath(_model.SceneData);
            var assetAtPath = SceneManager.GetSceneByPath(asset);

            _view.SwitchOpenAdditiveButtonState(assetAtPath.isLoaded);
            if (!assetAtPath.isLoaded) return;
            if (asset == SceneManager.GetActiveScene().path && SceneManager.sceneCount == 1)
            {
                _view.SetOpenAdditiveButtonEnable(false);
            }
        }

        private void SolveBuildSettingsButton()
        {
            _view.SetBuildSettingsButtonButtonEnable(!EditorApplication.isPlaying);
            var path = EditorSceneManagerHelper.GetSceneAssetPath(_model.SceneData);
            if (EditorSceneManagerHelper.IsSceneInBuildSettings(path))
            {
                _view.SwitchBuildSettingsButtonState(true);
                return;
            }

            _view.SwitchBuildSettingsButtonState(false);
        }

        private void SetFoldoutValueFromSavedState()
        {
            var id = _model.SceneData.GetInstanceID().ToString();
            var oldState = SessionState.GetBool(id, false);
            SetOpenState(oldState);
        }

        private void SetTagsContainer()
        {
            _tagsShelfController.UpdateData(_model);
            _tagsShelfController.Populate();
        }

        private void SetLoadButtonState()
        {
            if (!EditorApplication.isPlaying) return;
            if (SessionState.GetInt(CurrentActiveScene, -1) != _model.SceneData.GetInstanceID())
            {
                _view.SwitchLoadButtonState(false);
                return;
            }

            _view.SwitchLoadButtonState(true);
        }

        private void SetAsActiveScene()
        {
            EditorSceneManagerHelper.SetActiveScene(_model.SceneData);
        }

        private void InitializeOpenAdditiveButton()
        {
            var stateOne = new DualStateButtonState(
                _view.OpenAdditiveButton,
                Resources.Load<Sprite>(SpritesPath.OpenAdditive),
                OpenSceneAdditive);

            var stateTwo = new DualStateButtonState(
                _view.OpenAdditiveButton,
                Resources.Load<Sprite>(SpritesPath.RemoveAdditive),
                RemoveSceneAdditive);

            _view.OpenAdditiveButton.Initialize(stateOne, stateTwo);
        }

        private void InitializeLoadButton()
        {
            var stateOne = new DualStateButtonState(
                _view.LoadButton,
                Resources.Load<Sprite>(SpritesPath.Load),
                LoadScene);

            var stateTwo = new DualStateButtonState(
                _view.LoadButton,
                Resources.Load<Sprite>(SpritesPath.Stop),
                EditorApplication.ExitPlaymode);

            _view.LoadButton.Initialize(stateOne, stateTwo);
        }

        private void SolveOpenButton()
        {
            if (EditorApplication.isPlaying)
            {
                _view.SetOpenButtonEnable(false);
                return;
            }

            var isSceneActiveScene = EditorSceneManagerHelper.IsActiveScene(_model.SceneData);
            if (isSceneActiveScene) _currentSceneElement = this;
            _view.SetOpenButtonEnable(!isSceneActiveScene);
        }

        private void SelectSceneDataInProject()
        {
            Selection.activeObject = _model.SceneData;
            EditorGUIUtility.PingObject(Selection.activeObject);
        }

        private void ChangeFolder()
        {
            var sceneAssetPath = AssetDatabase.GetAssetPath(_model.SceneData);
            var absolutePath = EditorUtility.OpenFolderPanel("Select folder", "Assets", "");

            if (string.IsNullOrEmpty(absolutePath)) return;

            if (!absolutePath.StartsWith(Application.dataPath))
            {
                Debug.LogError(
                    $"Suspicious action not supported | Path was outside the Assets folder | {absolutePath}");
                return;
            }

            var sceneAssetFullPath = Path.GetFullPath(sceneAssetPath);
            var normalizedSceneAssetPath = Path.GetDirectoryName(sceneAssetFullPath);
            var normalizedAbsolutePath = Path.GetFullPath(absolutePath);

            if (normalizedSceneAssetPath == normalizedAbsolutePath) return;

            var relativeNewFolderPath = "Assets" + absolutePath[Application.dataPath.Length..];
            var fileName = Path.GetFileName(sceneAssetPath);
            var newAssetPath = Path.Combine(relativeNewFolderPath, fileName);
            AssetDatabase.MoveAsset(sceneAssetPath, newAssetPath);
        }

        private void InitializeBuildSettingsButton()
        {
            var stateOne = new DualStateButtonState(
                _view.BuildSettingsButton,
                Resources.Load<Sprite>(SpritesPath.AddBuildSettings),
                AddToBuildSettings);

            var stateTwo = new DualStateButtonState(
                _view.BuildSettingsButton,
                Resources.Load<Sprite>(SpritesPath.RemoveBuildSettings),
                RemoveFromBuildSettings);

            _view.BuildSettingsButton.Initialize(stateOne, stateTwo);
        }

        private void RenameAsset(BlurEvent evt)
        {
            if (_foldout.Title == _model.SceneData.name) return;

            if (!CanRenameAsset() || string.IsNullOrEmpty(_foldout.Title) || string.IsNullOrWhiteSpace(_foldout.Title))
            {
                _foldout.Title = _model.SceneData.name;
                return;
            }

            AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(_model.SceneData), _foldout.Title);

            var index = ResourcesLocator.GetQuickLoadElements().Elements.FindIndex(x => x == _model);
            var quickLoadController = Services.Get<SceneElementsController>();
            quickLoadController.ScrollToItemIndex(index);
        }

        private bool CanRenameAsset()
        {
            var assetPath = EditorSceneManagerHelper.GetSceneAssetPath(_model.SceneData);
            assetPath = Path.Combine(Path.GetDirectoryName(assetPath) ?? string.Empty, _foldout.Title + ".unity");
            var asset = AssetDatabase.LoadAssetAtPath<SceneAsset>(assetPath);

            if (!asset) return true;

            Debug.LogWarning($"SceneAsset with name already exists | {assetPath}");
            return false;
        }

        private void BindViewManager(AttachToPanelEvent evt)
        {
            _viewManager = this.Root().Q<ViewManager>();
        }

        private void LoadScene()
        {
            SessionState.SetInt(CurrentActiveScene, _model.SceneData.GetInstanceID());
            SceneManagerHelper.LoadScene(_model.SceneData);
            EditorApplication.playModeStateChanged += OnExitPlayModeSwitchToStateOne;
        }

        private void OnExitPlayModeSwitchToStateOne(PlayModeStateChange obj)
        {
            if (obj != PlayModeStateChange.ExitingPlayMode) return;
            EditorApplication.playModeStateChanged -= OnExitPlayModeSwitchToStateOne;

            var prefActiveSceneID = SessionState.GetInt(CurrentActiveScene, -1);
            var modelSceneID = _model.SceneData.GetInstanceID();
            if (prefActiveSceneID == modelSceneID) SessionState.EraseInt(CurrentActiveScene);
            _view.SwitchLoadButtonState(false);
        }

        private void OpenScene()
        {
            if (_currentSceneElement != null)
            {
                _currentSceneElement._view.SetOpenButtonEnable(true);
                _currentSceneElement._view.SetOpenAdditiveButtonEnable(true);
                _currentSceneElement._view.SwitchOpenAdditiveButtonState(false);
            }

            _currentSceneElement = this;

            _view.SetOpenButtonEnable(false);
            _view.SetOpenAdditiveButtonEnable(false);
            _view.SwitchOpenAdditiveButtonState(false);

            EditorSceneManagerHelper.OpenScene(_model.SceneData);
        }

        private void TransitionViewToTagsManager()
        {
            _viewManager.TransitionToFirstViewOfType<TagsManagerController>(new TagsManagerViewArgs(_model));
        }

        private void SessionStateCacheFoldoutValue(ClickEvent _)
        {
            var id = _model.SceneData.GetInstanceID().ToString();
            SessionState.SetBool(id, _foldout.IsOpen);
        }
    }
}