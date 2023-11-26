using UnityEditor;
using UnityEngine;

namespace Ludwell.Scene.Editor
{
    public static class LoaderSceneDataHelper
    {
        private static LoaderSceneData _loaderSceneData;
        private static DelayedEditorUpdateAction _delayedEditorUpdateAction;

        public static void SaveChange()
        {
            if (!_loaderSceneData)
            {
                _loaderSceneData = Resources.Load<LoaderSceneData>("Scriptables/" + nameof(LoaderSceneData));
            }

            EditorUtility.SetDirty(_loaderSceneData);
            AssetDatabase.SaveAssetIfDirty(_loaderSceneData);
        }

        public static void SaveChangeDelayed()
        {
            _delayedEditorUpdateAction ??= new DelayedEditorUpdateAction(0.5f, SaveChange);
            _delayedEditorUpdateAction.StartOrRefresh();
        }
    }
}
