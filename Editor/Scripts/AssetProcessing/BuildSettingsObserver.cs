using Ludwell.Architecture;
using UnityEditor;

namespace Ludwell.Scene.Editor
{
    [InitializeOnLoad]
    public class BuildSettingsObserver
    {
        static BuildSettingsObserver()
        {
            EditorBuildSettings.sceneListChanged -= SceneListChangedCallback;
            EditorBuildSettings.sceneListChanged += SceneListChangedCallback;
        }

        private static void SceneListChangedCallback()
        {
            SceneAssetReferenceController.SolveAllBuildSettingsButtonVisibleState();
            Signals.Dispatch<UISignals.RefreshView>();
        }
    }
}