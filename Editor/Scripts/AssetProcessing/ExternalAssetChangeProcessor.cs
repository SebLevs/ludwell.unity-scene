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
                SolveQuickLoadElements();
                break;
            }
        }

        public static void SolveQuickLoadElements()
        {
            var quickLoadElements = ResourcesLocator.GetQuickLoadElements().Elements;
            var tagContainer = ResourcesLocator.GetTagContainer();

            bool shouldSave = false;

            foreach (var element in quickLoadElements)
            {
                for (var index = element.Tags.Count - 1; index >= 0; index--)
                {
                    var tag = element.Tags[index];
                    if (tagContainer.Tags.Contains(tag)) continue;
                    element.Tags.Remove(tag);
                    shouldSave = true;
                }
            }

            Signals.Dispatch<UISignals.RefreshView>();

            if (!shouldSave) return;
            ResourcesLocator.SaveQuickLoadElements();
        }
    }
}