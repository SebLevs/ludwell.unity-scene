using UnityEditor;
using UnityEngine;

namespace Ludwell.Scene.Editor
{
    public class ContainersPostProcessor : AssetPostprocessor
    {
        public static bool IsImportCauseInternal;

        private static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            if (IsImportCauseInternal)
            {
                IsImportCauseInternal = false;
                return;
            }

            foreach (var asset in importedAssets)
            {
                if (!asset.EndsWith(".asset")) continue;

                var importedAsset = AssetDatabase.LoadAssetAtPath<ScriptableObject>(asset);
                if (importedAsset is Tags)
                {
                    DataSolver.SolveTagBindings();
                    continue;
                }

                if (importedAsset is not SceneAssetDataBinders) continue;
                DataSolver.SolveSceneAssetDataBinders();
            }
        }
    }
}