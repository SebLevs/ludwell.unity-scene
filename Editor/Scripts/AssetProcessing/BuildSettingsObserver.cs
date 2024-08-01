using Ludwell.Architecture;
using UnityEditor;

namespace Ludwell.Scene.Editor
{
    [InitializeOnLoad]
    public class BuildSettingsObserver
    {
        private static int _buildSettingsCount;

        static BuildSettingsObserver()
        {
            _buildSettingsCount = EditorBuildSettings.scenes.Length;
            EditorApplication.update += Update;
        }

        private static void Update()
        {
            if (_buildSettingsCount == EditorBuildSettings.scenes.Length) return;
            _buildSettingsCount = EditorBuildSettings.scenes.Length;

            Signals.Dispatch<UISignals.RefreshView>();
            SceneAssetReferenceDrawer.OnBuildSettingsChangedSolveHelpBoxes();
        }
    }
}