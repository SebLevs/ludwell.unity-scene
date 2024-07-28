using System;
using System.Collections.Generic;
using System.IO;
using Ludwell.Architecture;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Ludwell.Scene.Editor
{
    [InitializeOnLoad]
    public static class SceneDataGenerator
    {
        static SceneDataGenerator()
        {
            GenerateSceneData();
        }

        public static void CreateSceneAssetAtPath()
        {
            var absolutePath = EditorUtility.SaveFilePanel(
                "Select Folder", "Assets", "New Scene", "unity");

            if (string.IsNullOrEmpty(absolutePath)) return;

            if (!absolutePath.Contains("/Assets/"))
            {
                Debug.LogWarning($"Suspicious action | Path was outside the Assets folder | {absolutePath}");

                var quickLoadElements = ResourcesLocator.GetQuickLoadElements();
                for (var index = quickLoadElements.Elements.Count - 1; index >= 0; index--)
                {
                    var sceneDataAtIndex = AssetDatabase.GetAssetPath(quickLoadElements.Elements[index].SceneData);
                    var sceneAssetPath = Path.ChangeExtension(sceneDataAtIndex, ".unity");

                    var normalizedAbsolutePath = Path.GetFullPath(absolutePath)
                        .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                    var normalizedSceneAssetPath = Path.GetFullPath(sceneAssetPath)
                        .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

                    if (!normalizedAbsolutePath.Equals(normalizedSceneAssetPath, StringComparison.OrdinalIgnoreCase))
                        continue;

                    ResourcesLocator.GetQuickLoadElements().Remove(quickLoadElements.Elements[index].SceneData);
                }
            }
            else
            {
                var sceneData = GetSceneDataFromAbsolutePath(absolutePath);
                if (sceneData)
                {
                    ResourcesLocator.GetQuickLoadElements().Remove(sceneData);
                }
            }

            EditorSceneManager.SaveScene(EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects), absolutePath);

            if (!absolutePath.Contains("/Assets/"))
            {
                File.Delete(absolutePath + ".meta");
            }

            AssetDatabase.Refresh();
        }

        public static void GenerateSceneData()
        {
            var settings =
                (Settings)ResourcesSolver.EnsureAssetExistence(typeof(Settings), out _);

            if (!EditorPrefs.GetBool(Settings.GenerateSceneDataKey)) return;

            List<string> paths = new();

            var guids = AssetDatabase.FindAssets("t:SceneAsset");
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                paths.Add(Path.ChangeExtension(path, ".asset"));
            }

            var shouldSave = false;
            foreach (var path in paths)
            {
                var sceneData = AssetDatabase.LoadAssetAtPath<SceneData>(path);

                if (sceneData) continue;

                shouldSave = true;
                sceneData = ScriptableObject.CreateInstance<SceneData>();
                AssetDatabase.CreateAsset(sceneData, path);
                ResourcesLocator.GetQuickLoadElements().Add(sceneData);
            }

            EditorPrefs.SetBool(Settings.GenerateSceneDataKey, false);
            EditorUtility.SetDirty(settings);
            AssetDatabase.SaveAssetIfDirty(settings);

            if (!shouldSave) return;
            ResourcesLocator.SaveQuickLoadElementsAndTagContainerDelayed();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public static void PopulateQuickLoadElements()
        {
            var assetGuids = AssetDatabase.FindAssets("t:SceneData");
            var QuickLoadElements = ResourcesLocator.GetQuickLoadElements();
            foreach (var guid in assetGuids)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                var sceneData = AssetDatabase.LoadAssetAtPath<SceneData>(assetPath);
                if (QuickLoadElements.Contains(sceneData)) continue;
                var element = QuickLoadElements.Add(sceneData);

                var path = AssetDatabase.GetAssetPath(sceneData);
                element.IsOutsideAssetsFolder = !path.Contains("Assets/");
                Signals.Dispatch<UISignals.RefreshView>();
            }

            ResourcesLocator.SaveQuickLoadElementsAndTagContainerDelayed();
            AssetDatabase.Refresh();
        }

        private static SceneData GetSceneDataFromAbsolutePath(string absolutePath)
        {
            var indexOfAssets = absolutePath.IndexOf("Assets/", StringComparison.Ordinal);
            var relativePath = absolutePath[indexOfAssets..];
            var sceneData = AssetDatabase.LoadAssetAtPath<SceneData>(Path.ChangeExtension(relativePath, ".asset"));
            return sceneData;
        }
    }
}