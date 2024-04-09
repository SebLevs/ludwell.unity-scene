using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ludwell.Scene.Editor
{
    public class SceneDataController : IViewable
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

        private ObjectField _sceneObjectField;

        public SceneDataController(VisualElement parent)
        {
            _root = parent.Q(nameof(SceneDataView));
            _view = new SceneDataView(_root, UpdateStartingScene, UpdatePersistentScene, UpdateLoadingScene);

            _sceneObjectField = _root.Q(StartingSceneObjectFieldName).Q<ObjectField>();
            InitializeLoadButton();
            InitializeOpenButton();

            CloseFoldouts();

            _quickLoadController = new QuickLoadController(_root);

            _viewManager = parent.Root().Q<ViewManager>();
            _viewManager.Add(this);
        }

        public void Show(ViewArgs args)
        {
            _view.Show();
        }

        public void Hide()
        {
            _view.Hide();
        }

        private void UpdateStartingScene(ChangeEvent<Object> evt)
        {
            var coreScenes = DataFetcher.GetCoreScenes();
            coreScenes.LaunchScene = evt.newValue as SceneData;
            DataFetcher.SaveCoreScenes();
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
            var loadSceneButton = _root.Q<DualStateButton>(LoadSceneButtonName);

            var stateOne = new DualStateButtonState(
                loadSceneButton,
                LoadScene,
                Resources.Load<Sprite>(SpritesPath.LoadIcon));

            var stateTwo = new DualStateButtonState(
                loadSceneButton,
                EditorApplication.ExitPlaymode,
                Resources.Load<Sprite>(SpritesPath.StopIcon));

            loadSceneButton.Initialize(stateOne, stateTwo);
        }

        private void InitializeOpenButton()
        {
            var openSceneButton = _root.Q<ButtonWithIcon>(OpenSceneButtonName);
            openSceneButton.SetIcon(Resources.Load<Sprite>(SpritesPath.OpenIcon));
            openSceneButton.clicked += OpenScene;
        }

        private void LoadScene()
        {
            if (_sceneObjectField.value == null) return;
            QuickLoadSceneDataManager.LoadScene(_sceneObjectField.value as SceneData);
            EditorApplication.playModeStateChanged += OnExitPlayModeSwitchToStateOne;


            // SceneDataManagerEditorApplication.OpenScene(_sceneObjectField.value as SceneData);
            //
            // var persistentScene = DataFetcher.GetCoreScenes().PersistentScene;
            // SceneDataManagerEditorApplication.OpenSceneAdditive(persistentScene);
            // EditorApplication.EnterPlaymode();
            //
            // EditorApplication.playModeStateChanged += QuickLoadSceneDataManager.OnEnteredEditModeRemovePersistent;
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
            if (_sceneObjectField.value == null) return;
            SceneDataManagerEditorApplication.OpenScene(_sceneObjectField.value as SceneData);
        }

        private void CloseFoldouts()
        {
            _root.Q<Foldout>(FoldoutStartingSceneName).value = false;
            _root.Q<Foldout>(FoldoutCoreScenesName).value = false;
        }
    }
}