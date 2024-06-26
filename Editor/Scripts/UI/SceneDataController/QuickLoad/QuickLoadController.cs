using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Ludwell.Scene.Editor
{
    public class QuickLoadController
    {
        private const string TagListingStrategyName = "tag";
        private const string TagIconName = "icon_tag";
        private const string HierarchyIconName = "icon_hierarchy";

        private readonly QuickLoadElements _quickLoadElements;
        private ListViewHandler<QuickLoadElementController, QuickLoadElementData> _listViewHandler;

        private readonly QuickLoadView _view;
        private readonly ListView _listView;
        private readonly DropdownSearchField _dropdownSearchField;

        private ListingStrategy _hierarchyListingStrategy;

        public QuickLoadController(VisualElement parent)
        {
            var root = parent.Q(nameof(QuickLoadView));
            _view = new QuickLoadView(root);
            _view.CloseAllButton.clicked += CloseAll;
            _view.ListHierarchyButton.clicked += ListSceneHierarchy;
            _view.AddButton.clicked += SceneDataGenerator.CreateSceneAssetAtPath;
            _view.RemoveButton.clicked += DeleteSelection;

            _listView = root.Q<ListView>();
            _dropdownSearchField = root.Q<DropdownSearchField>();

            _quickLoadElements = ResourcesLocator.GetQuickLoadElements();

            InitializeListViewHandler(root.Q<ListView>());
            InitializeSearchField(root, root.Q<DropdownSearchField>());
            _listView.RegisterCallback<KeyUpEvent>(OnKeyUpDeleteSelected);

            ResourcesLocator.QuickLoadController = this; // todo: change for DI or service

            EditorSceneManager.sceneOpened += HandleAdditiveSceneOpened;
            EditorSceneManager.sceneClosed += HandleAdditiveSceneClosed;
            EditorSceneManager.activeSceneChangedInEditMode += HandleActiveSceneChangeEditor;
            SceneManager.activeSceneChanged += HandleActiveSceneChangeRuntime;


            InitializeContextMenuManipulator();
        }

        private void ListSceneHierarchy()
        {
            Debug.LogError("ListSceneHierarchy");
        }

        internal void Dispose()
        {
            EditorSceneManager.sceneOpened -= HandleAdditiveSceneOpened;
            EditorSceneManager.sceneClosed -= HandleAdditiveSceneClosed;
            EditorSceneManager.activeSceneChangedInEditMode -= HandleActiveSceneChangeEditor;
            SceneManager.activeSceneChanged -= HandleActiveSceneChangeRuntime;
        }

        // todo: delete when either service or DI is implemented
        public void ScrollToItemIndex(int index)
        {
            _listViewHandler.ListView.ScrollToItem(index);
            _listViewHandler.ListView.SetSelection(index);
            _listViewHandler.GetVisualElementAt(index)?.FocusTextField();
        }

        public void ForceRebuildListView()
        {
            _listViewHandler.ForceRebuild();
            _dropdownSearchField.RebuildActiveListing();
        }

        private List<QuickLoadElementController> GetVisualElementsWithoutActiveScene()
        {
            var enumerableSelection = _listViewHandler.GetSelectedVisualElements();
            var quickLoadElementControllers =
                enumerableSelection as List<QuickLoadElementController> ?? enumerableSelection.ToList();

            if (!quickLoadElementControllers.Any()) return quickLoadElementControllers;

            var activeScene = quickLoadElementControllers.FirstOrDefault(x => x.IsActiveScene());
            if (activeScene != null) quickLoadElementControllers.Remove(activeScene);
            return quickLoadElementControllers;
        }

        private void OpenSelectionAdditive(DropdownMenuAction _)
        {
            var quickLoadElementControllers = GetVisualElementsWithoutActiveScene();
            foreach (var quickLoadElementController in quickLoadElementControllers)
            {
                quickLoadElementController.OpenSceneAdditive();
            }
        }

        private void RemoveSelectionAdditive(DropdownMenuAction _)
        {
            var quickLoadElementControllers = GetVisualElementsWithoutActiveScene();
            foreach (var quickLoadElementController in quickLoadElementControllers)
            {
                quickLoadElementController.RemoveSceneAdditive();
            }
        }

        private void AddSelectionToBuildSettings(DropdownMenuAction _)
        {
            var enumerableSelection = _listViewHandler.GetSelectedVisualElements();
            var quickLoadElementControllers =
                enumerableSelection as QuickLoadElementController[] ?? enumerableSelection.ToArray();
            if (!quickLoadElementControllers.Any()) return;
            foreach (var quickLoadElementController in quickLoadElementControllers)
            {
                quickLoadElementController.AddToBuildSettings();
            }

            Signals.Dispatch<UISignals.RefreshView>();
        }

        private void RemoveSelectionFromBuildSettings(DropdownMenuAction _)
        {
            var enumerableSelection = _listViewHandler.GetSelectedVisualElements();
            var quickLoadElementControllers =
                enumerableSelection as QuickLoadElementController[] ?? enumerableSelection.ToArray();
            if (!quickLoadElementControllers.Any()) return;
            foreach (var quickLoadElementController in quickLoadElementControllers)
            {
                quickLoadElementController.RemoveFromBuildSettings();
            }

            Signals.Dispatch<UISignals.RefreshView>();
        }

        private void HandleAdditiveSceneOpened(UnityEngine.SceneManagement.Scene scene, OpenSceneMode mode)
        {
            if (EditorApplication.isPlaying) return;
            if (mode == OpenSceneMode.Single) return;

            foreach (var quickLoadElementController in _listViewHandler.VisualElements)
            {
                if (quickLoadElementController.Model.SceneData.name != scene.name) continue;
                var scenePath = Path.ChangeExtension(
                    AssetDatabase.GetAssetPath(quickLoadElementController.Model.SceneData),
                    ".unity");
                if (scenePath != scene.path) continue;
                quickLoadElementController.SwitchOpenAdditiveButtonState(true);
                quickLoadElementController.SolveSetActiveButton();
                return;
            }
        }

        private void HandleAdditiveSceneClosed(UnityEngine.SceneManagement.Scene scene)
        {
            if (EditorApplication.isPlaying) return;
            foreach (var quickLoadElementController in _listViewHandler.VisualElements)
            {
                if (quickLoadElementController.Model.SceneData.name != scene.name) continue;
                var scenePath = Path.ChangeExtension(
                    AssetDatabase.GetAssetPath(quickLoadElementController.Model.SceneData),
                    ".unity");
                if (scenePath != scene.path) continue;
                quickLoadElementController.SetOpenButtonEnable(true);
                quickLoadElementController.SetOpenAdditiveButtonEnable(true);
                quickLoadElementController.SwitchOpenAdditiveButtonState(false);
                quickLoadElementController.SolveSetActiveButton();
                return;
            }
        }

        private void HandleActiveSceneChangeRuntime(UnityEngine.SceneManagement.Scene arg0,
            UnityEngine.SceneManagement.Scene arg1)
        {
            var breakAtCount = SceneManager.sceneCount;
            var count = 0;
            foreach (var quickLoadElementController in _listViewHandler.VisualElements)
            {
                var sceneDataName = quickLoadElementController.Model.SceneData.name;
                if (sceneDataName != arg0.name && sceneDataName != arg1.name) continue;
                var scenePath = Path.ChangeExtension(
                    AssetDatabase.GetAssetPath(quickLoadElementController.Model.SceneData), ".unity");
                if (scenePath != arg0.path && scenePath != arg1.path) continue;

                if (scenePath == arg0.path) // previous active scene
                {
                    quickLoadElementController.SolveSetActiveButton();
                    if (++count == breakAtCount) return;
                    continue;
                }

                if (scenePath != arg1.path) continue; // new active scene
                quickLoadElementController.SolveSetActiveButton();
                if (++count == breakAtCount) return;
            }
        }

        private void HandleActiveSceneChangeEditor(UnityEngine.SceneManagement.Scene arg0,
            UnityEngine.SceneManagement.Scene arg1)
        {
            var breakAtCount = SceneManager.sceneCount;
            var count = 0;
            foreach (var quickLoadElementController in _listViewHandler.VisualElements)
            {
                var sceneDataName = quickLoadElementController.Model.SceneData.name;
                if (sceneDataName != arg0.name && sceneDataName != arg1.name) continue;
                var scenePath = Path.ChangeExtension(
                    AssetDatabase.GetAssetPath(quickLoadElementController.Model.SceneData), ".unity");
                if (scenePath != arg0.path && scenePath != arg1.path) continue;

                if (scenePath == arg0.path) // previous active scene
                {
                    quickLoadElementController.SetOpenButtonEnable(true);
                    quickLoadElementController.SolveOpenAdditiveButton();
                    quickLoadElementController.SolveSetActiveButton();
                    if (++count == breakAtCount) return;
                    continue;
                }

                if (scenePath != arg1.path) continue; // new active scene
                quickLoadElementController.SetOpenButtonEnable(false);
                quickLoadElementController.SetOpenAdditiveButtonEnable(false);
                quickLoadElementController.SwitchOpenAdditiveButtonState(false);
                quickLoadElementController.SolveSetActiveButton();
                if (++count == breakAtCount) return;
            }
        }

        private void InitializeContextMenuManipulator()
        {
            _listViewHandler.ListView.AddManipulator(new ContextualMenuManipulator(context =>
            {
                Func<DropdownMenuAction, DropdownMenuAction.Status> status = DropdownMenuAction.AlwaysEnabled;
                context.menu.AppendAction("Open selection additively", OpenSelectionAdditive, status);
                context.menu.AppendAction("Remove selection additively", RemoveSelectionAdditive, status);
                context.menu.AppendSeparator();
                context.menu.AppendAction("Add selection to build settings", AddSelectionToBuildSettings, status);
                context.menu.AppendAction("Remove selection from build settings", RemoveSelectionFromBuildSettings,
                    status);
            }));
        }

        /// <summary> If no item is selected, deletes the last item. </summary>
        private void DeleteSelection()
        {
            if (_listViewHandler.ListView.itemsSource.Count == 0) return;

            var arrayOfElements = _listViewHandler.GetSelectedData().ToArray();

            if (!arrayOfElements.Any())
            {
                var lastData = _listViewHandler.GetLastData();

                var sceneDataPath = AssetDatabase.GetAssetPath(lastData.SceneData);
                if (lastData.IsOutsideAssetsFolder)
                {
                    Debug.LogWarning($"Suspicious deletion | Path was outside the Assets folder | {sceneDataPath}");
                }

                AssetDatabase.DeleteAsset(sceneDataPath);

                _listView.ClearSelection();
                return;
            }

            for (var i = arrayOfElements.Length - 1; i >= 0; i--)
            {
                var sceneDataPath = AssetDatabase.GetAssetPath(arrayOfElements[i].SceneData);

                if (arrayOfElements[i].IsOutsideAssetsFolder)
                {
                    Debug.LogWarning($"Suspicious deletion | Path was outside the Assets folder | {sceneDataPath}");
                }

                AssetDatabase.DeleteAsset(sceneDataPath);
            }

            _listView.ClearSelection();
        }

        private void CloseAll()
        {
            if (_quickLoadElements == null || _quickLoadElements.Elements == null) return;
            foreach (var item in _listViewHandler.Data)
            {
                var id = item.SceneData.GetInstanceID().ToString();
                SessionState.SetBool(id, false);
            }

            foreach (var item in _listViewHandler.VisualElements)
            {
                item.SetOpenState(false);
            }
        }

        private void InitializeListViewHandler(ListView listView)
        {
            _listViewHandler =
                new ListViewHandler<QuickLoadElementController, QuickLoadElementData>(listView,
                    _quickLoadElements.Elements);

            _listViewHandler.ListView.itemsRemoved += indexEnumerable =>
            {
                foreach (var index in indexEnumerable)
                {
                    var element = _quickLoadElements.Elements[index] as TagSubscriberWithTags;
                    element.RemoveFromAllTags();
                }

                ResourcesLocator.SaveQuickLoadElementsAndTagContainerDelayed();
            };
        }

        private void InitializeSearchField(VisualElement root, DropdownSearchField dropdownSearchField)
        {
            Debug.LogError("initialize search field");
            dropdownSearchField.BindToListView(_listViewHandler.ListView);

            var tagSearchIcon = Resources.Load<Texture2D>(Path.Combine("Sprites", TagIconName));
            var searchListingStrategy = new ListingStrategy(TagListingStrategyName, tagSearchIcon, ListTag);

            var hierarchyListingIcon = Resources.Load<Texture2D>(Path.Combine("Sprites", HierarchyIconName));
            _hierarchyListingStrategy =
                new ListingStrategy("loaded scenes", hierarchyListingIcon, ListHierarchy, false);

            dropdownSearchField
                .WithResizableParent(root)
                .WithDropdownBehaviour(index =>
                {
                    dropdownSearchField.HideDropdown();
                    _listViewHandler.ListView.ScrollToItem(index);
                })
                .WithCyclingListingStrategy(searchListingStrategy)
                .WithCyclingListingStrategy(_hierarchyListingStrategy);
        }

        private void OnKeyUpDeleteSelected(KeyUpEvent keyUpEvent)
        {
            if (_listView.selectedItem == null) return;
            if (!((keyUpEvent.ctrlKey || keyUpEvent.commandKey) && keyUpEvent.keyCode == KeyCode.Delete)) return;

            DeleteSelection();
        }

        private List<IListable> ListTag(string searchFieldValue, IList boundItemSource)
        {
            List<IListable> filteredList = new();

            foreach (var data in boundItemSource)
            {
                foreach (var tag in (data as QuickLoadElementData).Tags)
                {
                    if (tag.Name != searchFieldValue) continue;
                    filteredList.Add(data as IListable);
                    break;
                }
            }

            return filteredList;
        }

        private List<IListable> ListHierarchy(string searchFieldValue, IList boundItemSource)
        {
            List<IListable> filteredList = new();

            List<UnityEngine.SceneManagement.Scene> scenesInHierarchy = new();
            for (var i = 0; i < SceneManager.sceneCount; i++)
            {
                scenesInHierarchy.Add(SceneManager.GetSceneAt(i));
            }

            var itemSourceAsData = boundItemSource.Cast<QuickLoadElementData>();

            foreach (var sceneInHierarchy in scenesInHierarchy)
            {
                foreach (var quickLoadElementData in itemSourceAsData)
                {
                    var sceneAssetPath =
                        SceneDataManagerEditorApplication.GetSceneAssetPath(quickLoadElementData.SceneData);
                    if (sceneInHierarchy.path != sceneAssetPath) continue;
                    filteredList.Add(quickLoadElementData);
                    break;
                }
            }

            return filteredList;
        }
    }
}