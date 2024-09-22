using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Ludwell.SceneManagerToolkit.Editor
{
    public static class EditorSceneManagerHelper
    {
        public static bool IsPathOutsideAssets(string path)
        {
            return !path.Contains("Assets/");
        }

        public static void OpenScene(string path)
        {
            EditorSceneManager.OpenScene(path);
        }

        public static void OpenSceneAdditive(string path)
        {
            EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);
        }

        /// <summary>
        /// Close a scene from the hierarchy.
        /// </summary>
        /// <param name="path">The scene to close.</param>
        /// <param name="isRemove">Should the scene be removed from the hierarchy.</param>
        public static void CloseScene(string path, bool isRemove)
        {
            var scene = SceneManager.GetSceneByPath(path);
            EditorSceneManager.CloseScene(scene, isRemove);
        }

        public static void RemoveSceneAdditive(string path)
        {
            var scene = SceneManager.GetSceneByPath(path);
            EditorSceneManager.CloseScene(scene, true);
        }

        public static bool IsSceneInBuildSettings(string path)
        {
            var buildScenes = EditorBuildSettings.scenes;

            foreach (var buildScene in buildScenes)
            {
                if (!string.Equals(buildScene.path, path)) continue;
                return true;
            }

            return false;
        }

        public static bool IsSceneEnabledInBuildSettings(string path)
        {
            var buildScenes = EditorBuildSettings.scenes;

            foreach (var buildScene in buildScenes)
            {
                if (!string.Equals(buildScene.path, path)) continue;
                return buildScene.enabled;
            }

            return false;
        }

        public static void AddSceneToBuildSettings(string path)
        {
            if (IsSceneInBuildSettings(path)) return;

            var buildSettingsScenes = EditorBuildSettings.scenes;
            ArrayUtility.Add(ref buildSettingsScenes, new EditorBuildSettingsScene(path, true));
            EditorBuildSettings.scenes = buildSettingsScenes;
        }

        public static void RemoveSceneFromBuildSettings(string path)
        {
            var buildSettingsScenes = EditorBuildSettings.scenes;

            for (var i = buildSettingsScenes.Length - 1; i >= 0; i--)
            {
                if (!string.Equals(buildSettingsScenes[i].path, path)) continue;

                ArrayUtility.RemoveAt(ref buildSettingsScenes, i);
                EditorBuildSettings.scenes = buildSettingsScenes;
                break;
            }
        }

        public static void EnableSceneInBuildSettings(string path, bool state)
        {
            var buildScenes = EditorBuildSettings.scenes;

            foreach (var buildScene in buildScenes)
            {
                if (!string.Equals(buildScene.path, path)) continue;
                buildScene.enabled = state;
                EditorBuildSettings.scenes = buildScenes;
                break;
            }
        }

        public static bool IsActiveScene(string path)
        {
            return EditorSceneManager.GetActiveScene().path == path;
        }

        public static bool IsSceneLoaded(string path)
        {
            return EditorSceneManager.GetSceneByPath(path).isLoaded;
        }

        public static bool IsSceneUnloadedInHierarchy(string path)
        {
            for (var i = 0; i < SceneManager.sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                if (scene.path == path) return !scene.isLoaded;
            }

            return false;
        }

        public static bool IsSceneValid(string path)
        {
            return EditorSceneManager.GetSceneByPath(path).IsValid();
        }

        public static void SetActiveScene(string path)
        {
            EditorSceneManager.SetActiveScene(EditorSceneManager.GetSceneByPath(path));
        }

        public static bool DoesHierarchyOnlyHasOneLoadedScene()
        {
            var count = 0;
            for (var i = 0; i < SceneManager.sceneCount; i++)
            {
                var scene = EditorSceneManager.GetSceneAt(i);
                if (scene.isLoaded) count++;
                if (count > 1) return false;
            }

            return true;
        }

        public static IEnumerable<Scene> GetDirtyScenes()
        {
            for (var i = 0; i < SceneManager.sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                if (scene.isDirty)
                {
                    yield return scene;
                }
            }
            
            // var dirtyScenes = new List<Scene>();
            // for (var i = 0; i < SceneManager.sceneCount; i++)
            // {
            //     var scene = SceneManager.GetSceneAt(i);
            //     if (scene.isDirty)
            //     {
            //         dirtyScenes.Add(scene);
            //     }
            // }
        }

        /// <param name="modifiedScenes">Were the SceneAssets saved</param>
        /// <returns>Returns false only if a cancellation occured.</returns>
        internal static bool SaveDirtyScenesDialogComplex()
        {
            var dirtyScenes = GetDirtyScenes();

            if (!dirtyScenes.Any()) return false;
            
            var namesAsStrings = "";
            foreach (var scene in dirtyScenes)
            {
                namesAsStrings += scene.name + "\n";
            }

            var option = EditorUtility.DisplayDialogComplex(
                "Scene(s) Have Been Modified",
                "Do you want to save the changes you made in the scenes:" +
                $"\n{namesAsStrings}\n" +
                "Your changes will be lost if you don't save them.",
                "Save",
                "Don't Save",
                "Cancel"
            );

            switch (option)
            {
                case 0:
                    foreach (var scene in dirtyScenes)
                    {
                        if (!ResourcesLocator.GetSceneAssetDataBinders().ContainsWithPath(scene.path)) continue;
                        if (scene.isDirty) EditorSceneManager.SaveScene(scene);
                    }

                    return true;
                case 1:
                    foreach (var scene in dirtyScenes)
                    {
                        if (!ResourcesLocator.GetSceneAssetDataBinders().ContainsWithPath(scene.path)) continue;

                        if (scene.isDirty)
                        {
                            EditorSceneManager.OpenScene(scene.path, OpenSceneMode.AdditiveWithoutLoading);
                        }
                    }

                    return true;
                case 2:
                    return false;
            }

            return false;
        }

        /// <param name="modifiedScenes">Were the SceneAssets saved</param>
        /// <returns>Returns false only if a cancellation occured.</returns>
        internal static bool SaveSceneDialogComplex(params SceneElementController[] modifiedScenes)
        {
            var namesAsStrings = "";
            foreach (var controller in modifiedScenes)
            {
                namesAsStrings += controller.Scene.name + "\n";
            }

            var option = EditorUtility.DisplayDialogComplex(
                "Scene(s) Have Been Modified",
                "Do you want to save the changes you made in the scenes:" +
                $"\n{namesAsStrings}\n" +
                "Your changes will be lost if you don't save them.",
                "Save",
                "Don't Save",
                "Cancel"
            );

            switch (option)
            {
                case 0:
                    foreach (var controller in modifiedScenes)
                    {
                        var sceneReference = controller.Scene;
                        if (sceneReference.isDirty) EditorSceneManager.SaveScene(sceneReference);
                    }

                    return true;
                case 1:
                    foreach (var controller in modifiedScenes)
                    {
                        var sceneReference = controller.Scene;
                        if (sceneReference.isDirty)
                        {
                            EditorSceneManager.OpenScene(sceneReference.path, OpenSceneMode.AdditiveWithoutLoading);
                        }
                    }

                    return true;
                case 2:
                    return false;
            }

            return false;
        }
    }
}
