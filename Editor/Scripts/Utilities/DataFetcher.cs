using System.IO;
using UnityEditor;
using UnityEngine;

namespace Ludwell.Scene.Editor
{
    public static class DataFetcher
    {
        private static CoreScenes _coreScenes;
        private static QuickLoadElements _quickLoadElements;
        private static TagContainer _tagContainer;
        private static DelayedEditorUpdateAction _delayedEditorUpdateAction;

        public static CoreScenes GetCoreScenes()
        {
            CacheCoreScenes();
            return _coreScenes;
        }

        public static QuickLoadElements GetQuickLoadElements()
        {
            CacheLoaderSceneData();
            return _quickLoadElements;
        }

        public static TagContainer GetTagContainer()
        {
            CacheTagContainer();
            return _tagContainer;
        }

        public static void SaveEveryScriptable()
        {
            CacheCoreScenes();
            EditorUtility.SetDirty(_coreScenes);
            AssetDatabase.SaveAssetIfDirty(_coreScenes);
            
            CacheLoaderSceneData();
            EditorUtility.SetDirty(_quickLoadElements);
            AssetDatabase.SaveAssetIfDirty(_quickLoadElements);

            CacheTagContainer();
            EditorUtility.SetDirty(_tagContainer);
            AssetDatabase.SaveAssetIfDirty(_tagContainer);
        }

        public static void SaveEveryScriptableDelayed()
        {
            _delayedEditorUpdateAction ??= new DelayedEditorUpdateAction(0.5f, SaveEveryScriptable);
            _delayedEditorUpdateAction.StartOrRefresh();
        }

        private static void CacheCoreScenes()
        {
            if (!_coreScenes)
            {
                _coreScenes = Resources.Load<CoreScenes>(Path.Combine("Scriptables", nameof(CoreScenes)));
            }
        }

        private static void CacheLoaderSceneData()
        {
            if (!_quickLoadElements)
            {
                _quickLoadElements = Resources.Load<QuickLoadElements>(Path.Combine("Scriptables", nameof(QuickLoadElements)));
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