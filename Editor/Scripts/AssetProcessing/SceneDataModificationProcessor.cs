using System.IO;
using UnityEditor;

namespace Ludwell.Scene.Editor
{
    public class SceneDataModificationProcessor : AssetModificationProcessor
    {
        private static bool _isHandling;

        private static AssetDeleteResult OnWillDeleteAsset(string assetPath, RemoveAssetOptions options)
        {
            if (_isHandling) return AssetDeleteResult.DidNotDelete;
            if (!assetPath.EndsWith(".unity") && !assetPath.EndsWith(".asset")) return AssetDeleteResult.DidNotDelete;

            _isHandling = true;

            var otherSpecifier = assetPath.EndsWith(".unity") ? ".asset" : ".unity";

            var assetName = Path.GetFileNameWithoutExtension(assetPath);
            var assetsGuid = AssetDatabase.FindAssets(assetName);

            foreach (var assetGuid in assetsGuid)
            {
                var path = AssetDatabase.GUIDToAssetPath(assetGuid);

                if (!path.EndsWith(otherSpecifier)) continue;

                var foundAssetName = Path.GetFileNameWithoutExtension(path);
                if (foundAssetName != assetName) continue;

                AssetDatabase.DeleteAsset(path);
            }

            LoaderSceneDataHelper.GetLoaderSceneData().RemoveElementWithMainScene(assetName);
            AssetDatabase.DeleteAsset(assetPath);
            _isHandling = false;

            return AssetDeleteResult.DidDelete;
        }

        public static AssetMoveResult OnWillMoveAsset(string oldAssetPath, string newAssetPath)
        {
            if (_isHandling) return AssetMoveResult.DidNotMove;
            if (!oldAssetPath.EndsWith(".unity") && !oldAssetPath.EndsWith(".asset")) return AssetMoveResult.DidNotMove;

            _isHandling = true;

            var oldAssetName = Path.GetFileNameWithoutExtension(oldAssetPath);
            (SceneAsset, SceneData) assets = LoaderSceneDataHelper.GetLoaderSceneData().GetScenesWithName(oldAssetName);

            if (!assets.Item1) return AssetMoveResult.DidNotMove;

            var newName = Path.GetFileNameWithoutExtension(newAssetPath);

            var oldPathSceneData = AssetDatabase.GetAssetPath(assets.Item2);
            AssetDatabase.RenameAsset(oldPathSceneData, newName);
            AssetDatabase.MoveAsset(oldPathSceneData,
                Path.Combine(Path.GetDirectoryName(newAssetPath), newName + ".asset"));


            var oldPathSceneAsset = AssetDatabase.GetAssetPath(assets.Item1);
            AssetDatabase.RenameAsset(oldPathSceneAsset, newName);
            AssetDatabase.MoveAsset(oldPathSceneAsset,
                Path.Combine(Path.GetDirectoryName(newAssetPath), newName + ".unity"));

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            _isHandling = false;

            return AssetMoveResult.DidNotMove;
        }
    }
}