using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Ludwell.Scene.Editor
{
    [InitializeOnLoad]
    public class ResourcesSolver : AssetPostprocessor
    {
        private static readonly string[] EditorPath =
        {
            "Plugins",
            "SceneDataManager",
            "Editor",
            "Resources"
        };

        private static readonly string[] RuntimePath =
        {
            "Plugins",
            "SceneDataManager",
            "Runtime",
            "Resources"
        };

        public static readonly Dictionary<string, (Type, string[])> ScriptableAssets = new()
        {
            { nameof(SceneDataManagerSettings), (typeof(SceneDataManagerSettings), EditorPath) },
            { nameof(CoreScenes), (typeof(CoreScenes), RuntimePath) },
            { nameof(QuickLoadElements), (typeof(QuickLoadElements), EditorPath) },
            { nameof(TagContainer), (typeof(TagContainer), EditorPath) }
        };

        static ResourcesSolver()
        {
            ResourcesFetcher.GetSceneDataManagerSettings();
            ResourcesFetcher.GetQuickLoadElements();
            ResourcesFetcher.GetTagContainer();
            ResourcesFetcher.GetCoreScenes();
        }

        public static ScriptableObject EnsureAssetExistence(Type type)
        {
            if (!ScriptableAssets.TryGetValue(type.Name, out var tuple)) return null;

            var assetPath = Path.Combine(TryCreatePath(tuple.Item2), type.Name + ".asset");
            var existsAtPath = AssetDatabase.LoadAssetAtPath(assetPath, type);
            if (existsAtPath) return (ScriptableObject)existsAtPath;

            var objects = Resources.FindObjectsOfTypeAll(type);
            if (objects.Length > 0) return (ScriptableObject)objects[0];

            var foundObject = AssetDatabase.FindAssets($"t:{type.Name}");
            if (foundObject != null)
            {
                return AssetDatabase.LoadAssetAtPath<ScriptableObject>(AssetDatabase.GUIDToAssetPath(foundObject[0]));
            }

            var scriptable = ScriptableObject.CreateInstance(type);
            AssetDatabase.CreateAsset(scriptable, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return (ScriptableObject)AssetDatabase.LoadAssetAtPath(assetPath, type);
        }

        private static string TryCreatePath(string[] paths)
        {
            var currentPath = "Assets";

            foreach (var folder in paths)
            {
                var newFolderPath = Path.Combine(currentPath, folder);

                if (!AssetDatabase.IsValidFolder(newFolderPath))
                {
                    AssetDatabase.CreateFolder(currentPath, folder);
                }

                currentPath = newFolderPath;
            }

            AssetDatabase.Refresh();

            return currentPath;
        }

        private static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            HandleMovedAssets(movedAssets, movedFromAssetPaths);
        }

        private static void HandleMovedAssets(string[] movedAssets, string[] movedFromAssetPaths)
        {
            if (movedAssets.Length == 0) return;

            for (var index = 0; index < movedAssets.Length; index++)
            {
                var asset = movedAssets[index];
                if (!asset.EndsWith(".asset")) continue;

                var previousFileName = Path.GetFileNameWithoutExtension(movedFromAssetPaths[index]);
                if (!ScriptableAssets.ContainsKey(previousFileName)) continue;

                var newFileName = Path.GetFileNameWithoutExtension(asset);
                if (newFileName != previousFileName)
                {
                    Debug.LogWarning(
                        $"Suspicious action | Resource asset must follow naming convention | {newFileName} > {previousFileName}");
                    AssetDatabase.RenameAsset(asset, previousFileName);
                    continue;
                }

                if (asset.Contains("Resources")) continue;
                Debug.LogWarning(
                    $"Suspicious action | Resource asset must remain in a Resources folder | {previousFileName}");
                AssetDatabase.MoveAsset(asset, movedFromAssetPaths[index]);
            }
        }
    }

    public class ResourcesModificationProcessor : AssetModificationProcessor
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