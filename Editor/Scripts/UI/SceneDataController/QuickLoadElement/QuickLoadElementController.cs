using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Ludwell.Scene.Editor
{
    public class QuickLoadElementController
    {
        private static QuickLoadElementController _loadedScene;

        private const string CurrentActiveScene = "currentActiveScene";

        private readonly TagsShelfController _tagsShelfController;

        private ViewManager _viewManager;

        private QuickLoadElementData _model = new();

        private readonly QuickLoadElementView _view;

        private DelayedEditorUpdateAction _updateAssetNameDelayed;

        private string _cacheUpatedName;

        private DualStateButton _loadButton;
        private DualStateButton _openAdditiveButton;

        public QuickLoadElementController(QuickLoadElementView view)
        {
            _view = view;
            _tagsShelfController = new TagsShelfController(view, TransitionViewToTagsManager);

            view.RegisterCallback<AttachToPanelEvent>(_ => { _viewManager = view.Root().Q<ViewManager>(); });

            _view.Q<Toggle>().RegisterValueChangedCallback(evt =>
            {
                if (evt.newValue == _model.IsOpen) return;
                ResourcesFetcher.SaveQuickLoadElementsDelayed();
            });

            _updateAssetNameDelayed = new DelayedEditorUpdateAction(1f, UpdateAndSaveAssetDelayed);
        }

        public void InitializePingButton(ButtonWithIcon buttonWithIcon)
        {
            buttonWithIcon.SetIcon(Resources.Load<Sprite>(SpritesPath.Ping));
            buttonWithIcon.clicked += SelectSceneDataInProject;
        }

        public void InitializeDirectoryChangeButton(ButtonWithIcon button)
        {
            button.SetIcon(Resources.Load<Sprite>(SpritesPath.MoveFile));
            button.clicked += ChangeFolder;
        }

        public void InitializeLoadButton(DualStateButton button)
        {
            var stateOne = new DualStateButtonState(
                button,
                LoadScene,
                Resources.Load<Sprite>(SpritesPath.Load));

            var stateTwo = new DualStateButtonState(
                button,
                EditorApplication.ExitPlaymode,
                Resources.Load<Sprite>(SpritesPath.Stop));

            button.Initialize(stateOne, stateTwo);
            _loadButton = button;
        }

        public void InitializeOpenButton(ButtonWithIcon button)
        {
            button.SetIcon(Resources.Load<Sprite>(SpritesPath.Open));
            button.clicked += OpenScene;
        }

        public void InitializeOpenAdditiveButton(DualStateButton button)
        {
            var stateOne = new DualStateButtonState(
                button,
                OpenSceneAdditive,
                Resources.Load<Sprite>(SpritesPath.OpenAdditive));

            var stateTwo = new DualStateButtonState(
                button,
                RemoveSceneAdditive,
                Resources.Load<Sprite>(SpritesPath.RemoveAdditive));

            button.Initialize(stateOne, stateTwo);
            _openAdditiveButton = button;
        }

        public void UpdateData(QuickLoadElementData data)
        {
            _model = data;
            _cacheUpatedName = _model.Name;
        }

        public void UpdateIsOpen(ChangeEvent<bool> evt)
        {
            _model.IsOpen = evt.newValue;
        }

        public void SetTagsContainer()
        {
            _tagsShelfController.UpdateData(_model);
            _tagsShelfController.Populate();
        }

        public void UpdateAndSaveAssetName(string value)
        {
            if (value == _cacheUpatedName)
            {
                _updateAssetNameDelayed.Stop();
                return;
            }

            _cacheUpatedName = value;
            _updateAssetNameDelayed.StartOrRefresh();
        }

        public void SetIsOpen(QuickLoadElementView view)
        {
            view.SetIsOpen(_model.IsOpen);
        }

        public void SetSceneData(QuickLoadElementView view)
        {
            view.SetSceneData(_model.SceneData);
        }

        public void SetIconAssetOutsideAssets(QuickLoadElementView view)
        {
            view.SetIconAssetOutsideAssets(_model.IsOutsideAssetsFolder);
        }

        public void SetTooltipAsAssetPath(VisualElement element)
        {
            element.tooltip = AssetDatabase.GetAssetPath(_model.SceneData);
        }

        public void SetDirectoryChangeButton()
        {
            _view.SetDirectoryChangeButtonEnable(!EditorApplication.isPlaying);
        }
        
        public void SetLoadButtonState()
        {
            if (!EditorApplication.isPlaying) return;
            if (EditorPrefs.GetInt(CurrentActiveScene) != _model.SceneData.GetInstanceID())
            {
                _loadButton.SwitchState(_loadButton.StateOne);
                return;
            }

            _loadButton.SwitchState(_loadButton.StateTwo);
        }

        public void SolveOpenButton()
        {
            if (EditorApplication.isPlaying)
            {
                _view.SetOpenButtonEnable(false);
                return;
            }

            // todo: optimize
            var scenePath = Path.ChangeExtension(AssetDatabase.GetAssetOrScenePath(_model.SceneData), ".unity");
            var isSceneActiveScene = SceneManager.GetActiveScene() == SceneManager.GetSceneByPath(scenePath);
            if (isSceneActiveScene) _loadedScene = this;
            _view.SetOpenButtonEnable(!isSceneActiveScene);
        }

        public void SolveOpenAdditiveButton()
        {
            _view.SetOpenAdditiveButtonEnable(!EditorApplication.isPlaying);
            
            var sceneCount = SceneManager.sceneCount;
            // todo: optimize
            var asset = Path.ChangeExtension(AssetDatabase.GetAssetOrScenePath(_model.SceneData), ".unity");
            for (var i = 0; i < sceneCount; i++)
            {
                var scene = EditorSceneManager.GetSceneAt(i);

                if (scene.path != asset) continue;

                if (scene == SceneManager.GetActiveScene())
                {
                    _view.SetOpenAdditiveButtonEnable(false);
                    _openAdditiveButton.SwitchState(_openAdditiveButton.StateOne);
                    return;
                }

                _openAdditiveButton.SwitchState(_openAdditiveButton.StateTwo);
                return;
            }

            _openAdditiveButton.SwitchState(_openAdditiveButton.StateOne);
        }

        private void TransitionViewToTagsManager(ClickEvent _)
        {
            _viewManager.TransitionToFirstViewOfType<TagsManagerController>(new TagsManagerViewArgs(_model));
        }

        private void UpdateAndSaveAssetDelayed()
        {
            if (_model.SceneData.name == _cacheUpatedName) return;

            AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(_model.SceneData), _cacheUpatedName);
            ResourcesFetcher.GetQuickLoadElements().Elements.Sort();
            Signals.Dispatch<UISignals.RefreshView>();

            var quickLoadController = ResourcesFetcher.QuickLoadController;
            var index = ResourcesFetcher.GetQuickLoadElements().Elements.FindIndex(x => x == _model);
            quickLoadController.ScrollToItemIndex(index);
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

        private void LoadScene()
        {
            EditorPrefs.SetInt(CurrentActiveScene, _model.SceneData.GetInstanceID());
            QuickLoadSceneDataManager.LoadScene(_model.SceneData);
            EditorApplication.playModeStateChanged += OnExitPlayModeSwitchToStateOne;
        }

        private void OnExitPlayModeSwitchToStateOne(PlayModeStateChange obj)
        {
            if (obj != PlayModeStateChange.ExitingPlayMode) return;
            EditorApplication.playModeStateChanged -= OnExitPlayModeSwitchToStateOne;

            var prefActiveSceneID = EditorPrefs.GetInt(CurrentActiveScene);
            var modelSceneID = _model.SceneData.GetInstanceID();
            if (prefActiveSceneID == modelSceneID) EditorPrefs.DeleteKey(CurrentActiveScene);
            _loadButton.SwitchState(_loadButton.StateOne);
        }

        private void OpenScene()
        {
            if (_loadedScene != null)
            {
                _loadedScene._view.SetOpenButtonEnable(true);
                _loadedScene._view.SetOpenAdditiveButtonEnable(true);
                _loadedScene._openAdditiveButton.SwitchState(_loadedScene._openAdditiveButton.StateOne);
            }

            _loadedScene = this;

            _view.SetOpenButtonEnable(false);
            _view.SetOpenAdditiveButtonEnable(false);
            _openAdditiveButton.SwitchState(_openAdditiveButton.StateOne);

            SceneDataManagerEditorApplication.OpenScene(_model.SceneData);
            Signals.Dispatch<UISignals.RefreshView>();
        }

        private void OpenSceneAdditive()
        {
            SceneDataManagerEditorApplication.OpenSceneAdditive(_model.SceneData);
        }

        private void RemoveSceneAdditive()
        {
            SceneDataManagerEditorApplication.RemoveSceneAdditive(_model.SceneData);
        }
    }
}