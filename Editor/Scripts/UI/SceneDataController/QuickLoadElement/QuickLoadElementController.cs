using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Ludwell.Scene.Editor
{
    public class QuickLoadElementController : VisualElement, IListViewVisualElement<QuickLoadElementData>
    {
        private static QuickLoadElementController _currentQuickLoadElement;

        private const string CurrentActiveScene = "currentActiveScene";

        private QuickLoadElementView _view;

        private FoldoutController _foldout;
        private readonly TagsShelfController _tagsShelfController;
        private ViewManager _viewManager;

        public QuickLoadElementData Model { get; private set; }

        public void SetOpenState(bool state) => _foldout.IsOpen = state;

        public void FocusTextField() => _foldout.FocusTextField();

        public void SetOpenButtonEnable(bool state) => _view.SetOpenButtonEnable(state);
        public void SetOpenAdditiveButtonEnable(bool state) => _view.SetOpenAdditiveButtonEnable(state);

        public void SwitchOpenAdditiveButtonState(bool state) => _view.SwitchOpenAdditiveButtonState(state);

        public QuickLoadElementController()
        {
            _view = new QuickLoadElementView(this);
            InitializeBuildSettingsButton();
            InitializeOpenAdditiveButton();
            _view.OpenButton.clicked += OpenScene;
            InitializeLoadButton();
            _view.PingButton.clicked += SelectSceneDataInProject;
            _view.DirectoryChangeButton.clicked += ChangeFolder;

            _foldout = new FoldoutController(this, false);
            _foldout.TitleTextField.RegisterCallback<KeyDownEvent>(evt =>
            {
                if (evt.keyCode == KeyCode.Return) UpdateAndSaveAssetName(_foldout.Title);
            });
            _foldout.TitleTextField.RegisterCallback<BlurEvent>(_ =>
            {
                if (_foldout.Title == Model.SceneData.name) return;
                _foldout.Title = Model.SceneData.name;
            });

            _tagsShelfController = new TagsShelfController(this, TransitionViewToTagsManager);
            RegisterCallback<AttachToPanelEvent>(_ => { _viewManager = this.Root().Q<ViewManager>(); });

            RegisterCallback<ClickEvent>(SessionStateCacheFoldoutValue);
        }

        public void CacheData(QuickLoadElementData data)
        {
            Model = data;
        }

        public void BindElementToCachedData()
        {
        }

        public void SetElementFromCachedData()
        {
            SetFoldoutValueFromSavedState();
            _foldout.Title = Model.SceneData.Name;
            _view.SetIconAssetOutsideAssets(Model.IsOutsideAssetsFolder);

            _view.SetPathTooltip(AssetDatabase.GetAssetPath(Model.SceneData));

            SetTagsContainer();

            SolveBuildSettingsButton();
            SolveOpenAdditiveButton();
            SolveOpenButton();
            SetLoadButtonState();
            _view.SetDirectoryChangeButtonEnable(!EditorApplication.isPlaying);
        }

        public void SolveOpenAdditiveButton()
        {
            _view.SetOpenAdditiveButtonEnable(!EditorApplication.isPlaying);

            var sceneCount = SceneManager.sceneCount;
            // todo: optimize
            var asset = Path.ChangeExtension(AssetDatabase.GetAssetOrScenePath(Model.SceneData), ".unity");
            for (var i = 0; i < sceneCount; i++)
            {
                var scene = EditorSceneManager.GetSceneAt(i);

                if (scene.path != asset) continue;

                if (scene == SceneManager.GetActiveScene())
                {
                    _view.SetOpenAdditiveButtonEnable(false);
                    _view.SwitchOpenAdditiveButtonState(false);
                    return;
                }

                _view.SwitchOpenAdditiveButtonState(true);
                return;
            }

            _view.SwitchOpenAdditiveButtonState(false);
        }

        private void SolveBuildSettingsButton()
        {
            _view.SetBuildSettingsButtonButtonEnable(!EditorApplication.isPlaying);
            var path = SceneDataManagerEditorApplication.GetSceneAssetPath(Model.SceneData);
            if (SceneDataManagerEditorApplication.IsSceneInBuildSettings(path))
            {
                _view.SwitchBuildSettingsButtonState(true);
                return;
            }

            _view.SwitchBuildSettingsButtonState(false);
        }

        private void SetFoldoutValueFromSavedState()
        {
            var id = Model.SceneData.GetInstanceID().ToString();
            var oldState = SessionState.GetBool(id, false);
            SetOpenState(oldState);
        }

        private void SetTagsContainer()
        {
            _tagsShelfController.UpdateData(Model);
            _tagsShelfController.Populate();
        }

        private void SetLoadButtonState()
        {
            if (!EditorApplication.isPlaying) return;
            if (SessionState.GetInt(CurrentActiveScene, -1) != Model.SceneData.GetInstanceID())
            {
                _view.SwitchLoadButtonState(false);
                return;
            }

            _view.SwitchLoadButtonState(true);
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

            // todo: optimize
            var scenePath = Path.ChangeExtension(AssetDatabase.GetAssetOrScenePath(Model.SceneData), ".unity");
            var isSceneActiveScene = SceneManager.GetActiveScene() == SceneManager.GetSceneByPath(scenePath);
            if (isSceneActiveScene) _currentQuickLoadElement = this;
            _view.SetOpenButtonEnable(!isSceneActiveScene);
        }

        private void SelectSceneDataInProject()
        {
            Selection.activeObject = Model.SceneData;
            EditorGUIUtility.PingObject(Selection.activeObject);
        }

        private void ChangeFolder()
        {
            var sceneAssetPath = AssetDatabase.GetAssetPath(Model.SceneData);
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

        private void LoadScene()
        {
            SessionState.SetInt(CurrentActiveScene, Model.SceneData.GetInstanceID());
            QuickLoadSceneDataManager.LoadScene(Model.SceneData);
            EditorApplication.playModeStateChanged += OnExitPlayModeSwitchToStateOne;
        }

        private void OnExitPlayModeSwitchToStateOne(PlayModeStateChange obj)
        {
            if (obj != PlayModeStateChange.ExitingPlayMode) return;
            EditorApplication.playModeStateChanged -= OnExitPlayModeSwitchToStateOne;

            var prefActiveSceneID = SessionState.GetInt(CurrentActiveScene, -1);
            var modelSceneID = Model.SceneData.GetInstanceID();
            if (prefActiveSceneID == modelSceneID) SessionState.EraseInt(CurrentActiveScene);
            _view.SwitchLoadButtonState(false);
        }

        private void OpenScene()
        {
            if (_currentQuickLoadElement != null)
            {
                _currentQuickLoadElement._view.SetOpenButtonEnable(true);
                _currentQuickLoadElement._view.SetOpenAdditiveButtonEnable(true);
                _currentQuickLoadElement.SwitchOpenAdditiveButtonState(false);
            }

            _currentQuickLoadElement = this;

            _view.SetOpenButtonEnable(false);
            _view.SetOpenAdditiveButtonEnable(false);
            _view.SwitchOpenAdditiveButtonState(false);

            SceneDataManagerEditorApplication.OpenScene(Model.SceneData);
            Signals.Dispatch<UISignals.RefreshView>();
        }

        private void AddToBuildSettings()
        {
            SceneDataManagerEditorApplication.AddSceneToBuildSettings(Model.SceneData);
        }

        private void RemoveFromBuildSettings()
        {
            SceneDataManagerEditorApplication.RemoveSceneFromBuildSettings(Model.SceneData);
        }

        private void OpenSceneAdditive()
        {
            SceneDataManagerEditorApplication.OpenSceneAdditive(Model.SceneData);
        }

        private void RemoveSceneAdditive()
        {
            SceneDataManagerEditorApplication.RemoveSceneAdditive(Model.SceneData);
        }

        private void TransitionViewToTagsManager(ClickEvent _)
        {
            _viewManager.TransitionToFirstViewOfType<TagsManagerController>(new TagsManagerViewArgs(Model));
        }

        private void SessionStateCacheFoldoutValue(ClickEvent _)
        {
            var id = Model.SceneData.GetInstanceID().ToString();
            SessionState.SetBool(id, _foldout.IsOpen);
        }

        private void UpdateAndSaveAssetName(string value)
        {
            if (value == Model.SceneData.name) return;

            AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(Model.SceneData), _foldout.Title);
            ResourcesLocator.GetQuickLoadElements().Elements.Sort();
            Signals.Dispatch<UISignals.RefreshView>();

            var quickLoadController = ResourcesLocator.QuickLoadController;
            var index = ResourcesLocator.GetQuickLoadElements().Elements.FindIndex(x => x == Model);
            quickLoadController.ScrollToItemIndex(index);
            _foldout.FocusTextField();
        }
    }
}