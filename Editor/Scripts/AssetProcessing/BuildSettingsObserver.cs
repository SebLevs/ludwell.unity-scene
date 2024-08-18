using Ludwell.Architecture;
using UnityEditor;
using UnityEngine;

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
            Debug.LogError("todo: make it so it is only called once when called through a chunk");
            SceneAssetReferenceController.SolveAllBuildSettingsButtonVisibleState();
            Signals.Dispatch<UISignals.RefreshView>();
        }
    }
}