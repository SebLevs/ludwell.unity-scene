using System;
using System.IO;
using Ludwell.UIToolkitElements.Editor;
using Ludwell.UIToolkitUtilities;
using Ludwell.UIToolkitUtilities.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Ludwell.SceneManagerToolkit.Editor
{
    internal class SceneElementController : VisualElement, IDisposable, IListViewVisualElement<SceneAssetDataBinder>
    {
        private static SceneElementController _currentSceneElement;

        private const string CurrentActiveScene = "currentActiveScene";

        private readonly SceneElementView _view;
        private SceneAssetDataBinder _model;

        private readonly FoldoutController _foldout;
        private readonly TagsShelfController _tagsShelfController;
        private ViewManager _viewManager;

        public Scene Scene => SceneManager.GetSceneByPath(_model.Data.Path);

        public void SetOpenState(bool state) => _foldout.IsOpen = state;

        public bool IsTextFieldValue(string value) => value == _foldout.TitleTextField.value;

        public void FocusTextField() => _foldout.FocusTextField();

        public bool IsActiveScene() => EditorSceneManagerHelper.IsActiveScene(_model.Data.Path);

        public SceneElementController()
        {
            _view = new SceneElementView(this);
            _view.SetActiveButton.clicked += SetAsActiveScene;
            InitializeLoadAdditiveButton();
            InitializeOpenAdditiveButton();
            _view.OpenButton.clicked += OpenScene;
            InitializeLoadButton();
            _view.PingButton.clicked += SelectSceneDataInProject;
            _view.DirectoryChangeButton.clicked += ChangeFolder;
            InitializeBuildSettingsButton();
            InitializeEnabledInBuildSettingsButton();
            InitializeAddressablesButton();

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

        public void CacheData(SceneAssetDataBinder data)
        {
            _model = data;
        }

        public void BindElementToCachedData()
        {
        }

        public void SetElementFromCachedData()
        {
            SetFoldoutValueFromSavedState();
            _foldout.Title = _model.Data.Name;
            _view.SetIconAssetOutsideAssets(EditorSceneManagerHelper.IsPathOutsideAssets(_model.Data.Path));

            _view.SetPathTooltip(_model.Data.Path);

            SetTagsContainer();

            SolveSetActiveButton();
            SolveLoadAdditiveButton();
            SolveOpenAdditiveButton();
            SolveOpenButton();
            SetLoadButtonState();
            _view.SetDirectoryChangeButtonEnable(!EditorApplication.isPlaying);
            SolveBuildSettingsButton();
            SolveEnabledInBuildSettingsButton();
            SolveAddressablesButton();
        }

        public void AddToBuildSettings()
        {
            EditorSceneManagerHelper.AddSceneToBuildSettings(_model.Data.Path);
        }

        public void RemoveFromBuildSettings()
        {
            EditorSceneManagerHelper.RemoveSceneFromBuildSettings(_model.Data.Path);
        }

        public void EnableInBuildSettings()
        {
            EditorSceneManagerHelper.EnableSceneInBuildSettings(_model.Data.Path, true);
        }

        public void DisableInBuildSettings()
        {
            EditorSceneManagerHelper.EnableSceneInBuildSettings(_model.Data.Path, false);
        }

        public void LoadSceneAdditive()
        {
            EditorSceneManagerHelper.OpenSceneAdditive(_model.Data.Path);
        }

        public void UnloadSceneAdditive()
        {
            if (Scene.isDirty && !EditorSceneManagerHelper.SaveSceneDialogue(this))
            {
                SolveLoadAdditiveButton();
                return;
            }

            EditorSceneManagerHelper.CloseScene(_model.Data.Path, false);
        }

        public void OpenSceneAdditive()
        {
            EditorSceneManagerHelper.OpenSceneAdditive(_model.Data.Path);
        }

        public void RemoveSceneAdditive()
        {
            if (Scene.isDirty && !EditorSceneManagerHelper.SaveSceneDialogue(this))
            {
                SolveOpenAdditiveButton();
                return;
            }

            EditorSceneManagerHelper.RemoveSceneAdditive(_model.Data.Path);
        }

        public void AddToAddressables()
        {
#if USE_ADDRESSABLES_EDITOR
            AddressablesProcessor.AddToAddressables(_model.Data.GUID);
#endif
        }

        public void RemoveFromAddressables()
        {
#if USE_ADDRESSABLES_EDITOR
            AddressablesProcessor.RemoveFromAddressables(_model.Data.AddressableID);
#endif
        }

        private void SolveSetActiveButton()
        {
            var isSceneLoaded = EditorSceneManagerHelper.IsSceneLoaded(_model.Data.Path);
            _view.SetSetActiveButtonEnable(isSceneLoaded && !IsActiveScene());
        }

        private void SolveLoadAdditiveButton()
        {
            var isPlaying = EditorApplication.isPlaying;
            var onlyOneLoadedScene = EditorSceneManagerHelper.DoesHierarchyOnlyHasOneLoadedScene();
            var isActiveScene = _model.Data.Path == SceneManager.GetActiveScene().path;
            var isSceneLoaded = EditorSceneManagerHelper.IsSceneLoaded(_model.Data.Path);
            var isSceneUnloadedInHierarchy = EditorSceneManagerHelper.IsSceneUnloadedInHierarchy(_model.Data.Path);

            var defaultEnabledState = !isPlaying && !(onlyOneLoadedScene && isActiveScene);
            var specificEnabledState = isSceneLoaded || isSceneUnloadedInHierarchy;
            _view.SetLoadAdditiveButtonEnable(defaultEnabledState && specificEnabledState);

            var defaultState = isSceneLoaded && !isSceneUnloadedInHierarchy;
            _view.SwitchLoadAdditiveButtonState(defaultState);
        }

        private void SolveOpenAdditiveButton()
        {
            var isPlaying = EditorApplication.isPlaying;
            var onlyOneLoadedScene = EditorSceneManagerHelper.DoesHierarchyOnlyHasOneLoadedScene();
            var isActiveScene = _model.Data.Path == SceneManager.GetActiveScene().path;
            var isSceneLoaded = EditorSceneManagerHelper.IsSceneLoaded(_model.Data.Path);
            var isSceneUnloadedInHierarchy = EditorSceneManagerHelper.IsSceneUnloadedInHierarchy(_model.Data.Path);

            var defaultEnabledState = !isPlaying && !(onlyOneLoadedScene && isActiveScene);
            var specificEnabledState = true;
            _view.SetOpenAdditiveButtonEnable(defaultEnabledState && specificEnabledState);

            var defaultState = isSceneLoaded || isSceneUnloadedInHierarchy;
            _view.SwitchOpenAdditiveButtonState(defaultState);
        }

        private void SolveBuildSettingsButton()
        {
            _view.SetBuildSettingsButtonButtonEnable(!EditorApplication.isPlaying);
            if (EditorSceneManagerHelper.IsSceneInBuildSettings(_model.Data.Path))
            {
                _view.SwitchBuildSettingsButtonState(true);
                return;
            }

            _view.SwitchBuildSettingsButtonState(false);
        }

        private void SolveEnabledInBuildSettingsButton()
        {
            var isInBuildSettings = EditorSceneManagerHelper.IsSceneInBuildSettings(_model.Data.Path);

            var isPlaying = EditorApplication.isPlaying;
            _view.SetEnabledInBuildSettingsButtonEnable(!(isPlaying || _model.Data.IsAddressable) && isInBuildSettings);

            if (EditorSceneManagerHelper.IsSceneEnabledInBuildSettings(_model.Data.Path))
            {
                _view.SwitchEnabledInBuildSettingsButtonState(true);
                return;
            }

            _view.SwitchEnabledInBuildSettingsButtonState(false);
        }

        private void SolveAddressablesButton()
        {
#if USE_ADDRESSABLES_EDITOR
            _view.SetAddressablesButtonEnable(true);
            if (_model.Data.IsAddressable)
            {
                _view.SwitchAddressablesButtonState(true);
                return;
            }

            _view.SwitchAddressablesButtonState(false);
#else
            _view.SetAddressablesButtonEnable(false);
            _view.SetAddressableButtonTooltipWithoutPackage();
#endif
        }

        private void SetFoldoutValueFromSavedState()
        {
            var id = _model.Data.GUID;
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
            if (SessionState.GetString(CurrentActiveScene, string.Empty) != _model.Data.GUID)
            {
                _view.SwitchLoadButtonState(false);
                return;
            }

            _view.SwitchLoadButtonState(true);
        }

        private void SetAsActiveScene()
        {
            EditorSceneManagerHelper.SetActiveScene(_model.Data.Path);
        }

        private void InitializeLoadAdditiveButton()
        {
            Debug.LogError("Change icons for LoadAdditive");
            var stateOne = new DualStateButtonState(
                _view.LoadAdditiveButton,
                Resources.Load<Sprite>(SpritesPath.LoadAdditive),
                LoadSceneAdditive);

            var stateTwo = new DualStateButtonState(
                _view.LoadAdditiveButton,
                Resources.Load<Sprite>(SpritesPath.UnloadAdditive),
                UnloadSceneAdditive);

            _view.LoadAdditiveButton.Initialize(stateOne, stateTwo);
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

        private void InitializeAddressablesButton()
        {
            var stateOne = new DualStateButtonState(
                _view.AddressablesButton,
                Resources.Load<Sprite>(SpritesPath.AddToAddressables),
                AddToAddressables);

            var stateTwo = new DualStateButtonState(
                _view.AddressablesButton,
                Resources.Load<Sprite>(SpritesPath.RemoveFromAddressables),
                RemoveFromAddressables);

            _view.AddressablesButton.Initialize(stateOne, stateTwo);
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

            var isSceneActiveScene = EditorSceneManagerHelper.IsActiveScene(_model.Data.Path);
            if (isSceneActiveScene) _currentSceneElement = this;
            _view.SetOpenButtonEnable(!isSceneActiveScene);
        }

        private void SelectSceneDataInProject()
        {
            Selection.activeObject = AssetDatabase.LoadAssetAtPath<SceneAsset>(_model.Data.Path);
            EditorGUIUtility.PingObject(Selection.activeObject);
        }

        private void ChangeFolder()
        {
            var absolutePath = EditorUtility.OpenFolderPanel("Select folder", "Assets", "");

            if (string.IsNullOrEmpty(absolutePath)) return;

            if (!absolutePath.StartsWith(Application.dataPath))
            {
                Debug.LogError(
                    $"Suspicious action not supported | Path was outside the Assets folder | {absolutePath}");
                return;
            }

            var sceneAssetFullPath = Path.GetFullPath(_model.Data.Path);
            var normalizedSceneAssetPath = Path.GetDirectoryName(sceneAssetFullPath);
            var normalizedAbsolutePath = Path.GetFullPath(absolutePath);

            if (normalizedSceneAssetPath == normalizedAbsolutePath) return;

            var relativeNewFolderPath = "Assets" + absolutePath[Application.dataPath.Length..];
            var fileName = Path.GetFileName(_model.Data.Path);
            var newAssetPath = Path.Combine(relativeNewFolderPath, fileName);
            AssetDatabase.MoveAsset(_model.Data.Path, newAssetPath);
        }

        private void InitializeBuildSettingsButton()
        {
            var stateOne = new DualStateButtonState(
                _view.BuildSettingsButton,
                Resources.Load<Sprite>(SpritesPath.AddToBuildSettings),
                AddToBuildSettings);

            var stateTwo = new DualStateButtonState(
                _view.BuildSettingsButton,
                Resources.Load<Sprite>(SpritesPath.RemoveBuildSettings),
                RemoveFromBuildSettings);

            _view.BuildSettingsButton.Initialize(stateOne, stateTwo);
        }

        private void InitializeEnabledInBuildSettingsButton()
        {
            var stateOne = new DualStateButtonState(
                _view.EnabledInBuildSettingsButton,
                Resources.Load<Sprite>(SpritesPath.EnableInBuildSettings),
                EnableInBuildSettings);

            var stateTwo = new DualStateButtonState(
                _view.EnabledInBuildSettingsButton,
                Resources.Load<Sprite>(SpritesPath.DisableInBuildSettings),
                DisableInBuildSettings);

            _view.EnabledInBuildSettingsButton.Initialize(stateOne, stateTwo);
        }

        private void RenameAsset(BlurEvent evt)
        {
            if (_foldout.Title == _model.Data.Name) return;

            if (!CanRenameAsset() || string.IsNullOrEmpty(_foldout.Title) || string.IsNullOrWhiteSpace(_foldout.Title))
            {
                _foldout.Title = _model.Data.Name;
                return;
            }

            AssetDatabase.RenameAsset(_model.Data.Path, _foldout.Title);

            var index = SceneAssetDataBinders.Instance.IndexOf(_model);
            var sceneElementsController = Services.Get<SceneElementsController>();
            sceneElementsController.ScrollToItemIndexThenFocusTextField(index);
        }

        private bool CanRenameAsset()
        {
            var assetPath = Path.Combine(Path.GetDirectoryName(_model.Data.Path) ?? string.Empty,
                _foldout.Title + ".unity");
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
            SessionState.SetString(CurrentActiveScene, _model.Data.GUID);
            SceneManagerHelper.LoadScene(_model);
            EditorApplication.playModeStateChanged += OnExitPlayModeSwitchToStateOne;
        }

        private void OnExitPlayModeSwitchToStateOne(PlayModeStateChange obj)
        {
            if (obj != PlayModeStateChange.ExitingPlayMode) return;
            EditorApplication.playModeStateChanged -= OnExitPlayModeSwitchToStateOne;

            var prefActiveSceneID = SessionState.GetString(CurrentActiveScene, string.Empty);
            var modelSceneID = _model.Data.GUID;
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

            EditorSceneManagerHelper.OpenScene(_model.Data.Path);
        }

        private void TransitionViewToTagsManager()
        {
            _viewManager.TransitionToFirstViewOfType<TagsManagerController>(new TagsManagerViewArgs(_model));
        }

        private void SessionStateCacheFoldoutValue(ClickEvent _)
        {
            var id = _model.Data.GUID;
            SessionState.SetBool(id, _foldout.IsOpen);
        }
    }
}
