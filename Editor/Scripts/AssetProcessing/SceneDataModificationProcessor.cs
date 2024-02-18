using System.IO;
using UnityEditor;

namespace Ludwell.Scene.Editor
{
    public class SceneDataModificationProcessor : AssetModificationProcessor
    {
        private static bool _isHandling;

        private static AssetDeleteResult OnWillDeleteAsset(string assetPath, RemoveAssetOptions options)
        {
            return HandleDeletedAsset(assetPath);
        }

        private static AssetDeleteResult HandleDeletedAsset(string assetPath)
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
    }
}