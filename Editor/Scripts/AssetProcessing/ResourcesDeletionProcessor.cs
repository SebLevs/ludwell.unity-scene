using System.IO;
using UnityEditor;
using UnityEngine;

namespace Ludwell.Scene.Editor
{
    public class ResourcesDeletionProcessor : AssetModificationProcessor
    {
        private static AssetDeleteResult OnWillDeleteAsset(string assetPath, RemoveAssetOptions options)
        {
            if (EvaluateAsset(assetPath, out var onWillDeleteAsset)) return onWillDeleteAsset;

            return EvaluateFolder(assetPath);
        }

        private static bool EvaluateAsset(string assetPath, out AssetDeleteResult onWillDeleteAsset)
        {
            if (assetPath.EndsWith(".asset"))
            {
                var fileName = Path.GetFileNameWithoutExtension(assetPath);
                if (!ResourcesSolver.ScriptableAssets.ContainsKey(fileName))
                {
                    {
                        onWillDeleteAsset = AssetDeleteResult.DidNotDelete;
                        return true;
                    }
                }

                Debug.LogWarning($"Suspicious action | Asset is required | {fileName}");
                {
                    onWillDeleteAsset = AssetDeleteResult.DidDelete;
                    return true;
                }
            }

            onWillDeleteAsset = AssetDeleteResult.DidNotDelete;
            return false;
        }

        private static AssetDeleteResult EvaluateFolder(string assetPath)
        {
            if (!Directory.Exists(assetPath)) return AssetDeleteResult.DidNotDelete;

            var guids = AssetDatabase.FindAssets($"t:{nameof(ScriptableObject)}", new[] { assetPath });

            if (guids.Length == 0)
            {
                return AssetDeleteResult.DidNotDelete;
            }

            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var fileName = Path.GetFileNameWithoutExtension(path);
                if (!ResourcesSolver.ScriptableAssets.ContainsKey(fileName)) continue;
                Debug.LogWarning($"Suspicious action | Path contains required assets | {assetPath}");
                return AssetDeleteResult.DidDelete;
            }

            return AssetDeleteResult.DidNotDelete;
        }
    }
}