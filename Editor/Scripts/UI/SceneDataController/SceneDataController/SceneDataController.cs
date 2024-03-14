using Ludwell.Scene.Editor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ludwell.Scene
{
    public class SceneDataController
    {
        private const string MainMenuButtonsName = "main-menu__buttons";
        private const string MainMenuObjectFieldName = "launcher__main-menu";

        private readonly VisualElement _root;
        private SceneDataView _view;
        private readonly QuickLoadController _quickLoadController;

        public SceneDataController(VisualElement parent)
        {
            _root = parent.Q(nameof(SceneDataView));
            _view = new SceneDataView(_root, UpdateStartingScene, UpdatePersistentScene, UpdateLoadingScene);
            
            InitMainMenuButtons();
            
            _quickLoadController = new QuickLoadController(_root);
        }

        private void UpdateStartingScene(ChangeEvent<Object> evt)
        {
            var coreScenes = DataFetcher.GetCoreScenes();
            coreScenes.LaunchScene = evt.newValue as SceneData;
            DataFetcher.SaveEveryScriptable();
        }
        
        private void UpdatePersistentScene(ChangeEvent<Object> evt)
        {
            var coreScenes = DataFetcher.GetCoreScenes();
            coreScenes.PersistentScene = evt.newValue as SceneData;
            DataFetcher.SaveEveryScriptable();
        }
        
        private void UpdateLoadingScene(ChangeEvent<Object> evt)
        {
            var coreScenes = DataFetcher.GetCoreScenes();
            coreScenes.LoadingScene = evt.newValue as SceneData;
            DataFetcher.SaveEveryScriptable();
        }

        private void InitMainMenuButtons()
        {
            var mainMenuButtons = _root.Q<EditorSceneDataButtons>(MainMenuButtonsName);
            var objectField = _root.Q(MainMenuObjectFieldName).Q<ObjectField>();

            mainMenuButtons.AddAction(ButtonType.Load, () =>
            {
                if (objectField.value == null) return;
                SceneDataManagerEditorApplication.LoadScene(objectField.value as SceneData);
            });

            mainMenuButtons.AddAction(ButtonType.Open, () =>
            {
                if (objectField.value == null) return;
                SceneDataManagerEditorApplication.OpenScene(objectField.value as SceneData);
            });
        }
    }
}