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
                if (importedAsset is QuickLoadElements)
                {
                    SolveQuickLoadElements();
                }
                else if (importedAsset is TagContainer)
                {
                    SolveTags();
                }
            }
        }

        public static void SolveQuickLoadElements()
        {
            var quickLoadElements = DataFetcher.GetQuickLoadElements().Elements;
            var tags = DataFetcher.GetTagContainer().Tags;

            if (quickLoadElements.Count == 0)
            {
                foreach (var tag in tags)
                {
                    tag.Clear();
                }
            }

            foreach (var element in quickLoadElements)
            {
                for (var index = element.Tags.Count - 1; index >= 0; index--)
                {
                    var tag = element.Tags[index];
                    var tagWithSubscribers = (TagWithSubscribers)tag;
                    if (tags.Contains(tagWithSubscribers)) continue;
                    tagWithSubscribers.RemoveFromAllSubscribers();
                }
            }

            Signals.Dispatch<UISignals.RefreshQuickLoadListView>(); // todo: refresh current view instead of quick load. ie.tag view
            DataFetcher.SaveQuickLoadElementsAndTagContainerDelayed();
        }

        public static void SolveTags()
        {
            var quickLoadElements = DataFetcher.GetQuickLoadElements().Elements;
            var tags = DataFetcher.GetTagContainer().Tags;

            if (tags.Count == 0)
            {
                foreach (var element in quickLoadElements)
                {
                    element.Clear();
                }
            }

            foreach (var tag in tags)
            {
                for (var index = tag.Subscribers.Count - 1; index >= 0; index--)
                {
                    var tagSubscriber = tag.Subscribers[index];
                    var quickLoadElementData = (QuickLoadElementData)tagSubscriber;
                    if (quickLoadElements.Contains(quickLoadElementData)) continue;
                    quickLoadElementData.RemoveFromAllTags();
                }
            }

            Signals.Dispatch<UISignals.RefreshQuickLoadListView>(); // todo: refresh current view instead of quick load. ie.tag view
            DataFetcher.SaveQuickLoadElementsAndTagContainerDelayed();
        }
    }
}