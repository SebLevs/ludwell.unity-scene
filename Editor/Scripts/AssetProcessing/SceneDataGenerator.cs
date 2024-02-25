using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Ludwell.Scene.Editor
{
    [InitializeOnLoad]
    public class SceneDataGenerator : MonoBehaviour
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

            EditorSceneManager.SaveScene(EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects), absolutePath);
            AssetDatabase.Refresh();

            ReplaceMissingQuickLoadReferences(absolutePath);
        }

        public static void GenerateSceneData()
        {
            var settings = Resources.Load<SceneDataManagerSettings>(nameof(SceneDataManagerSettings));

            if (!settings.GenerateSceneData) return;

            List<string> paths = new();

            var guids = AssetDatabase.FindAssets("t:SceneAsset");
            foreach (var guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var directory = Path.GetDirectoryName(path);
                var name = Path.GetFileNameWithoutExtension(path);
                paths.Add(Path.Combine(directory, name + ".asset"));
            }

            var shouldSave = false;
            foreach (var path in paths)
            {
                var sceneData = AssetDatabase.LoadAssetAtPath<SceneData>(path);

                if (sceneData) continue;

                shouldSave = true;
                sceneData = ScriptableObject.CreateInstance<SceneData>();
                AssetDatabase.CreateAsset(sceneData, path);
                AddSceneDataToQuickLoadContainer(sceneData);
            }

            settings.GenerateSceneData = false;
            EditorUtility.SetDirty(settings);
            AssetDatabase.SaveAssetIfDirty(settings);
            Debug.Log("SceneData generation completed.");

            if (!shouldSave) return;
            LoaderSceneDataHelper.SaveChange();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static void ReplaceMissingQuickLoadReferences(string absolutePath)
        {
            var sceneData = GetSceneDataFromAbsolutePath(absolutePath);

            var loaderSceneDataElements = LoaderSceneDataHelper.GetLoaderSceneData().Elements;
            LoaderListViewElementData replacedElement = null;
            foreach (var element in loaderSceneDataElements)
            {
                if (element.MainScene != null) continue;

                if (element.Name == sceneData.Name)
                {
                    replacedElement = element;
                    continue;
                }

                element.MainScene = sceneData;
            }

            loaderSceneDataElements.Remove(replacedElement);

            Signals.Dispatch<UISignals.RefreshQuickLoadListView>();
        }

        private static SceneData GetSceneDataFromAbsolutePath(string absolutePath)
        {
            var indexOfAssets = absolutePath.IndexOf("Assets/", StringComparison.Ordinal);
            var relativePath = absolutePath[indexOfAssets..];
            var directory = Path.GetDirectoryName(relativePath);
            var assetName = Path.GetFileNameWithoutExtension(relativePath);
            var sceneData = AssetDatabase.LoadAssetAtPath<SceneData>(Path.Combine(directory, assetName + ".asset"));
            return sceneData;
        }

        private static void AddSceneDataToQuickLoadContainer(SceneData sceneData)
        {
            var container = LoaderSceneDataHelper.GetLoaderSceneData();
            container.Elements.Add(new LoaderListViewElementData()
            {
                Name = sceneData.Name,
                MainScene = sceneData
            });
        }
    }
}