using Ludwell.Architecture;
using UnityEditor;

namespace Ludwell.Scene.Editor
{
    public class SceneAssetModificationProcessor : AssetModificationProcessor
    {
        private static bool _isHandling;

        private static AssetDeleteResult OnWillDeleteAsset(string assetPath, RemoveAssetOptions options)
        {
            if (_isHandling) return AssetDeleteResult.DidNotDelete;
            if (!assetPath.EndsWith(".unity")) return AssetDeleteResult.DidNotDelete;

            _isHandling = true;

            ResourcesLocator.GetSceneAssetDataBinders().Remove(AssetDatabase.AssetPathToGUID(assetPath));
            ResourcesLocator.SaveSceneAssetDataBindersAndTagsDelayed();
            Signals.Dispatch<UISignals.RefreshView>();

            AssetDatabase.DeleteAsset(assetPath);

            _isHandling = false;

            return AssetDeleteResult.DidDelete;
        }
    }
}