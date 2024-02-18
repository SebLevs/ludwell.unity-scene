using System.IO;
using UnityEditor;
using UnityEngine;

namespace Ludwell.Scene.Editor
{
    public static class LoaderSceneDataHelper
    {
        private static LoaderSceneData _loaderSceneData;
        private static TagContainer _tagContainer;
        private static DelayedEditorUpdateAction _delayedEditorUpdateAction;

        public static LoaderSceneData GetLoaderSceneData()
        {
            CacheLoaderSceneData();

            return _loaderSceneData;
        }

        public static void SaveChange()
        {
            if (!_loaderSceneData)
            {
                _loaderSceneData = Resources.Load<LoaderSceneData>(Path.Combine("Scriptables", nameof(LoaderSceneData)));
            }

            EditorUtility.SetDirty(_loaderSceneData);
            AssetDatabase.SaveAssetIfDirty(_loaderSceneData);

            if (!_tagContainer)
            {
                _tagContainer = Resources.Load<TagContainer>(Path.Combine("Scriptables", nameof(TagContainer)));
            }

            EditorUtility.SetDirty(_tagContainer);
            AssetDatabase.SaveAssetIfDirty(_tagContainer);
        }

        public static void SaveChangeDelayed()
        {
            _delayedEditorUpdateAction ??= new DelayedEditorUpdateAction(0.5f, SaveChange);
            _delayedEditorUpdateAction.StartOrRefresh();
        }

        private static void CacheLoaderSceneData()
        {
            if (!_loaderSceneData)
            {
                _loaderSceneData = Resources.Load<LoaderSceneData>(Path.Combine("Scriptables",  nameof(LoaderSceneData)));
            }
        }
    }
}