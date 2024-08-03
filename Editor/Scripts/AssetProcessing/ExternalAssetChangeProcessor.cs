using Ludwell.Architecture;
using UnityEditor;
using UnityEngine;

namespace Ludwell.Scene.Editor
{
    // todo: this class is trash, find better solution to handle external modifications such as git discard
    public class ExternalAssetChangeProcessor : AssetPostprocessor
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

            if (importedAssets.Length <= 0) return;

            foreach (var asset in importedAssets)
            {
                if (!asset.EndsWith(".asset")) continue;

                var importedAsset = AssetDatabase.LoadAssetAtPath<ScriptableObject>(asset);
                if (importedAsset is not TagContainer) continue;
                SolveSceneAssetListing();
                break;
            }
        }

        public static void SolveSceneAssetListing()
        {
            var dataBinders = SceneAssetDataContainer.Instance.DataBinders;
            var tagContainer = ResourcesLocator.GetTagContainer();

            var shouldSave = false;

            foreach (var dataBinder in dataBinders)
            {
                for (var index = dataBinder.Tags.Count - 1; index >= 0; index--)
                {
                    var tag = dataBinder.Tags[index];
                    if (tagContainer.Tags.Contains(tag)) continue;
                    dataBinder.Tags.Remove(tag);
                    shouldSave = true;
                }
            }

            Signals.Dispatch<UISignals.RefreshView>();

            if (!shouldSave) return;
            ResourcesLocator.SaveSceneAssetDataContainer();
        }
    }
}