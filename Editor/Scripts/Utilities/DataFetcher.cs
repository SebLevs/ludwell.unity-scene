using System.IO;
using UnityEditor;
using UnityEngine;

namespace Ludwell.Scene.Editor
{
    public static class DataFetcher
    {
        private static CoreScenes _coreScenes;

        private static QuickLoadElements _quickLoadElements;
        private static DelayedEditorUpdateAction _delayedSaveQuickLoadElements;

        private static TagContainer _tagContainer;
        private static DelayedEditorUpdateAction _delayedSaveTagContainer;

        private static DelayedEditorUpdateAction _delayedSaveQuickLoadElementsAndTagContainer;

        public static CoreScenes GetCoreScenes()
        {
            CacheCoreScenes();
            return _coreScenes;
        }

        public static QuickLoadElements GetQuickLoadElements()
        {
            CacheQuickLoadData();
            return _quickLoadElements;
        }

        public static TagContainer GetTagContainer()
        {
            CacheTagContainer();
            return _tagContainer;
        }

        public static void SaveCoreScenes()
        {
            CacheCoreScenes();
            EditorUtility.SetDirty(_coreScenes);
            AssetDatabase.SaveAssetIfDirty(_coreScenes);
        }

        public static void SaveTagContainerDelayed()
        {
            _delayedSaveTagContainer ??= new DelayedEditorUpdateAction(0.5f, SaveCoreScenes);
            _delayedSaveTagContainer.StartOrRefresh();
        }

        public static void SaveQuickLoadElements()
        {
            CacheQuickLoadData();
            EditorUtility.SetDirty(_quickLoadElements);
            AssetDatabase.SaveAssetIfDirty(_quickLoadElements);
        }

        public static void SaveQuickLoadElementsDelayed()
        {
            _delayedSaveQuickLoadElements ??= new DelayedEditorUpdateAction(0.5f, SaveQuickLoadElements);
            _delayedSaveQuickLoadElements.StartOrRefresh();
        }

        public static void SaveTagContainer()
        {
            CacheTagContainer();
            EditorUtility.SetDirty(_tagContainer);
            AssetDatabase.SaveAssetIfDirty(_tagContainer);
        }

        public static void SaveQuickLoadElementsAndTagContainerDelayed()
        {
            _delayedSaveQuickLoadElementsAndTagContainer ??=
                new DelayedEditorUpdateAction(0.5f, SaveQuickLoadElementsAndTagContainer);
            _delayedSaveQuickLoadElementsAndTagContainer.StartOrRefresh();
        }

        private static void SaveQuickLoadElementsAndTagContainer()
        {
            SaveQuickLoadElements();
            SaveTagContainer();
        }

        private static void CacheCoreScenes()
        {
            if (!_coreScenes)
            {
                _coreScenes = Resources.Load<CoreScenes>(Path.Combine("Scriptables", nameof(CoreScenes)));
            }
        }

        private static void CacheQuickLoadData()
        {
            if (!_quickLoadElements)
            {
                _quickLoadElements =
                    Resources.Load<QuickLoadElements>(Path.Combine("Scriptables", nameof(QuickLoadElements)));
            }
        }

        private static void CacheTagContainer()
        {
            if (!_tagContainer)
            {
                _tagContainer = Resources.Load<TagContainer>(Path.Combine("Scriptables", nameof(TagContainer)));
            }
        }
    }
}