using System.Collections.Generic;
using System.IO;
using Ludwell.Architecture;
using UnityEditor;
using UnityEngine;

namespace Ludwell.Scene.Editor
{
    public class SceneAssetPostProcessor : AssetPostprocessor
    {
        private static bool _isHandlingMoved;
        private static bool _isHandlingImported;
        private static bool _isHandlingDeleted;

        private static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            if (!SolveDeleted(deletedAssets) &&
                !SolveMoved(movedAssets) &&
                !SolveImported(importedAssets))
                return;

            Signals.Dispatch<UISignals.RefreshView>();
            ResourcesLocator.SaveSceneAssetDataBindersDelayed();
        }

        private static bool SolveDeleted(IReadOnlyList<string> deleted)
        {
            if (deleted.Count <= 0) return false;
            if (_isHandlingDeleted) return false;

            _isHandlingDeleted = true;
            var hasSolved = false;

            for (var index = 0; index < deleted.Count; index++)
            {
                if (!deleted[index].EndsWith(".unity")) continue;

                hasSolved = true;
                ResourcesLocator.GetSceneAssetDataBinders().Remove(AssetDatabase.AssetPathToGUID(deleted[index]));
            }

            _isHandlingDeleted = false;
            return hasSolved;
        }

        private static bool SolveMoved(IReadOnlyList<string> movedAssets)
        {
            if (movedAssets.Count <= 0) return false;
            if (_isHandlingMoved) return false;

            _isHandlingMoved = true;
            var hasSolved = false;

            for (var index = 0; index < movedAssets.Count; index++)
            {
                if (!movedAssets[index].EndsWith(".unity")) continue;

                hasSolved = true;

                var sceneAssetDataContainer = ResourcesLocator.GetSceneAssetDataBinders();

                var guid = AssetDatabase.AssetPathToGUID(movedAssets[index]);
                var binder = sceneAssetDataContainer.GetBinderFromId(guid);
                binder.Data.Name = Path.GetFileNameWithoutExtension(movedAssets[index]);
                binder.Data.Path = movedAssets[index];
            }

            if (hasSolved) ResourcesLocator.GetSceneAssetDataBinders().Elements.Sort();
            _isHandlingMoved = false;
            return hasSolved;
        }

        private static bool SolveImported(IReadOnlyCollection<string> importedAssets)
        {
            if (importedAssets.Count <= 0) return false;
            if (_isHandlingImported) return false;

            _isHandlingImported = true;
            var hasSolved = false;

            foreach (var path in importedAssets)
            {
                if (!path.EndsWith(".unity")) continue;

                hasSolved = true;
                var guid = AssetDatabase.AssetPathToGUID(path);
                Debug.LogError("todo?: imported from scene save? prevent addition if data already exists in binders? duplication bug?");
                DataSolver.AddSceneAssetDataBinderFromGuid(guid);
            }

            _isHandlingImported = false;

            return hasSolved;
        }
    }
}