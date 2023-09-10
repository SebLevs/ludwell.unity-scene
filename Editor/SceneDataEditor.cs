#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Ludwell.Scene
{
    public class SceneDataEditor : MonoBehaviour
    {
        private const string sceneFolder = "Assets/Scenes/";
        private static string[] sceneFolders = new[] { sceneFolder };
        private const string sceneDataFolder = "Assets/Scriptables/SceneDatas";
        private static string[] sceneDataFolders = new[] { sceneDataFolder };

        [MenuItem("Tool/SceneData/Create default folder paths")]
        private static void TryCreateDefaultFolders()
        {
            string[] sceneFolders = sceneFolder.Split('/');
            TryCreateFolders(sceneFolders);

            string[] assetFolders = sceneDataFolder.Split('/');
            TryCreateFolders(assetFolders);
        }

        private static void TryCreateFolders(string[] folders)
        {
            string folderPath = "";
            foreach (string assetFolder in folders)
            {
                if (!Directory.Exists(Path.Combine(folderPath, assetFolder)))
                {
                    AssetDatabase.CreateFolder(folderPath, assetFolder);
                }

                folderPath = Path.Combine(folderPath, assetFolder);
            }
        }

        [MenuItem("Tool/SceneData/Create SceneAssets with folder hierarchy")]
        private static void CreateSceneAssetsWithFolderHierarchy()
        {
            string[] assetFolders = sceneDataFolder.Split('/');
            TryCreateFolders(assetFolders);

            string defaultAssetDataPath = Path.Combine(sceneDataFolder.Split("/"));
            foreach (var sceneAsset in GetSceneAssets())
            {
                string createAssetAtPath = defaultAssetDataPath;
                SceneData sceneData = ScriptableObject.CreateInstance<SceneData>();
                sceneData.EditorSceneAsset = sceneAsset;

                string assetFolderName = Path.GetDirectoryName(AssetDatabase.GetAssetPath(sceneAsset.GetInstanceID()));
                string[] cleanedFolderPath = assetFolderName.Substring(sceneFolder.Length - 1).Split("\\");

                foreach (string folder in cleanedFolderPath)
                {
                    if (!Directory.Exists(Path.Combine(createAssetAtPath, folder)))
                    {
                        AssetDatabase.CreateFolder(createAssetAtPath, folder);
                    }

                    createAssetAtPath = Path.Combine(createAssetAtPath, folder);
                }

                createAssetAtPath = Path.Combine(createAssetAtPath, sceneData.EditorSceneAsset.name + ".asset");
                TryCreateSceneDataAsset(createAssetAtPath, sceneData);
            }
        }

        private static List<SceneAsset> GetSceneAssets()
        {
            string[] sceneAssetsGuid = AssetDatabase.FindAssets("t:SceneAsset", sceneFolders);
            List<SceneAsset> sceneAssets = new();
            foreach (var sceneGuid in sceneAssetsGuid)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(sceneGuid);
                sceneAssets.Add(AssetDatabase.LoadAssetAtPath<SceneAsset>(assetPath));
            }

            return sceneAssets;
        }

        private static void TryCreateSceneDataAsset(string specificAssetDataPath, SceneData sceneData)
        {
            SceneData existCheck = AssetDatabase.LoadAssetAtPath<SceneData>(specificAssetDataPath);
            if (!existCheck || existCheck.EditorSceneAsset != sceneData.EditorSceneAsset)
            {
                AssetDatabase.CreateAsset(sceneData, specificAssetDataPath);
            }
        }

        [MenuItem("Tool/SceneData/Update scene asset names")]
        private static void UpdateScriptableNameFromSceneAsset()
        {
            string[] sceneDatasGuid = AssetDatabase.FindAssets("t:SceneData", sceneDataFolders);
            foreach (var sceneDataGuid in sceneDatasGuid)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(sceneDataGuid);
                SceneData sceneData = AssetDatabase.LoadAssetAtPath<SceneData>(assetPath);
                if (sceneData.name == sceneData.EditorSceneAsset.name)
                {
                    continue;
                }

                AssetDatabase.RenameAsset(assetPath, sceneData.EditorSceneAsset.name);
            }

            AssetDatabase.SaveAssets();
        }
    }
}
#endif

