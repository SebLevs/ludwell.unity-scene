using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using SceneRuntime = UnityEngine.SceneManagement.Scene;

namespace Ludwell.Scene.Editor
{
    public class SceneDataController : AViewable
    {
        private readonly ViewManager _viewManager;

        private readonly VisualElement _root;
        private readonly SceneDataView _view;
        private readonly QuickLoadController _quickLoadController;

        private readonly DelayedEditorUpdateAction _delayedRebuild;

        private void HandleSceneUnloaded(SceneRuntime arg0) => _delayedRebuild.StartOrRefresh();

        private void HandleSceneLoaded(SceneRuntime arg0, LoadSceneMode arg1) => _delayedRebuild.StartOrRefresh();

        private void HandleSceneOpened(SceneRuntime scene, OpenSceneMode mode) => _delayedRebuild.StartOrRefresh();

        private void HandleActiveSceneChangeRuntime(SceneRuntime arg0, SceneRuntime arg1) =>
            _delayedRebuild.StartOrRefresh();

        private void HandleActiveSceneChangeEditor(SceneRuntime arg0, SceneRuntime arg1) =>
            _delayedRebuild.StartOrRefresh();

        private void HandleSceneClosed(SceneRuntime scene) => _delayedRebuild.StartOrRefresh();

        private void HandlePlayModeStateChange(PlayModeStateChange playModeStateChange) =>
            _delayedRebuild.StartOrRefresh();

        private void DispatchRefreshSignal() => Signals.Dispatch<UISignals.RefreshView>();

        public SceneDataController(VisualElement parent) : base(parent)
        {
            _root = parent.Q(nameof(SceneDataView));
            _view = new SceneDataView(_root);

            _quickLoadController = new QuickLoadController(_root);
            OnShow = AddRefreshViewSignal;
            OnHide = RemoveRefreshViewSignal;

            _delayedRebuild = new DelayedEditorUpdateAction(0.0f, DispatchRefreshSignal);

            SceneManager.sceneLoaded += HandleSceneLoaded;
            SceneManager.sceneUnloaded += HandleSceneUnloaded;
            EditorSceneManager.sceneOpened += HandleSceneOpened;
            EditorSceneManager.sceneClosed += HandleSceneClosed;
            EditorSceneManager.activeSceneChangedInEditMode += HandleActiveSceneChangeEditor;
            SceneManager.activeSceneChanged += HandleActiveSceneChangeRuntime;
            EditorApplication.playModeStateChanged += HandlePlayModeStateChange;

            _root.RegisterCallback<DetachFromPanelEvent>(Dispose);
        }

        protected override void Show(ViewArgs args)
        {
            _view.Show();
            _delayedRebuild.StartOrRefresh();
        }

        protected override void Hide()
        {
            _view.Hide();
        }

        private void AddRefreshViewSignal()
        {
            Signals.Add<UISignals.RefreshView>(_quickLoadController.RebuildActiveListing);
        }

        private void RemoveRefreshViewSignal()
        {
            Signals.Remove<UISignals.RefreshView>(_quickLoadController.RebuildActiveListing);
        }

        private void Dispose(DetachFromPanelEvent evt)
        {
            _root.UnregisterCallback<DetachFromPanelEvent>(Dispose);

            SceneManager.sceneLoaded += HandleSceneLoaded;
            SceneManager.sceneUnloaded += HandleSceneUnloaded;
            EditorSceneManager.sceneOpened += HandleSceneOpened;
            EditorSceneManager.sceneClosed += HandleSceneClosed;
            EditorSceneManager.activeSceneChangedInEditMode += HandleActiveSceneChangeEditor;
            SceneManager.activeSceneChanged += HandleActiveSceneChangeRuntime;
            EditorApplication.playModeStateChanged -= HandlePlayModeStateChange;
        }
    }
}