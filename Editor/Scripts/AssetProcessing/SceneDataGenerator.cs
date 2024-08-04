using System;
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
            // GenerateSceneData();
            PopulateQuickLoadElements();
        }

        public static void CreateSceneAssetAtPath()
        {
            var absolutePath = EditorUtility.SaveFilePanel(
                "Select Folder", "Assets", "New Scene", "unity");

            if (string.IsNullOrEmpty(absolutePath)) return;

            if (!absolutePath.Contains("/Assets/"))
            {
                Debug.LogWarning($"Suspicious action | Path was outside the Assets folder | {absolutePath}");

                var sceneAssetDataContainer = ResourcesLocator.GetSceneAssetDataBinders();
                for (var index = sceneAssetDataContainer.Elements.Count - 1; index >= 0; index--)
                {
                    var sceneDataAtIndex = sceneAssetDataContainer.Elements[index].Data.Path;
                    var sceneAssetPath = Path.ChangeExtension(sceneDataAtIndex, ".unity");

                    var normalizedAbsolutePath = Path.GetFullPath(absolutePath)
                        .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                    var normalizedSceneAssetPath = Path.GetFullPath(sceneAssetPath)
                        .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

                    if (!normalizedAbsolutePath.Equals(normalizedSceneAssetPath, StringComparison.OrdinalIgnoreCase))
                        continue;

                    ResourcesLocator.GetSceneAssetDataBinders()
                        .Remove(sceneAssetDataContainer.Elements[index].ID);
                }
            }
            else
            {
                var binder = ResourcesLocator.GetSceneAssetDataBinders().GetBinderFromPath(absolutePath);
                if (binder != null)
                {
                    ResourcesLocator.GetSceneAssetDataBinders().Remove(binder.ID);
                }
            }

            EditorSceneManager.SaveScene(EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects), absolutePath);

            if (!absolutePath.Contains("/Assets/"))
            {
                File.Delete(absolutePath + ".meta");
            }

            AssetDatabase.Refresh();
        }

        public static void PopulateQuickLoadElements()
        {
            var assetGuids = AssetDatabase.FindAssets("t:SceneAsset");
            foreach (var guid in assetGuids)
            {
                AddFromGuid(guid);
                Signals.Dispatch<UISignals.RefreshView>();
            }

            var instance = SceneAssetDataBinders.Instance;
            EditorUtility.SetDirty(instance);
            AssetDatabase.SaveAssetIfDirty(instance);

            ResourcesLocator.SaveSceneAssetDataBindersAndTagsDelayed();
            AssetDatabase.Refresh();
        }

        public static void AddFromGuid(string guid)
        {
            Debug.LogError("Setup addressable ID");
            var instance = SceneAssetDataBinders.Instance;
            var assetPath = AssetDatabase.GUIDToAssetPath(guid);
            var sceneData = AssetDatabase.LoadAssetAtPath<SceneAsset>(assetPath);
            if (instance.ContainsWithId(guid)) return;
            instance.Add(new SceneAssetDataBinder
            {
                ID = guid,
                Data = new SceneAssetData
                {
                    Name = sceneData.name,
                    Path = assetPath,
                    AddressableID = "TO BE FILLED"
                }
            });

            instance.Elements.Sort();
        }
    }
}