using System.Collections.Generic;
using System.Linq;
using Ludwell.UIToolkitUtilities.Editor;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Ludwell.SceneManagerToolkit.Editor
{
    internal class SceneElementsContextualMenu
    {
        private readonly ListViewHandler<SceneElementController, SceneAssetDataBinder> _listViewHandler;

        public SceneElementsContextualMenu(
            ListViewHandler<SceneElementController, SceneAssetDataBinder> listViewHandler)
        {
            _listViewHandler = listViewHandler;
            InitializeContextualMenuManipulator();
        }

        private void InitializeContextualMenuManipulator()
        {
            _listViewHandler.ListView.AddManipulator(new ContextualMenuManipulator(context =>
            {
                var controllers = GetSceneElementControllersInHierarchy();

                var defaultValidation =
                    _listViewHandler.ListView.selectedIndices.Any() && !EditorApplication.isPlaying;
                var selectionNotInHierarchy = controllers.Count == 0;
                var onlyOneLoadedScene = EditorSceneManagerHelper.DoesHierarchyOnlyHasOneLoadedScene();

                var defaultStatus =
                    !defaultValidation
                        ? DropdownMenuAction.Status.Disabled
                        : DropdownMenuAction.Status.Normal;

                var isCount = controllers.Count == SceneManager.sceneCount;
                var isActiveScene =
                    controllers.Any() && controllers[0].Scene.path == SceneManager.GetActiveScene().path;
                var destructiveStatus =
                    !defaultValidation || (onlyOneLoadedScene && isActiveScene) || selectionNotInHierarchy || isCount
                        ? DropdownMenuAction.Status.Disabled
                        : DropdownMenuAction.Status.Normal;

                context.menu.AppendAction("Load selection additively", LoadSelectionAdditive, defaultStatus);
                context.menu.AppendAction("Unload selection additively", UnloadSelectionAdditive, destructiveStatus);
                context.menu.AppendAction("Open selection additively", OpenSelectionAdditive, defaultStatus);
                context.menu.AppendAction("Remove selection additively", RemoveSelectionAdditive, destructiveStatus);
                context.menu.AppendSeparator();
                context.menu.AppendAction("Add selection to build settings", AddSelectionToBuildSettings,
                    defaultStatus);
                context.menu.AppendAction("Remove selection from build settings", RemoveSelectionFromBuildSettings,
                    defaultStatus);
                context.menu.AppendAction("Enable selection to in build settings", EnableSelectionInBuildSettings,
                    defaultStatus);
                context.menu.AppendAction("Disable selection to in build settings", DisableSelectionInBuildSettings,
                    defaultStatus);
                context.menu.AppendSeparator();
                context.menu.AppendAction("Add selection to addressable default group", AddSelectionToAddressables,
                    defaultStatus);
                context.menu.AppendAction("Remove selection from addressables", RemoveSelectionFromAddressables,
                    defaultStatus);
            }));
        }

        private SceneElementController[] GetSceneElementControllers()
        {
            var enumerableSelection = _listViewHandler.GetSelectedVisualElements();
            return enumerableSelection as SceneElementController[] ?? enumerableSelection.ToArray();
        }

        private List<SceneElementController> GetSceneElementControllersInHierarchy()
        {
            var enumerableSelection = _listViewHandler.GetSelectedVisualElements();

            List<SceneElementController> controllers = new();
            foreach (var controller in enumerableSelection)
            {
                for (var i = 0; i < SceneManager.sceneCount; i++)
                {
                    var scene = EditorSceneManager.GetSceneAt(i);
                    if (controller.Scene != scene) continue;
                    controllers.Add(controller);
                }
            }

            return controllers;
        }

        private void LoadSelectionAdditive(DropdownMenuAction _)
        {
            if (SceneManager.sceneCount == 1) return;

            var controllers = GetSceneElementControllersInHierarchy();

            foreach (var controller in controllers)
            {
                controller.LoadSceneAdditive();
            }
        }

        private void UnloadSelectionAdditive(DropdownMenuAction _)
        {
            var controllers = GetSceneElementControllersInHierarchy();

            var modifiedScenes = controllers.Where(controller => controller.Scene.isDirty).ToArray();

            if (modifiedScenes.Length > 0 && !EditorSceneManagerHelper.SaveSceneDialogue(modifiedScenes)) return;

            foreach (var controller in controllers)
            {
                controller.UnloadSceneAdditive();
            }
        }

        private void OpenSelectionAdditive(DropdownMenuAction _)
        {
            var controllers = GetSceneElementControllers();
            foreach (var controller in controllers)
            {
                controller.OpenSceneAdditive();
            }
        }

        private void RemoveSelectionAdditive(DropdownMenuAction _)
        {
            var controllers = GetSceneElementControllers();
            foreach (var controller in controllers)
            {
                controller.RemoveSceneAdditive();
            }
        }

        private void AddSelectionToBuildSettings(DropdownMenuAction _)
        {
            var controllers = GetSceneElementControllers();
            if (!controllers.Any()) return;
            foreach (var controller in controllers)
            {
                controller.AddToBuildSettings();
            }
        }

        private void RemoveSelectionFromBuildSettings(DropdownMenuAction _)
        {
            var controllers = GetSceneElementControllers();
            if (!controllers.Any()) return;
            foreach (var controller in controllers)
            {
                controller.RemoveFromBuildSettings();
            }
        }

        private void EnableSelectionInBuildSettings(DropdownMenuAction _)
        {
            var controllers = GetSceneElementControllers();
            if (!controllers.Any()) return;
            foreach (var controller in controllers)
            {
                controller.EnableInBuildSettings();
            }
        }

        private void DisableSelectionInBuildSettings(DropdownMenuAction _)
        {
            var enumerableSelection = _listViewHandler.GetSelectedVisualElements();
            var controllers =
                enumerableSelection as SceneElementController[] ?? enumerableSelection.ToArray();
            if (!controllers.Any()) return;
            foreach (var controller in controllers)
            {
                controller.DisableInBuildSettings();
            }
        }

        private void AddSelectionToAddressables(DropdownMenuAction _)
        {
            var enumerableSelection = _listViewHandler.GetSelectedVisualElements();
            var controllers = enumerableSelection as SceneElementController[] ?? enumerableSelection.ToArray();
            foreach (var controller in controllers)
            {
                controller.AddToAddressables();
            }
        }

        private void RemoveSelectionFromAddressables(DropdownMenuAction _)
        {
            var enumerableSelection = _listViewHandler.GetSelectedVisualElements();
            var controllers = enumerableSelection as SceneElementController[] ?? enumerableSelection.ToArray();
            foreach (var controller in controllers)
            {
                controller.RemoveFromAddressables();
            }
        }
    }
}
