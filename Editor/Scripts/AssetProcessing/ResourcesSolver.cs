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

        private static readonly Dictionary<string, (Type, string[])> ScriptableAssets = new()
        {
            { nameof(SceneDataManagerSettings), (typeof(SceneDataManagerSettings), EditorPath) },
            { nameof(CoreScenes), (typeof(CoreScenes), RuntimePath) },
            { nameof(QuickLoadElements), (typeof(QuickLoadElements), EditorPath) },
            { nameof(TagContainer), (typeof(TagContainer), EditorPath) }
        };

        private static bool _isHandlingMove;

        static ResourcesSolver()
        {
            EnsureAssetExistence(typeof(SceneDataManagerSettings));
            EnsureAssetExistence(typeof(QuickLoadElements));
            EnsureAssetExistence(typeof(TagContainer));
            EnsureAssetExistence(typeof(CoreScenes));
        }

        public static ScriptableObject EnsureAssetExistence(Type type)
        {
            if (!ScriptableAssets.TryGetValue(type.Name, out var tuple)) return null;
            var assetPath = Path.Combine(TryCreatePath(tuple.Item2), type.Name + ".asset");

            var existsAtPath = AssetDatabase.LoadAssetAtPath(assetPath, type);

            if (existsAtPath) return (ScriptableObject)existsAtPath;

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
            if (_isHandlingMove) return;
            if (HandleDeletedAssets(deletedAssets)) return;
            HandleMovedAssets(movedAssets, movedFromAssetPaths);
        }

        private static bool HandleDeletedAssets(string[] deletedAssets)
        {
            if (deletedAssets.Length == 0) return false;

            if (_isHandlingMove) return false;
            _isHandlingMove = true;

            foreach (var deletedAsset in deletedAssets)
            {
                if (!deletedAsset.EndsWith(".asset")) continue;

                var fileName = Path.GetFileNameWithoutExtension(deletedAsset);
                if (!ScriptableAssets.ContainsKey(fileName)) return false;
                Debug.LogWarning($"Suspicious action | Asset is required | {fileName}");
                EnsureAssetExistence(ScriptableAssets[fileName].Item1);
            }

            _isHandlingMove = false;
            return true;
        }

        private static bool HandleMovedAssets(string[] movedAssets, string[] movedFromAssetPaths)
        {
            if (movedAssets.Length == 0) return false;

            if (_isHandlingMove) return false;
            _isHandlingMove = true;

            for (var index = 0; index < movedAssets.Length; index++)
            {
                var asset = movedAssets[index];
                if (!asset.EndsWith(".asset")) continue;

                var fileName = Path.GetFileNameWithoutExtension(movedFromAssetPaths[index]);
                if (!ScriptableAssets.ContainsKey(fileName)) continue;

                Debug.LogWarning($"Suspicious action | Asset must remain at its initial path | {fileName}");
                AssetDatabase.MoveAsset(asset, movedFromAssetPaths[index]);
                AssetDatabase.DeleteAsset(asset);
            }

            return _isHandlingMove = false;
        }
    }
}