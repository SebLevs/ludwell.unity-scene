using Ludwell.Architecture;
using Ludwell.EditorUtilities;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using SceneRuntime = UnityEngine.SceneManagement.Scene;

namespace Ludwell.Scene.Editor
{
    internal class SceneElementsListViewRefresh
    {
        private readonly VisualElement _root;

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

        public SceneElementsListViewRefresh(VisualElement parent)
        {
            _root = parent;

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

        public void StartOrRefreshDelayedRebuild()
        {
            _delayedRebuild.StartOrRefresh();
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
