using Ludwell.Architecture;
using Ludwell.EditorUtilities;
using UnityEditor;

namespace Ludwell.SceneManagerToolkit.Editor
{
    [InitializeOnLoad]
    internal class BuildSettingsObserver
    {
        static BuildSettingsObserver()
        {
            var delayedSceneListChangedCallback = new DelayedEditorUpdateAction(0, SceneListChangedCallback);
            EditorBuildSettings.sceneListChanged -= delayedSceneListChangedCallback.StartOrRefresh;
            EditorBuildSettings.sceneListChanged += delayedSceneListChangedCallback.StartOrRefresh;
        }

        private static void SceneListChangedCallback()
        {
            SceneAssetReferenceDrawer.RepaintInspectorWindows();
            Signals.Dispatch<UISignals.RefreshView>();
        }
    }
}
