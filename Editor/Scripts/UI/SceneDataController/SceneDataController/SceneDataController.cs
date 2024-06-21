using UnityEditor;
using UnityEditor.SceneManagement;
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
        
        private DelayedEditorUpdateAction _delayedRefreshView;

        public SceneDataController(VisualElement parent) : base(parent)
        {
            _root = parent.Q(nameof(SceneDataView));
            _view = new SceneDataView(_root, UpdateStartingScene, UpdatePersistentScene, UpdateLoadingScene);

            _startingSceneObjectField = _root.Q(StartingSceneObjectFieldName).Q<ObjectField>();
            InitializeLoadButton();
            InitializeOpenButton();
            SetButtonsEnable(ResourcesFetcher.GetCoreScenes().StartingScene);

            CloseFoldouts();

            _quickLoadController = new QuickLoadController(_root);
            OnShow = AddRefreshViewSignal;
            OnHide = RemoveRefreshViewSignal;

            _delayedRefreshView = new DelayedEditorUpdateAction(0.2f, DispatchRefreshView);
        }
        
        protected override void Show(ViewArgs args)
        {
            EditorApplication.playModeStateChanged += HandlePlayModeStateChange;
            EditorSceneManager.sceneOpened += HandleSceneOpened;
            EditorSceneManager.sceneClosed += HandleSceneClosed;
            EditorSceneManager.activeSceneChangedInEditMode += HandleActiveSceneChange;
            _view.Show();
        }

        protected override void Hide()
        {
            EditorApplication.playModeStateChanged -= HandlePlayModeStateChange;
            EditorSceneManager.sceneOpened -= HandleSceneOpened;
            EditorSceneManager.sceneClosed -= HandleSceneClosed;
            EditorSceneManager.activeSceneChangedInEditMode -= HandleActiveSceneChange;
            _view.Hide();
        }
        
        private void HandleSceneOpened(UnityEngine.SceneManagement.Scene scene, OpenSceneMode mode)
        {
            if (EditorApplication.isPlaying) return;
            if (mode == OpenSceneMode.Single) return;
            _delayedRefreshView.StartOrRefresh();
        }
        
        private void HandleSceneClosed(UnityEngine.SceneManagement.Scene scene)
        {
            if (EditorApplication.isPlaying) return;
            _delayedRefreshView.StartOrRefresh();
            Debug.LogError("Closed");
        }
        
        private void HandleActiveSceneChange(UnityEngine.SceneManagement.Scene arg0, UnityEngine.SceneManagement.Scene arg1)
        {
            if (EditorApplication.isPlaying) return;
            _delayedRefreshView.StartOrRefresh();
        }
        
        private void HandlePlayModeStateChange(PlayModeStateChange playModeStateChange)
        {
            _delayedRefreshView.StartOrRefresh();
        }

        private void DispatchRefreshView()
        {
            Debug.LogError("Refresh");
            Signals.Dispatch<UISignals.RefreshView>();
        }

        private void UpdateStartingScene(ChangeEvent<Object> evt)
        {
            SetButtonsEnable(evt.newValue != null);

            var coreScenes = ResourcesFetcher.GetCoreScenes();
            coreScenes.StartingScene = evt.newValue as SceneData;
            ResourcesFetcher.SaveCoreScenes();
        }

        private void SetButtonsEnable(bool state)
        {
            _loadSceneButton.SetEnabled(state);
            _openSceneButton.SetEnabled(state);
        }

        private void UpdatePersistentScene(ChangeEvent<Object> evt)
        {
            var coreScenes = ResourcesFetcher.GetCoreScenes();
            coreScenes.PersistentScene = evt.newValue as SceneData;
            ResourcesFetcher.SaveCoreScenes();
        }

        private void UpdateLoadingScene(ChangeEvent<Object> evt)
        {
            var coreScenes = ResourcesFetcher.GetCoreScenes();
            coreScenes.LoadingScene = evt.newValue as SceneData;
            ResourcesFetcher.SaveCoreScenes();
        }

        private void InitializeLoadButton()
        {
            _loadSceneButton = _root.Q<DualStateButton>(LoadSceneButtonName);

            var stateOne = new DualStateButtonState(
                _loadSceneButton,
                LoadScene,
                Resources.Load<Sprite>(SpritesPath.Load));

            var stateTwo = new DualStateButtonState(
                _loadSceneButton,
                EditorApplication.ExitPlaymode,
                Resources.Load<Sprite>(SpritesPath.Stop));

            _loadSceneButton.Initialize(stateOne, stateTwo);
        }

        private void InitializeOpenButton()
        {
            _openSceneButton = _root.Q<ButtonWithIcon>(OpenSceneButtonName);
            _openSceneButton.SetIcon(Resources.Load<Sprite>(SpritesPath.Open));
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