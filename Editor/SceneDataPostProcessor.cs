using System.IO;
using UnityEngine;
using UnityEditor;

namespace Ludwell.Scene.Editor
{
    public class SceneDataPostProcessor : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            foreach (string importedAsset in importedAssets)
            {
                if (importedAsset.EndsWith(".unity"))
                {
                    var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(importedAsset);
                    var sceneData = CreateSceneDataInstance(sceneAsset);

                    var directoryPath = Path.GetDirectoryName(importedAsset);
                    var assetName = sceneData.EditorSceneAsset.name + ".asset";
                    var createAssetAtPath = Path.Combine(directoryPath, assetName);
                    CreateAndSaveSceneDataAsset(sceneData, createAssetAtPath);
                }
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
            Debug.LogError($"scenedata: {sceneDataAsset}");
            container.Elements.Add(new LoaderListViewElementData()
            {
                Name = sceneDataAsset.Name,
                MainScene = sceneDataAsset
            });
        }
    }
}