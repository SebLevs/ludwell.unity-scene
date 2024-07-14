using UnityEditor;

namespace Ludwell.Scene.Editor
{
    public static class ResourcesLocator
    {
        private static SceneDataManagerSettings _settings;

        private static QuickLoadElements _quickLoadElements;
        private static DelayedEditorUpdateAction _delayedSaveQuickLoadElements;

        private static TagContainer _tagContainer;
        private static DelayedEditorUpdateAction _delayedSaveTagContainer;
        
        private static DelayedEditorUpdateAction _delayedSaveQuickLoadElementsAndTagContainer;
        
        private static JoinTable _joinTable;

        public static SceneDataManagerSettings GetSceneDataManagerSettings()
        {
            if (_settings) return _settings;
            _settings = (SceneDataManagerSettings)ResourcesSolver.EnsureAssetExistence(typeof(SceneDataManagerSettings),
                out var _);
            return _settings;
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
        
        public static JoinTable GetJoinTable()
        {
            CacheJoinTable();
            return _joinTable;
        }

        public static void SaveQuickLoadElements()
        {
            ExternalAssetChangeProcessor.IsImportCauseInternal = true;
            CacheQuickLoadData();
            EditorUtility.SetDirty(_quickLoadElements);
            AssetDatabase.SaveAssetIfDirty(_quickLoadElements);
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
        
        public static void SaveJoinTable()
        {
            CacheJoinTable();
            EditorUtility.SetDirty(_joinTable);
            AssetDatabase.SaveAssetIfDirty(_joinTable);
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
            if (_quickLoadElements) return;
            _quickLoadElements =
                (QuickLoadElements)ResourcesSolver.EnsureAssetExistence(typeof(QuickLoadElements), out var existed);
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
        
        private static void CacheJoinTable()
        {
            if (_joinTable) return;
            _joinTable = (JoinTable)ResourcesSolver.EnsureAssetExistence(typeof(JoinTable), out _);
        }
    }
}