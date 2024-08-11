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
            "SceneManagerToolkit",
            "Editor",
            "Resources"
        };

        private static readonly string[] RuntimePath =
        {
            "Plugins",
            "SceneManagerToolkit",
            "Runtime",
            "Resources"
        };

        /// <summary> Add resources here </summary>
        public static readonly Dictionary<string, (Type, string[])> ScriptableAssets = new()
        {
            { nameof(Settings), (typeof(Settings), EditorPath) },
            { nameof(Tags), (typeof(Tags), RuntimePath) },
            { nameof(SceneAssetDataBinders), (typeof(SceneAssetDataBinders), RuntimePath)}
        };

        static ResourcesSolver()
        {
            ResourcesLocator.GetSceneDataManagerSettings();
            ResourcesLocator.GetTags();
            ResourcesLocator.GetSceneAssetDataBinders();
        }

        public static ScriptableObject EnsureAssetExistence(Type type, out bool existed)
        {
            if (!ScriptableAssets.TryGetValue(type.Name, out var tuple))
            {
                existed = false;
                return null;
            }

            var assetPath = Path.Combine(TryCreatePath(tuple.Item2), type.Name + ".asset");
            var existsAtPath = AssetDatabase.LoadAssetAtPath(assetPath, type);
            if (existsAtPath)
            {
                existed = true;
                return (ScriptableObject)existsAtPath;
            }

            var objects = Resources.FindObjectsOfTypeAll(type);
            if (objects.Length > 0)
            {
                existed = true;
                return (ScriptableObject)objects[0];
            }

            var foundObject = AssetDatabase.FindAssets($"t:{type.Name}");
            if (foundObject is { Length: > 0 })
            {
                existed = true;
                return AssetDatabase.LoadAssetAtPath<ScriptableObject>(AssetDatabase.GUIDToAssetPath(foundObject[0]));
            }

            var scriptable = ScriptableObject.CreateInstance(type);
            AssetDatabase.CreateAsset(scriptable, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            existed = false;
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
}