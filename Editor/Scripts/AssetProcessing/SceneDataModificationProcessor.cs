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

            var directoryName = Path.GetDirectoryName(assetPath);
            var assetName = Path.GetFileNameWithoutExtension(assetPath);
            var sceneData = AssetDatabase.LoadAssetAtPath<SceneData>(Path.Combine(directoryName, assetName + ".asset"));
            if (sceneData == null)
            {
                _isHandling = false;
                return AssetDeleteResult.DidNotDelete;
            }
            ResourcesLocator.GetQuickLoadElements().Remove(sceneData);
            ResourcesLocator.SaveQuickLoadElementsAndTagContainerDelayed();

            var otherSpecifier = assetPath.EndsWith(".unity") ? ".asset" : ".unity";
            AssetDatabase.DeleteAsset(Path.Combine(directoryName, assetName + otherSpecifier));
            AssetDatabase.DeleteAsset(assetPath);

            _isHandling = false;

            return AssetDeleteResult.DidDelete;
        }
    }
}