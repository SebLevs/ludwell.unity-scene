using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ludwell.Scene.Editor
{
    public class SceneDataController : IViewable
    {
        private const string MainMenuButtonsName = "main-menu__buttons";
        private const string MainMenuObjectFieldName = "launcher__starting-scene";
        
        private const string FoldoutStartingSceneName = "foldout__starting-scene";
        private const string FoldoutCoreScenesName = "foldout__core-scenes";

        private readonly ViewManager _viewManager;
        
        private readonly VisualElement _root;
        private SceneDataView _view;
        private readonly QuickLoadController _quickLoadController;

        public SceneDataController(VisualElement parent)
        {
            _root = parent.Q(nameof(SceneDataView));
            _view = new SceneDataView(_root, UpdateStartingScene, UpdatePersistentScene, UpdateLoadingScene);
            
            InitMainMenuButtons();

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

        private void InitMainMenuButtons()
        {
            var mainMenuButtons = _root.Q<EditorSceneDataButtons>(MainMenuButtonsName);
            var objectField = _root.Q(MainMenuObjectFieldName).Q<ObjectField>();

            mainMenuButtons.AddAction(ButtonType.Load, () =>
            {
                if (objectField.value == null) return;
                SceneDataManagerEditorApplication.OpenScene(objectField.value as SceneData);

                var persistentScene = DataFetcher.GetCoreScenes().PersistentScene;
                SceneDataManagerEditorApplication.OpenSceneAdditive(persistentScene);
                EditorApplication.EnterPlaymode();

                EditorApplication.playModeStateChanged += OnEnteredEditModeRemovePersistent;
            });
            
            mainMenuButtons.AddAction(ButtonType.Open, () =>
            {
                if (objectField.value == null) return;
                SceneDataManagerEditorApplication.OpenScene(objectField.value as SceneData);
            });
        }

        private void OnEnteredEditModeRemovePersistent(PlayModeStateChange obj)
        {
            if (obj != PlayModeStateChange.EnteredEditMode) return;
            RemovePersistentScene();
        }

        private void RemovePersistentScene()
        {
            EditorApplication.playModeStateChanged -= OnEnteredEditModeRemovePersistent;
            var persistentScene = DataFetcher.GetCoreScenes().PersistentScene;
            SceneDataManagerEditorApplication.CloseScene(persistentScene, true);
        }

        private void CloseFoldouts()
        {
            _root.Q<Foldout>(FoldoutStartingSceneName).value = false;
            _root.Q<Foldout>(FoldoutCoreScenesName).value = false;
        }
    }
}