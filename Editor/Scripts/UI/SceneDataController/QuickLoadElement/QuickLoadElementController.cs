using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ludwell.Scene.Editor
{
    public class QuickLoadElementController
    {
        private ViewManager _viewManager;

        private readonly TagsShelfController _tagsShelfController;

        private QuickLoadElementData _model = new();

        private VisualElement _view;

        private DelayedEditorUpdateAction _updateAssetNameDelayed;

        private string _cachedUpatedName;

        public QuickLoadElementController(VisualElement view)
        {
            _view = view;
            _tagsShelfController = new TagsShelfController(view, _ => InitializeViewTransition());

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

        public void InitializeDirectoryChangeButton(ButtonWithIcon buttonWithIcon)
        {
            buttonWithIcon.SetIcon(Resources.Load<Sprite>(SpritesPath.MoveFile));
            buttonWithIcon.clicked += ChangeFolder;
        }

        public void InitializeLoadButton(DualStateButton dualStateButton)
        {
            var stateOne = new DualStateButtonState(
                dualStateButton,
                LoadScene,
                Resources.Load<Sprite>(SpritesPath.Load));

            var stateTwo = new DualStateButtonState(
                dualStateButton,
                EditorApplication.ExitPlaymode,
                Resources.Load<Sprite>(SpritesPath.Stop));

            dualStateButton.Initialize(stateOne, stateTwo);
        }

        public void InitializeOpenButton(ButtonWithIcon buttonWithIcon)
        {
            buttonWithIcon.SetIcon(Resources.Load<Sprite>(SpritesPath.Open));
            buttonWithIcon.clicked += OpenScene;
        }

        public void UpdateData(QuickLoadElementData data)
        {
            _model = data;
            _cachedUpatedName = _model.Name;
        }

        public void UpdateIsOpen(ChangeEvent<bool> evt)
        {
            _model.IsOpen = evt.newValue;
        }

        public void UpdateTagsContainer()
        {
            _tagsShelfController.UpdateData(_model);
            _tagsShelfController.Populate();
        }

        public void UpdateAndSaveAssetName(string value)
        {
            if (value == _cachedUpatedName)
            {
                _updateAssetNameDelayed.Stop();
                return;
            }

            _cachedUpatedName = value;
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

        private void InitializeViewTransition()
        {
            _viewManager.TransitionToFirstViewOfType<TagsManagerController>(new TagsManagerViewArgs(_model));
        }

        private void UpdateAndSaveAssetDelayed()
        {
            if (_model.SceneData.name == _cachedUpatedName) return;

            AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(_model.SceneData), _cachedUpatedName);
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
            QuickLoadSceneDataManager.LoadScene(_model.SceneData);
            EditorApplication.playModeStateChanged += OnExitPlayModeSwitchToStateOne;
        }

        private void OnExitPlayModeSwitchToStateOne(PlayModeStateChange obj)
        {
            if (obj != PlayModeStateChange.ExitingPlayMode) return;
            EditorApplication.playModeStateChanged -= OnExitPlayModeSwitchToStateOne;
            var dualStateButton = _view.Q<DualStateButton>();
            dualStateButton.SwitchState(dualStateButton.StateOne);
        }

        private void OpenScene()
        {
            SceneDataManagerEditorApplication.OpenScene(_model.SceneData);
        }
    }
}