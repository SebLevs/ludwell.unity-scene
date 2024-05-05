using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ludwell.Scene.Editor
{
    public class QuickLoadElementController
    {
        private const string DataPresetSectionName = "preset__data";

        private QuickLoadElementData _model = new();
        private VisualElement _view;

        private ViewManager _viewManager;

        private readonly TagsShelfController _tagsShelfController;
        private readonly PresetSectionController _sectionController;

        public QuickLoadElementController(VisualElement view)
        {
            _view = view;
            _tagsShelfController = new TagsShelfController(view, _ => TransitionViewToTagsManager());

            _sectionController =
                new PresetSectionController(_model, view.Q(DataPresetSectionName), TransitionViewToDataPreset);

            view.RegisterCallback<AttachToPanelEvent>(_ => { _viewManager = view.Root().Q<ViewManager>(); });

            view.Q<Toggle>().RegisterCallback<MouseUpEvent>(SaveQuickLoadElements);
        }

        ~QuickLoadElementController()
        {
            _view.Q<Toggle>().UnregisterCallback<MouseUpEvent>(SaveQuickLoadElements);
        }

        public void InitializeLoadButton(DualStateButton dualStateButton)
        {
            var stateOne = new DualStateButtonState(
                dualStateButton,
                LoadScene,
                Resources.Load<Sprite>(SpritesPath.LoadIcon));

            var stateTwo = new DualStateButtonState(
                dualStateButton,
                EditorApplication.ExitPlaymode,
                Resources.Load<Sprite>(SpritesPath.StopIcon));

            dualStateButton.Initialize(stateOne, stateTwo);
        }

        public void InitializeOpenButton(ButtonWithIcon buttonWithIcon)
        {
            buttonWithIcon.SetIcon(Resources.Load<Sprite>(SpritesPath.OpenIcon));
            buttonWithIcon.clicked += OpenScene;
        }

        public void UpdateData(QuickLoadElementData data)
        {
            _model = data;
        }

        public void UpdateIsOpen(ChangeEvent<bool> evt)
        {
            _model.IsOpen = evt.newValue;
        }

        public void UpdateName(ChangeEvent<string> evt)
        {
            _model.Name = evt.newValue;
        }

        public void SelectSceneDataInProject()
        {
            Selection.activeObject = _model.SceneData;
            EditorGUIUtility.PingObject(Selection.activeObject);
        }

        public void UpdateTagsContainer()
        {
            _tagsShelfController.UpdateData(_model);
            _tagsShelfController.Populate();
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

        public void SetSelectedDataPreset(QuickLoadElementView view)
        {
            // todo: refactor into a preset container MVC for both here and manager?
            view.SetSelectedDataPreset(_model.DataPreset.SelectedPreset == null
                ? "No data preset selected"
                : _model.DataPreset.SelectedPreset.Label);
        }

        private void TransitionViewToTagsManager()
        {
            _viewManager.TransitionToFirstViewOfType<TagsManagerController>(new TagsManagerViewArgs(_model));
        }

        private void TransitionViewToDataPreset()
        {
            _viewManager.TransitionToFirstViewOfType<PresetManagerController>(
                new PresetManagerViewArgs(_model, _model.DataPreset));
        }

        private void SaveQuickLoadElements(MouseUpEvent evt)
        {
            DataFetcher.SaveQuickLoadElementsDelayed();
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