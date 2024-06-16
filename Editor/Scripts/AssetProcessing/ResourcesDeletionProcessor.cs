using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Ludwell.Scene.Editor
{
    public class ResourcesDeletionProcessor : AssetModificationProcessor
    {
        private static AssetDeleteResult OnWillDeleteAsset(string assetPath, RemoveAssetOptions options)
        {
            if (assetPath.EndsWith(".asset"))
            {
                var fileName = Path.GetFileNameWithoutExtension(assetPath);
                if (!ResourcesSolver.ScriptableAssets.ContainsKey(fileName))
                {
                    return AssetDeleteResult.DidNotDelete;
                }

                Debug.LogWarning($"Suspicious action | Asset is required | {fileName}");
                return AssetDeleteResult.DidDelete;
            }

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
                Debug.LogWarning($"Suspicious action | Asset is required | {fileName}");
                return AssetDeleteResult.DidDelete;
            }

            return AssetDeleteResult.DidNotDelete;
        }
    }
}
