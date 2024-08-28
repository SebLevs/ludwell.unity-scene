using Ludwell.EditorUtilities;
using UnityEditor;

namespace Ludwell.SceneManagerToolkit.Editor
{
    public static class ResourcesLocator
    {
        private static Settings _toolkitSettings;

        private static SceneAssetDataBinders _sceneAssetDataBinders;
        private static DelayedEditorUpdateAction _delayedSaveSceneAssetDataBinders;

        private static Tags _tags;

        private static DelayedEditorUpdateAction _delayedSaveSceneAssetDataBindersAndTags;

        public static Settings GetSceneDataManagerSettings()
        {
            if (_toolkitSettings) return _toolkitSettings;
            _toolkitSettings = (Settings)ResourcesSolver.EnsureAssetExistence(typeof(Settings), out _);
            return _toolkitSettings;
        }

        public static SceneAssetDataBinders GetSceneAssetDataBinders()
        {
            CacheSceneAssetDataBinders();
            return _sceneAssetDataBinders;
        }

        public static Tags GetTags()
        {
            CacheTags();
            return _tags;
        }

        public static void SaveSceneAssetDataBinders()
        {
            ContainersPostProcessor.IsImportCauseInternal = true;
            CacheSceneAssetDataBinders();
            EditorUtility.SetDirty(_sceneAssetDataBinders);
            AssetDatabase.SaveAssetIfDirty(_sceneAssetDataBinders);
            AssetDatabase.Refresh();
        }

        public static void SaveSceneAssetDataBindersDelayed()
        {
            _delayedSaveSceneAssetDataBinders ??= new DelayedEditorUpdateAction(0.5f, SaveSceneAssetDataBinders);
            _delayedSaveSceneAssetDataBinders.StartOrRefresh();
        }

        public static void SaveTags()
        {
            ContainersPostProcessor.IsImportCauseInternal = true;
            CacheTags();
            EditorUtility.SetDirty(_tags);
            AssetDatabase.SaveAssetIfDirty(_tags);
        }

        private static void SaveSceneAssetDataBindersAndTags()
        {
            ContainersPostProcessor.IsImportCauseInternal = true;
            SaveSceneAssetDataBinders();
            SaveTags();
        }

        public static void SaveSceneAssetDataBindersAndTagsDelayed()
        {
            _delayedSaveSceneAssetDataBindersAndTags ??=
                new DelayedEditorUpdateAction(0.5f, SaveSceneAssetDataBindersAndTags);
            _delayedSaveSceneAssetDataBindersAndTags.StartOrRefresh();
        }

        private static void CacheSceneAssetDataBinders()
        {
            if (_sceneAssetDataBinders) return;
            _sceneAssetDataBinders =
                (SceneAssetDataBinders)ResourcesSolver.EnsureAssetExistence(typeof(SceneAssetDataBinders),
                    out var existed);
            if (!existed)
            {
                DataSolver.PopulateSceneAssetDataBinders();
            }
        }

        private static void CacheTags()
        {
            if (_tags) return;
            _tags = (Tags)ResourcesSolver.EnsureAssetExistence(typeof(Tags), out _);
        }
    }
}