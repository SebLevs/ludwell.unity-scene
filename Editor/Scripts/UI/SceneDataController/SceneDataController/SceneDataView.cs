using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace Ludwell.Scene.Editor
{
    public class SceneDataView
    {
        private const string StartingObjectFieldName = "launcher__starting-scene";
        private const string PersistentObjectFieldName = "core-scene__persistent";
        private const string LoadingObjectFieldName = "core-scene__loading";

        private VisualElement _root;

        private readonly EventCallback<ChangeEvent<Object>> _onStartingSceneChanged;
        private readonly EventCallback<ChangeEvent<Object>> _onPersistentSceneChanged;
        private readonly EventCallback<ChangeEvent<Object>> _onLoadingSceneChanged;

        public SceneDataView(
            VisualElement parent,
            EventCallback<ChangeEvent<Object>> onStartingSceneChanged,
            EventCallback<ChangeEvent<Object>> onPersistentSceneChanged,
            EventCallback<ChangeEvent<Object>> onLoadingSceneChanged)
        {
            _root = parent.Q(nameof(SceneDataView));

            _onStartingSceneChanged = onStartingSceneChanged;
            _onPersistentSceneChanged = onPersistentSceneChanged;
            _onLoadingSceneChanged = onLoadingSceneChanged;

            InitializeStartingSceneCallback();
            InitializePersistentSceneCallback();
            InitializeLoadingSceneCallback();
        }

        public void Show()
        {
            _root.style.display = DisplayStyle.Flex;
        }

        public void Hide()
        {
            _root.style.display = DisplayStyle.None;
        }

        private void InitializeStartingSceneCallback()
        {
            var coreScenes = ResourcesLocator.GetCoreScenes();
            var mainMenuObjectField = _root.Q(StartingObjectFieldName).Q<ObjectField>();
            mainMenuObjectField.value = coreScenes.StartingScene;
            mainMenuObjectField.RegisterValueChangedCallback(_onStartingSceneChanged);
        }

        private void InitializePersistentSceneCallback()
        {
            var coreScenes = ResourcesLocator.GetCoreScenes();
            var mainMenuObjectField = _root.Q(PersistentObjectFieldName).Q<ObjectField>();
            mainMenuObjectField.RegisterValueChangedCallback(_onPersistentSceneChanged);
            mainMenuObjectField.value = coreScenes.PersistentScene;
        }

        private void InitializeLoadingSceneCallback()
        {
            var coreScenes = ResourcesLocator.GetCoreScenes();
            var mainMenuObjectField = _root.Q(LoadingObjectFieldName).Q<ObjectField>();
            mainMenuObjectField.RegisterValueChangedCallback(_onLoadingSceneChanged);
            mainMenuObjectField.value = coreScenes.LoadingScene;
        }
    }
}