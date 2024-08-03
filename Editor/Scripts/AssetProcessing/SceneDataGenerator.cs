using System;
using System.IO;
using Ludwell.Architecture;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

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

                var sceneAssetDataContainer = ResourcesLocator.GetSceneAssetDataContainer();
                for (var index = sceneAssetDataContainer.DataBinders.Count - 1; index >= 0; index--)
                {
                    var sceneDataAtIndex = sceneAssetDataContainer.DataBinders[index].Data.Path;
                    var sceneAssetPath = Path.ChangeExtension(sceneDataAtIndex, ".unity");

                    var normalizedAbsolutePath = Path.GetFullPath(absolutePath)
                        .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                    var normalizedSceneAssetPath = Path.GetFullPath(sceneAssetPath)
                        .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

                    if (!normalizedAbsolutePath.Equals(normalizedSceneAssetPath, StringComparison.OrdinalIgnoreCase))
                        continue;

                    ResourcesLocator.GetSceneAssetDataContainer()
                        .Remove(sceneAssetDataContainer.DataBinders[index].ID);
                }
            }
            else
            {
                var binder = ResourcesLocator.GetSceneAssetDataContainer().GetBinderFromPath(absolutePath);
                if (binder != null)
                {
                    ResourcesLocator.GetSceneAssetDataContainer().Remove(binder.ID);
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

            var instance = SceneAssetDataContainer.Instance;
            EditorUtility.SetDirty(instance);
            AssetDatabase.SaveAssetIfDirty(instance);

            ResourcesLocator.SaveSceneAssetContainerAndTagContainerDelayed();
            AssetDatabase.Refresh();
        }

        public static void AddFromGuid(string guid)
        {
            Debug.LogError("Setup addressable ID");
            var instance = SceneAssetDataContainer.Instance;
            var assetPath = AssetDatabase.GUIDToAssetPath(guid);
            var sceneData = AssetDatabase.LoadAssetAtPath<SceneAsset>(assetPath);
            if (instance.Contains(guid)) return;
            instance.Add(new SceneAssetDataBinder
            {
                ID = guid,
                Data = new SceneAssetData
                {
                    BuildIndex = SceneUtility.GetBuildIndexByScenePath(assetPath),
                    Name = sceneData.name,
                    Path = assetPath,
                    AddressableID = "TO BE FILLED"
                }
            });

            instance.DataBinders.Sort();
        }
    }
}