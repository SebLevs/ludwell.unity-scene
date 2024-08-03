using Ludwell.EditorUtilities;
using UnityEditor;

namespace Ludwell.Scene.Editor
{
    public static class ResourcesLocator
    {
        private static Settings _toolkitSettings;

        private static SceneAssetDataContainer _sceneAssetDataContainer;
        private static DelayedEditorUpdateAction _delayedSaveSceneAssetDataContainer;

        private static TagContainer _tagContainer;
        private static DelayedEditorUpdateAction _delayedSaveTagContainer;
        
        private static DelayedEditorUpdateAction _delayedSaveSceneAssetsContainerAndTagContainer;

        public static Settings GetSceneDataManagerSettings() 
        {
            if (_toolkitSettings) return _toolkitSettings;
            _toolkitSettings = (Settings)ResourcesSolver.EnsureAssetExistence(typeof(Settings),
                out var _);
            return _toolkitSettings;
        }
        
        public static SceneAssetDataContainer GetSceneAssetDataContainer()
        {
            CacheSceneAssetDataContainer();
            return _sceneAssetDataContainer;
        }

        public static TagContainer GetTagContainer()
        {
            CacheTagContainer();
            return _tagContainer;
        }
        
        public static void SaveSceneAssetDataContainer()
        {
            ExternalAssetChangeProcessor.IsImportCauseInternal = true;
            CacheSceneAssetDataContainer();
            EditorUtility.SetDirty(_sceneAssetDataContainer);
            AssetDatabase.SaveAssetIfDirty(_sceneAssetDataContainer);
            AssetDatabase.Refresh();
        }

        public static void SaveSceneAssetDataContainerDelayed()
        {
            _delayedSaveSceneAssetDataContainer ??= new DelayedEditorUpdateAction(0.5f, SaveSceneAssetDataContainer);
            _delayedSaveSceneAssetDataContainer.StartOrRefresh();
        }

        public static void SaveTagContainer()
        {
            ExternalAssetChangeProcessor.IsImportCauseInternal = true;
            CacheTagContainer();
            EditorUtility.SetDirty(_tagContainer);
            AssetDatabase.SaveAssetIfDirty(_tagContainer);
        }
        
        public static void SaveTagContainerDelayed()
        {
            _delayedSaveTagContainer ??= new DelayedEditorUpdateAction(0.5f, SaveTagContainer);
            _delayedSaveTagContainer.StartOrRefresh();
        }

        private static void SaveSceneAssetContainerAndTagContainer()
        {
            SaveSceneAssetDataContainer();
            SaveTagContainer();
        }
        
        
        public static void SaveSceneAssetContainerAndTagContainerDelayed()
        {
            _delayedSaveSceneAssetsContainerAndTagContainer ??=
                new DelayedEditorUpdateAction(0.5f, SaveSceneAssetContainerAndTagContainer);
            _delayedSaveSceneAssetsContainerAndTagContainer.StartOrRefresh();
        }
        
        private static void CacheSceneAssetDataContainer()
        {
            if (_sceneAssetDataContainer) return;
            _sceneAssetDataContainer =
                (SceneAssetDataContainer)ResourcesSolver.EnsureAssetExistence(typeof(SceneAssetDataContainer), out var existed);
            if (!existed)
            {
                SceneDataGenerator.PopulateQuickLoadElements();
            }
        }

        private static void CacheTagContainer()
        {
            if (_tagContainer) return;
            _tagContainer = (TagContainer)ResourcesSolver.EnsureAssetExistence(typeof(TagContainer), out _);
        }
    }
}