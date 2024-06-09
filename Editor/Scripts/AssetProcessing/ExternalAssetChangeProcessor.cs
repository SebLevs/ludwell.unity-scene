using UnityEditor;
using UnityEngine;

namespace Ludwell.Scene.Editor
{
    // todo: this class is trash, find better solution to handle scriptable object changes outside of unity
    [InitializeOnLoad]
    public class ExternalAssetChangeProcessor : AssetPostprocessor
    {
        private static bool _isHandlingBookKeeping;

        static ExternalAssetChangeProcessor()
        {
            SolveTags();
            SolveQuickLoadElements();
        }

        private static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            if (importedAssets.Length <= 0) return;
            if (_isHandlingBookKeeping) return;

            _isHandlingBookKeeping = true;

            foreach (var asset in importedAssets)
            {
                if (!asset.EndsWith(".asset")) continue;

                var importedAsset = AssetDatabase.LoadAssetAtPath<ScriptableObject>(asset);
                if (importedAsset is QuickLoadElements)
                {
                    SolveQuickLoadElements();
                }
                else if (importedAsset is not TagContainer)
                {
                    SolveTags();
                }
            }

            _isHandlingBookKeeping = false;
        }

        public static void SolveQuickLoadElements()
        {
            var quickLoadElements = DataFetcher.GetQuickLoadElements().Elements;
            var tags = DataFetcher.GetTagContainer().Tags;

            Debug.LogError("QuickLoadElements");

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

            Signals.Dispatch<UISignals.RefreshQuickLoadListView>();
        }

        public static void SolveTags()
        {
            var quickLoadElements = DataFetcher.GetQuickLoadElements().Elements;
            var tags = DataFetcher.GetTagContainer().Tags;

            Debug.LogError("TagContainer");

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

            Signals.Dispatch<UISignals.RefreshQuickLoadListView>();
        }
    }
}