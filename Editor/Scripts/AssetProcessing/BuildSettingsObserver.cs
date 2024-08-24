using Ludwell.Architecture;
using Ludwell.EditorUtilities;
using UnityEditor;

namespace Ludwell.Scene.Editor
{
    [InitializeOnLoad]
    public class BuildSettingsObserver
    {
        private static DelayedEditorUpdateAction _delayedSceneListChangedCallback;

        static BuildSettingsObserver()
        {
            _delayedSceneListChangedCallback = new DelayedEditorUpdateAction(0, SceneListChangedCallback);
            EditorBuildSettings.sceneListChanged -= _delayedSceneListChangedCallback.StartOrRefresh;
            EditorBuildSettings.sceneListChanged += _delayedSceneListChangedCallback.StartOrRefresh;
        }

        private static void SceneListChangedCallback()
        {
            SceneAssetReferenceDrawer.RepaintInspectorWindows();
            Signals.Dispatch<UISignals.RefreshView>();
        }
    }
}