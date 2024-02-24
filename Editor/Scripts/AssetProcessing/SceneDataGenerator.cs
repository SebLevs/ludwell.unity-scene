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
            var path = EditorUtility.SaveFilePanel("Select Folder", "Assets", "New Scene", "unity");
            
            if (string.IsNullOrEmpty(path)) return;
            
            var projectPath = Application.dataPath.Replace("/Assets", "");
            if (path.StartsWith(projectPath))
            {
                EditorSceneManager.SaveScene(EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects), path);
                AssetDatabase.Refresh();
            }
            else
            { 
                Debug.LogError($"Operation was aborted | Invalid path | \"{path}\"");
            }
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
            Debug.Log("SceneData were generated");

            if (!shouldSave) return;
            LoaderSceneDataHelper.SaveChange();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
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