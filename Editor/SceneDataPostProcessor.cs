using System.IO;
using UnityEngine;
using UnityEditor;

namespace Ludwell.Scene
{
    public class SceneDataPostProcessor : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(string[] importedAssets,
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

        private static void CreateAndSaveSceneDataAsset(SceneData sceneData, string createAssetAtPath)
        {
            AssetDatabase.CreateAsset(sceneData, createAssetAtPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}