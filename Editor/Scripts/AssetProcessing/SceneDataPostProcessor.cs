using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Ludwell.Scene.Editor
{
    public class SceneDataPostProcessor : AssetPostprocessor
    {
        private static bool _isHandlingMove;
        private static bool _isHandlingImport;

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
            if (!TryHandleMove(movedAssets, movedFromAssetPaths) && !TryHandleImport(importedAssets)) return;
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static bool TryHandleMove(IReadOnlyList<string> movedAssets, IReadOnlyList<string> movedFromAssetPaths)
        {
            if (movedAssets.Count <= 0) return false;
            if (_isHandlingMove) return false;

            _isHandlingMove = true;
            var shouldSave = false;

            for (var index = 0; index < movedAssets.Count; index++)
            {
                var isAsset = movedAssets[index].EndsWith(".asset");
                if (!movedAssets[index].EndsWith(".unity") && !isAsset) continue;

                if (isAsset)
                {
                    var sceneData = AssetDatabase.LoadAssetAtPath<SceneData>(movedAssets[index]);
                    if (!sceneData) continue;
                }

                shouldSave = true;
                Debug.LogError("MOVED");

                var oppositeExtension = movedAssets[index].EndsWith(".unity") ? ".asset" : ".unity";

                var oppositeOldPathFull = Path.ChangeExtension(movedFromAssetPaths[index], oppositeExtension);
                var oppositeNewPathFull = Path.ChangeExtension(movedAssets[index], oppositeExtension);
                AssetDatabase.MoveAsset(oppositeOldPathFull, oppositeNewPathFull);

                var oldAssetName = Path.GetFileNameWithoutExtension(movedFromAssetPaths[index]);
                var newAssetName = Path.GetFileNameWithoutExtension(movedAssets[index]);
                LoaderSceneDataHelper.GetLoaderSceneData().UpdateElement(oldAssetName, newAssetName);
            }

            _isHandlingMove = false;
            return shouldSave;
        }

        private static bool TryHandleImport(IReadOnlyCollection<string> importedAssets)
        {
            if (importedAssets.Count <= 0) return false;
            if (_isHandlingImport) return false;

            _isHandlingImport = true;
            var shouldSave = false;

            foreach (var asset in importedAssets)
            {
                if (asset.EndsWith(".unity"))
                {
                    shouldSave = true;
                    CreateSceneDataAsset(asset);
                }
                else if (asset.EndsWith(".asset"))
                {
                    if (!AssetDatabase.LoadAssetAtPath<SceneData>(asset)) continue;
                    shouldSave = true;
                    CreateSceneAsset(asset);
                }
            }

            _isHandlingImport = false;

            return shouldSave;
        }

        private static void CreateSceneDataAsset(string sceneAssetPath)
        {
            var sceneDataPath = Path.ChangeExtension(sceneAssetPath, ".asset");

            var sceneData = AssetDatabase.LoadAssetAtPath<SceneData>(sceneDataPath);
            if (sceneData)
            {
                LoaderSceneDataHelper.GetLoaderSceneData().AddElement(sceneData);
                return;
            }

            var myAsset = ScriptableObject.CreateInstance<SceneData>();
            AssetDatabase.CreateAsset(myAsset, sceneDataPath);
            LoaderSceneDataHelper.GetLoaderSceneData().AddElement(myAsset);
        }

        private static void CreateSceneAsset(string sceneDataPath)
        {
            var scenePath = Path.ChangeExtension(sceneDataPath, ".unity");

            if (AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath)) return;
            var newScene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

            if (!newScene.IsValid()) return;
            EditorSceneManager.SaveScene(newScene, scenePath);

            var sceneData = AssetDatabase.LoadAssetAtPath<SceneData>(sceneDataPath);
            LoaderSceneDataHelper.GetLoaderSceneData().AddElement(sceneData);
        }
    }
}