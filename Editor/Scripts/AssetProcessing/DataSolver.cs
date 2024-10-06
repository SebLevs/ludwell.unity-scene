using System;
using System.IO;
using System.Linq;
using Ludwell.Architecture;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Ludwell.SceneManagerToolkit.Editor
{
    internal static class DataSolver
    {
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
                        .Remove(sceneAssetDataContainer.Elements[index].Data.GUID);
                }
            }
            else
            {
                var binder = ResourcesLocator.GetSceneAssetDataBinders().GetBinderFromPath(absolutePath);
                if (binder != null)
                {
                    ResourcesLocator.GetSceneAssetDataBinders().Remove(binder.Data.GUID);
                }
            }

            var anyDirtyScene = EditorSceneManagerHelper.GetDirtyScenes().Any();
            if (anyDirtyScene)
            {
                var savedDirtyScenes = EditorSceneManagerHelper.SaveDirtyScenesDialogComplex();
                if (!savedDirtyScenes) return;
            }

            var newScene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects);
            EditorSceneManager.SaveScene(newScene, absolutePath);

            if (!absolutePath.Contains("/Assets/"))
            {
                File.Delete(absolutePath + ".meta");
            }

            AssetDatabase.Refresh();
            EditorSceneManagerHelper.OpenScene(newScene.path);
        }

        public static void PopulateSceneAssetDataBinders()
        {
            var assetGuids = AssetDatabase.FindAssets("t:SceneAsset");
            var hasSolved = false;
            foreach (var guid in assetGuids)
            {
                hasSolved = AddSceneAssetDataBinderFromGuid(guid) || hasSolved;
            }

            if (!hasSolved) return;

            Signals.Dispatch<UISignals.RefreshView>();
            ResourcesLocator.SaveSceneAssetDataBindersDelayed();
        }

        public static bool AddSceneAssetDataBinderFromGuid(string guid)
        {
            var binders = ResourcesLocator.GetSceneAssetDataBinders();
            var assetPath = AssetDatabase.GUIDToAssetPath(guid);

            var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(assetPath);

            var address = SceneAssetDataBinders.NotAddressableName;
#if USE_ADDRESSABLES_EDITOR
            address = AddressablesProcessor.GetAddressableIDForGUID(guid);
#endif
            var hasAdded = binders.TryAddUnique(guid, sceneAsset.name, assetPath, address);

            return hasAdded;
        }

        public static void RemoveAllInvalidSceneAssetDataBinders()
        {
            var binders = ResourcesLocator.GetSceneAssetDataBinders();
            var assetGuids = AssetDatabase.FindAssets("t:SceneAsset");
            var hasSolved = false;

            for (var i = binders.Elements.Count - 1; i >= 0; i--)
            {
                if (assetGuids.Contains(binders.Elements[i].Data.GUID)) continue;
                binders.Remove(binders.Elements[i].Data.GUID);
                hasSolved = true;
            }

            if (!hasSolved) return;

            Signals.Dispatch<UISignals.RefreshView>();
            ResourcesLocator.SaveSceneAssetDataBindersDelayed();
        }

        public static void SolveTagBindings()
        {
            var dataBinders = ResourcesLocator.GetSceneAssetDataBinders().Elements;
            var tagContainer = ResourcesLocator.GetTags();

            var hasSolved = false;

            foreach (var dataBinder in dataBinders)
            {
                for (var index = dataBinder.Tags.Count - 1; index >= 0; index--)
                {
                    var tag = dataBinder.Tags[index];
                    if (tagContainer.Elements.Contains(tag)) continue;
                    dataBinder.Tags.Remove(tag);
                    hasSolved = true;
                }
            }

            if (!hasSolved) return;

            Signals.Dispatch<UISignals.RefreshView>();
            ResourcesLocator.SaveSceneAssetDataBindersDelayed();
        }

        public static void SolveSceneAssetDataBinders()
        {
            PopulateSceneAssetDataBinders();
            RemoveAllInvalidSceneAssetDataBinders();
        }
    }
}
