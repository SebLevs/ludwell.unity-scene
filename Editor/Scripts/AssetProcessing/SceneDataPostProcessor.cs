using System.IO;
using UnityEditor;
using UnityEngine;

namespace Ludwell.Scene.Editor
{
    public class SceneDataPostProcessor : AssetPostprocessor
    {
        private static bool _isHandling;

        private static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            if (_isHandling) return;

            if (movedFromAssetPaths.Length > 0)
            {
                for (var index = 0; index < movedAssets.Length; index++)
                {
                    if (!movedAssets[index].EndsWith(".unity") && !movedAssets[index].EndsWith(".asset")) continue;
                    _isHandling = true;

                    var oppositeExtension = movedAssets[index].EndsWith(".unity") ? ".asset" : ".unity";

                    var oldAssetName = Path.GetFileNameWithoutExtension(movedFromAssetPaths[index]);
                    var oldPath = Path.GetDirectoryName(movedFromAssetPaths[index]);
                    var oppositeOldPathFull = Path.Combine(oldPath, oldAssetName + oppositeExtension);

                    var newAssetName = Path.GetFileNameWithoutExtension(movedAssets[index]);
                    var newPath = Path.GetDirectoryName(movedAssets[index]);
                    var oppositeNewPathFull = Path.Combine(newPath, newAssetName + oppositeExtension);

                    AssetDatabase.MoveAsset(oppositeOldPathFull, oppositeNewPathFull);

                    LoaderSceneDataHelper.GetLoaderSceneData().UpdateElement(oldAssetName, newAssetName);
                    LoaderSceneDataHelper.SaveChange();
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();

                    _isHandling = false;
                }

                return;
            }

            foreach (var importedAsset in importedAssets)
            {
                if (!importedAsset.EndsWith(".unity")) continue;

                var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(importedAsset);
                var sceneData = CreateSceneDataInstance(sceneAsset);

                var directoryPath = Path.GetDirectoryName(importedAsset);
                var assetName = sceneData.EditorSceneAsset.name + ".asset";
                var createAssetAtPath = Path.Combine(directoryPath, assetName);
                CreateAndSaveSceneDataAsset(sceneData, createAssetAtPath);
            }
        }

        private static SceneData CreateSceneDataInstance(SceneAsset sceneAsset)
        {
            var sceneData = ScriptableObject.CreateInstance<SceneData>();
            sceneData.EditorSceneAsset = sceneAsset;
            return sceneData;
        }

        private static void CreateAndSaveSceneDataAsset(SceneData sceneData, string fullAssetPath)
        {
            AssetDatabase.CreateAsset(sceneData, fullAssetPath);
            AddSceneDataToQuickLoadContainer(fullAssetPath);
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
            LoaderSceneDataHelper.SaveChange();
        }
    }
}