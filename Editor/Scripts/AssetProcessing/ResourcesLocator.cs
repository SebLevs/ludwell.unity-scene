using Ludwell.EditorUtilities;
using UnityEditor;

namespace Ludwell.Scene.Editor
{
    public static class ResourcesLocator
    {
        private static Settings _toolkitSettings;

        private static SceneManagerElements _sceneManagerElements;
        private static DelayedEditorUpdateAction _delayedSaveQuickLoadElements;

        private static TagContainer _tagContainer;
        private static DelayedEditorUpdateAction _delayedSaveTagContainer;
        
        private static DelayedEditorUpdateAction _delayedSaveQuickLoadElementsAndTagContainer;

        public static Settings GetSceneDataManagerSettings() 
        {
            if (_toolkitSettings) return _toolkitSettings;
            _toolkitSettings = (Settings)ResourcesSolver.EnsureAssetExistence(typeof(Settings),
                out var _);
            return _toolkitSettings;
        }

        public static SceneManagerElements GetQuickLoadElements()
        {
            CacheQuickLoadData();
            return _sceneManagerElements;
        }

        public static TagContainer GetTagContainer()
        {
            CacheTagContainer();
            return _tagContainer;
        }

        public static void SaveQuickLoadElements()
        {
            ExternalAssetChangeProcessor.IsImportCauseInternal = true;
            CacheQuickLoadData();
            EditorUtility.SetDirty(_sceneManagerElements);
            AssetDatabase.SaveAssetIfDirty(_sceneManagerElements);
            AssetDatabase.Refresh();
        }

        public static void SaveQuickLoadElementsDelayed()
        {
            _delayedSaveQuickLoadElements ??= new DelayedEditorUpdateAction(0.5f, SaveQuickLoadElements);
            _delayedSaveQuickLoadElements.StartOrRefresh();
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

        private static void SaveQuickLoadElementsAndTagContainer()
        {
            SaveQuickLoadElements();
            SaveTagContainer();
        }
        
        
        public static void SaveQuickLoadElementsAndTagContainerDelayed()
        {
            _delayedSaveQuickLoadElementsAndTagContainer ??=
                new DelayedEditorUpdateAction(0.5f, SaveQuickLoadElementsAndTagContainer);
            _delayedSaveQuickLoadElementsAndTagContainer.StartOrRefresh();
        }

        private static void CacheQuickLoadData()
        {
            if (_sceneManagerElements) return;
            _sceneManagerElements =
                (SceneManagerElements)ResourcesSolver.EnsureAssetExistence(typeof(SceneManagerElements), out var existed);
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