/*
 [SerializeField] private SceneDataEditorSettings editorSettings;

        [MenuItem("Tool/SceneData/Create default folder paths")]
        public void TryCreateDefaultFolders()
        {
            string[] sceneFolders = editorSettings.SceneFolder.Split('/');
            TryCreateFolders(sceneFolders);

            string[] assetFolders = editorSettings.SceneDataFolder.Split('/');
            TryCreateFolders(assetFolders);
        }

        private static void TryCreateFolders(string[] folders)
        {
            string folderPath = "";
            foreach (string assetFolder in folders)
            {
                if (!Directory.Exists(Path.Combine(folderPath, assetFolder)))
                {
                    AssetDatabase.CreateFolder(folderPath, assetFolder);
                }

                folderPath = Path.Combine(folderPath, assetFolder);
            }
        }

        [MenuItem("Tool/SceneData/Create SceneAssets with folder hierarchy")]
        public void CreateSceneAssetsWithFolderHierarchy()
        {
            string[] assetFolders = editorSettings.SceneDataFolder.Split('/');
            TryCreateFolders(assetFolders);

            string defaultAssetDataPath = Path.Combine(editorSettings.SceneDataFolder.Split("/"));
            foreach (var sceneAsset in GetSceneAssets())
            {
                string createAssetAtPath = defaultAssetDataPath;
                SceneData sceneData = ScriptableObject.CreateInstance<SceneData>();
                sceneData.EditorSceneAsset = sceneAsset;

                string assetFolderName = Path.GetDirectoryName(AssetDatabase.GetAssetPath(sceneAsset.GetInstanceID()));
                string[] cleanedFolderPath = assetFolderName.Substring(editorSettings.SceneFolder.Length - 1).Split("\\");

                foreach (string folder in cleanedFolderPath)
                {
                    if (!Directory.Exists(Path.Combine(createAssetAtPath, folder)))
                    {
                        AssetDatabase.CreateFolder(createAssetAtPath, folder);
                    }

                    createAssetAtPath = Path.Combine(createAssetAtPath, folder);
                }

                createAssetAtPath = Path.Combine(createAssetAtPath, sceneData.EditorSceneAsset.name + ".asset");
                TryCreateSceneDataAsset(createAssetAtPath, sceneData);
            }
        }

        private List<SceneAsset> GetSceneAssets()
        {
            string[] sceneAssetsGuid = AssetDatabase.FindAssets("t:SceneAsset", editorSettings.GetSceneFolders);
            List<SceneAsset> sceneAssets = new();
            foreach (var sceneGuid in sceneAssetsGuid)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(sceneGuid);
                sceneAssets.Add(AssetDatabase.LoadAssetAtPath<SceneAsset>(assetPath));
            }

            return sceneAssets;
        }

        private void TryCreateSceneDataAsset(string specificAssetDataPath, SceneData sceneData)
        {
            SceneData existCheck = AssetDatabase.LoadAssetAtPath<SceneData>(specificAssetDataPath);
            if (!existCheck || existCheck.EditorSceneAsset != sceneData.EditorSceneAsset)
            {
                AssetDatabase.CreateAsset(sceneData, specificAssetDataPath);
            }
        }

        [MenuItem("Tool/SceneData/Update scene asset names")]
        public void UpdateScriptableNameFromSceneAsset()
        {
            string[] sceneDatasGuid = AssetDatabase.FindAssets("t:SceneData", editorSettings.GetSceneDataFolders);
            foreach (var sceneDataGuid in sceneDatasGuid)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(sceneDataGuid);
                SceneData sceneData = AssetDatabase.LoadAssetAtPath<SceneData>(assetPath);
                if (sceneData.name == sceneData.EditorSceneAsset.name)
                {
                    continue;
                }

                AssetDatabase.RenameAsset(assetPath, sceneData.EditorSceneAsset.name);
            }

            AssetDatabase.SaveAssets();
        }
        */