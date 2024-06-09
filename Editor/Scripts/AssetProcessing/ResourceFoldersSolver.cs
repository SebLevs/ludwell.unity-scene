using System.IO;
using UnityEditor;
using UnityEngine;

namespace Ludwell.Scene.Editor
{
    [InitializeOnLoad]
    public class ResourceFoldersSolver
    {
        private static readonly string[] EditorPath =
        {
            "Plugins",
            "SceneDataManager",
            "Editor",
            "Resources",
            "Scriptables"
        };
        
        private static readonly string[] RuntimePath =
        {
            "Plugins",
            "SceneDataManager",
            "Runtime",
            "Resources",
            "Scriptables"
        };

        static ResourceFoldersSolver()
        {
            TryCreateScriptableObject<SceneDataManagerSettings>(EditorPath);
            TryCreateScriptableObject<QuickLoadElements>(EditorPath);
            TryCreateScriptableObject<TagContainer>(EditorPath);
            TryCreateScriptableObject<CoreScenes>(RuntimePath); 
        }
        
        private static void TryCreateScriptableObject<T>(string[] path) where T : ScriptableObject
        {
            var assetPath = Path.Combine(TryCreatePath(path), typeof(T).Name + ".asset");
        
            bool existsAtPath = AssetDatabase.LoadAssetAtPath<T>(assetPath);
        
            if (existsAtPath) return;
        
            var scriptable = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(scriptable, assetPath);
        
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        
        private static string TryCreatePath(string[] paths)
        {
            var currentPath = "Assets";
         
            foreach (var folder in paths)
            {
                var newFolderPath = Path.Combine(currentPath, folder);
             
                if (!AssetDatabase.IsValidFolder(newFolderPath))
                {
                    AssetDatabase.CreateFolder(currentPath, folder);
                }
            
                currentPath = newFolderPath;
            }

            AssetDatabase.Refresh();

            return currentPath;
        }
    }
}