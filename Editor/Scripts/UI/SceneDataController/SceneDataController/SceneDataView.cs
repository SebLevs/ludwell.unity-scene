using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace Ludwell.Scene.Editor
{
    public class SceneDataView : IViewable
    {
        private const string StartingObjectFieldName = "launcher__main-menu";
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
            
            ViewManager.Instance.Add(this);
        }
        
        ~SceneDataView()
        {
            ViewManager.Instance.Remove(this);
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
            var coreScenes = DataFetcher.GetCoreScenes();
            var mainMenuObjectField = _root.Q(StartingObjectFieldName).Q<ObjectField>();
            mainMenuObjectField.value = coreScenes.LaunchScene;
            mainMenuObjectField.RegisterValueChangedCallback(_onStartingSceneChanged);
        }

        private void InitializePersistentSceneCallback()
        {
            var coreScenes = DataFetcher.GetCoreScenes();
            var mainMenuObjectField = _root.Q(PersistentObjectFieldName).Q<ObjectField>();
            mainMenuObjectField.value = coreScenes.LaunchScene;
            mainMenuObjectField.RegisterValueChangedCallback(_onPersistentSceneChanged);
        }

        private void InitializeLoadingSceneCallback()
        {
            var coreScenes = DataFetcher.GetCoreScenes();
            var mainMenuObjectField = _root.Q(LoadingObjectFieldName).Q<ObjectField>();
            mainMenuObjectField.value = coreScenes.LaunchScene;
            mainMenuObjectField.RegisterValueChangedCallback(_onLoadingSceneChanged);
        }
    }
}