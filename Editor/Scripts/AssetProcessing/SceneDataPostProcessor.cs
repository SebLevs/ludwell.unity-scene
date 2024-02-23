using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace Ludwell.Scene.Editor
{
    public class SceneDataPostProcessor : AssetPostprocessor
    {
        private static bool _isHandling;

        [OnOpenAsset]
        private static bool HandleDoubleClick(int instanceId, int line)
        {
            var path = AssetDatabase.GetAssetPath(instanceId);
            var sceneData = AssetDatabase.LoadAssetAtPath<SceneData>(path);

            if (!sceneData) return false;

            SceneDataManagerEditorApplication.OpenScene(sceneData);

            return true;
        }

        private static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            if (_isHandling) return;

            var shouldSave = false;

            if (movedFromAssetPaths.Length > 0)
            {
                for (var index = 0; index < movedAssets.Length; index++)
                {
                    if (!movedAssets[index].EndsWith(".unity") && !movedAssets[index].EndsWith(".asset")) continue;
                    _isHandling = true;
                    shouldSave = true;

                    var oppositeExtension = movedAssets[index].EndsWith(".unity") ? ".asset" : ".unity";

                    var oldAssetName = Path.GetFileNameWithoutExtension(movedFromAssetPaths[index]);
                    var oldPath = Path.GetDirectoryName(movedFromAssetPaths[index]);
                    var oppositeOldPathFull = Path.Combine(oldPath, oldAssetName + oppositeExtension);

                    var newAssetName = Path.GetFileNameWithoutExtension(movedAssets[index]);
                    var newPath = Path.GetDirectoryName(movedAssets[index]);
                    var oppositeNewPathFull = Path.Combine(newPath, newAssetName + oppositeExtension);

                    AssetDatabase.MoveAsset(oppositeOldPathFull, oppositeNewPathFull);

                    LoaderSceneDataHelper.GetLoaderSceneData().UpdateElement(oldAssetName, newAssetName);

                    _isHandling = false;
                }

                if (!shouldSave) return;
                LoaderSceneDataHelper.SaveChange();
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                return;
            }

            foreach (var importedAsset in importedAssets)
            {
                if (!importedAsset.EndsWith(".unity")) continue;
                shouldSave = true;

                var directoryPath = Path.GetDirectoryName(importedAsset);
                var sceneData = ScriptableObject.CreateInstance<SceneData>();
                var assetName = Path.GetFileNameWithoutExtension(importedAsset) + ".asset";
                var createAssetAtPath = Path.Combine(directoryPath, assetName);
                AssetDatabase.CreateAsset(sceneData, createAssetAtPath);
                AddSceneDataToQuickLoadContainer(createAssetAtPath);
            }

            if (!shouldSave) return;
            LoaderSceneDataHelper.SaveChange();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static void AddSceneDataToQuickLoadContainer(string fullAssetPath)
        {
            var container = LoaderSceneDataHelper.GetLoaderSceneData();
            var sceneDataAsset = AssetDatabase.LoadAssetAtPath<SceneData>(fullAssetPath);
            container.Elements.Add(new LoaderListViewElementData()
            {
                Name = sceneDataAsset.Name,
                MainScene = sceneDataAsset
            });
        }
    }
}