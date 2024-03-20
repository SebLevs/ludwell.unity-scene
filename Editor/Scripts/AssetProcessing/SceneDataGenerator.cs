using System;
using System.Collections.Generic;
using System.IO;
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
            var absolutePath = EditorUtility.SaveFilePanel("Select Folder", "Assets", "New Scene", "unity");

            if (string.IsNullOrEmpty(absolutePath)) return;

            var projectPath = Application.dataPath.Replace("/Assets", "");
            if (!absolutePath.StartsWith(projectPath))
            {
                Debug.LogError($"Operation cancelled | Invalid path | {absolutePath}");
                return;
            }

            var sceneData = GetSceneDataFromAbsolutePath(absolutePath);
            if (sceneData)
            {
                DataFetcher.GetQuickLoadElements().RemoveElement(sceneData);
            }

            EditorSceneManager.SaveScene(EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects), absolutePath);
            DataFetcher.SaveQuickLoadElementsAndTagContainerDelayed();
            AssetDatabase.Refresh();
        }

        public static void GenerateSceneData()
        {
            var settings = Resources.Load<SceneDataManagerSettings>(Path.Combine("Scriptables", nameof(SceneDataManagerSettings)));

            if (!settings.GenerateSceneData) return;

            List<string> paths = new();

            var guids = AssetDatabase.FindAssets("t:SceneAsset");
            foreach (var guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
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
                DataFetcher.GetQuickLoadElements().AddElement(sceneData);
            }

            settings.GenerateSceneData = false;
            EditorUtility.SetDirty(settings);
            AssetDatabase.SaveAssetIfDirty(settings);

            if (!shouldSave) return;
            DataFetcher.SaveQuickLoadElementsAndTagContainerDelayed();
            AssetDatabase.SaveAssets();
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