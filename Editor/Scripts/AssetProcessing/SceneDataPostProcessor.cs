using System.Collections.Generic;
using System.IO;
using Ludwell.Architecture;
using UnityEditor;

namespace Ludwell.Scene.Editor
{
    public class SceneDataPostProcessor : AssetPostprocessor
    {
        private static bool _isHandlingMove;
        private static bool _isHandlingImport;

        private static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            if (!TryHandleMove(movedAssets, movedFromAssetPaths) && !TryHandleImport(importedAssets)) return;

            ResourcesLocator.GetSceneAssetDataBinders().Elements.Sort();
            Signals.Dispatch<UISignals.RefreshView>();
            ResourcesLocator.SaveSceneAssetDataBinders();
        }

        private static bool TryHandleMove(IReadOnlyList<string> movedAssets, IReadOnlyList<string> movedFromAssetPaths)
        {
            if (movedAssets.Count <= 0) return false;
            if (_isHandlingMove) return false;

            _isHandlingMove = true;
            var shouldSave = false;

            for (var index = 0; index < movedAssets.Count; index++)
            {
                if (!movedAssets[index].EndsWith(".unity")) continue;

                shouldSave = true;

                var sceneAssetDataContainer = ResourcesLocator.GetSceneAssetDataBinders();

                var binder = sceneAssetDataContainer.GetBinderFromPath(movedFromAssetPaths[index]);
                binder.ID = AssetDatabase.AssetPathToGUID(movedAssets[index]);
                binder.Data.Name = Path.GetFileNameWithoutExtension(movedAssets[index]);
                binder.Data.Path = movedAssets[index];
            }

            _isHandlingMove = false;
            return shouldSave;
        }

        private static bool TryHandleImport(IReadOnlyCollection<string> importedAssets)
        {
            if (importedAssets.Count <= 0) return false;
            if (_isHandlingImport) return false;

            _isHandlingImport = true;
            var shouldSave = false;

            foreach (var path in importedAssets)
            {
                if (!path.EndsWith(".unity")) continue;
                shouldSave = true;
                var guid = AssetDatabase.AssetPathToGUID(path);
                if (ResourcesLocator.GetSceneAssetDataBinders().ContainsWithId(guid)) continue;

                SceneDataGenerator.AddFromGuid(guid);
            }

            _isHandlingImport = false;

            return shouldSave;
        }
    }
}