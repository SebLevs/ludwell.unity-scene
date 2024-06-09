using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace Ludwell.Scene.Editor
{
    public class SceneDataController : AViewable
    {
        private const string StartingSceneObjectFieldName = "launcher__starting-scene";
        private const string LoadSceneButtonName = "button__load";
        private const string OpenSceneButtonName = "button__open";

        private const string FoldoutStartingSceneName = "foldout__starting-scene";
        private const string FoldoutCoreScenesName = "foldout__core-scenes";

        private readonly ViewManager _viewManager;

        private readonly VisualElement _root;
        private SceneDataView _view;
        private readonly QuickLoadController _quickLoadController;

        private ObjectField _startingSceneObjectField;

        private DualStateButton _loadSceneButton;
        private ButtonWithIcon _openSceneButton;

        public SceneDataController(VisualElement parent) : base(parent)
        {
            _root = parent.Q(nameof(SceneDataView));
            _view = new SceneDataView(_root, UpdateStartingScene, UpdatePersistentScene, UpdateLoadingScene);

            _startingSceneObjectField = _root.Q(StartingSceneObjectFieldName).Q<ObjectField>();
            InitializeLoadButton();
            InitializeOpenButton();
            SetButtonsEnable(DataFetcher.GetCoreScenes().StartingScene);

            CloseFoldouts();

            _quickLoadController = new QuickLoadController(_root);
            OnShow = AddRefreshViewSignal;
            OnHide = RemoveRefreshViewSignal;
        }

        protected override void Show(ViewArgs args)
        {
            _view.Show();
        }

        protected override void Hide()
        {
            _view.Hide();
        }

        private void UpdateStartingScene(ChangeEvent<Object> evt)
        {
            SetButtonsEnable(evt.newValue != null);

            var coreScenes = DataFetcher.GetCoreScenes();
            coreScenes.StartingScene = evt.newValue as SceneData;
            DataFetcher.SaveCoreScenes();
        }

        private void SetButtonsEnable(bool state)
        {
            _loadSceneButton.SetEnabled(state);
            _openSceneButton.SetEnabled(state);
        }

        private void UpdatePersistentScene(ChangeEvent<Object> evt)
        {
            var coreScenes = DataFetcher.GetCoreScenes();
            coreScenes.PersistentScene = evt.newValue as SceneData;
            DataFetcher.SaveCoreScenes();
        }

        private void UpdateLoadingScene(ChangeEvent<Object> evt)
        {
            var coreScenes = DataFetcher.GetCoreScenes();
            coreScenes.LoadingScene = evt.newValue as SceneData;
            DataFetcher.SaveCoreScenes();
        }

        private void InitializeLoadButton()
        {
            _loadSceneButton = _root.Q<DualStateButton>(LoadSceneButtonName);

            var stateOne = new DualStateButtonState(
                _loadSceneButton,
                LoadScene,
                Resources.Load<Sprite>(SpritesPath.LoadIcon));

            var stateTwo = new DualStateButtonState(
                _loadSceneButton,
                EditorApplication.ExitPlaymode,
                Resources.Load<Sprite>(SpritesPath.StopIcon));

            _loadSceneButton.Initialize(stateOne, stateTwo);
        }

        private void InitializeOpenButton()
        {
            _openSceneButton = _root.Q<ButtonWithIcon>(OpenSceneButtonName);
            _openSceneButton.SetIcon(Resources.Load<Sprite>(SpritesPath.OpenIcon));
            _openSceneButton.clicked += OpenScene;
        }

        private void LoadScene()
        {
            if (_startingSceneObjectField.value == null) return;
            QuickLoadSceneDataManager.LoadScene(_startingSceneObjectField.value as SceneData);
            EditorApplication.playModeStateChanged += OnExitPlayModeSwitchToStateOne;
        }

        private void OnExitPlayModeSwitchToStateOne(PlayModeStateChange obj)
        {
            if (obj != PlayModeStateChange.ExitingPlayMode) return;
            EditorApplication.playModeStateChanged -= OnExitPlayModeSwitchToStateOne;
            var dualStateButton = _root.Q<DualStateButton>();
            dualStateButton.SwitchState(dualStateButton.StateOne);
        }

        private void OpenScene()
        {
            if (_startingSceneObjectField.value == null) return;
            SceneDataManagerEditorApplication.OpenScene(_startingSceneObjectField.value as SceneData);
        }

        private void CloseFoldouts()
        {
            _root.Q<Foldout>(FoldoutStartingSceneName).value = false;
            _root.Q<Foldout>(FoldoutCoreScenesName).value = false;
        }

        private void AddRefreshViewSignal()
        {
            Signals.Add<UISignals.RefreshView>(_quickLoadController.ForceRebuildListView);
        }

        private void RemoveRefreshViewSignal()
        {
            Signals.Remove<UISignals.RefreshView>(_quickLoadController.ForceRebuildListView);
        }
    }
}