using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ludwell.Architecture;
using Ludwell.MoreInformation.Editor;
using Ludwell.UIToolkitElements;
using Ludwell.UIToolkitElements.Editor;
using Ludwell.UIToolkitUtilities;
using Ludwell.UIToolkitUtilities.Editor;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using SceneRuntime = UnityEngine.SceneManagement.Scene;

namespace Ludwell.SceneManagerToolkit.Editor
{
    internal class SceneElementsViewArgs : ViewArgs
    {
        public SceneElementsViewArgs(SceneAssetDataBinder binder)
        {
            Binder = binder;
        }

        public SceneAssetDataBinder Binder { get; }
    }

    internal class SceneElementsController : AViewable
    {
        private const string TagListingStrategyName = "tag";
        private const string TagIconName = "icon_tag";
        private const string HierarchyIconName = "icon_hierarchy";

        private readonly SceneAssetDataBinders _sceneAssetDataBinders;

        private readonly VisualElement _root;
        private readonly SceneElementsView _view;
        private readonly ListView _listView;
        private readonly DropdownSearchField _dropdownSearchField;

        private readonly MoreInformationController _moreInformationController;

        private readonly SceneElementsListViewRefresh _sceneElementsListViewRefresh;

        private ListViewHandler<SceneElementController, SceneAssetDataBinder> _listViewHandler;

        private ListingStrategy _hierarchyListingStrategy;

        public SceneElementsController(VisualElement parent) : base(parent)
        {
            _root = parent.Q(nameof(SceneElementsView));
            _view = new SceneElementsView(_root);
            _view.CloseAllButton.clicked += CloseAll;
            _view.AddButton.clicked += DataSolver.CreateSceneAssetAtPath;
            _view.RemoveButton.clicked += DeleteSelection;

            _listView = _root.Q<ListView>();
            _dropdownSearchField = _root.Q<DropdownSearchField>();

            _moreInformationController = new MoreInformationController(_root);
            _view.MoreInformationButton.clicked += _moreInformationController.Show;
            _moreInformationController.Hide();

            _sceneAssetDataBinders = SceneAssetDataBinders.Instance;

            InitializeListViewHandler(_root.Q<ListView>());
            InitializeSearchField(_root, _root.Q<DropdownSearchField>());
            _listView.RegisterCallback<KeyUpEvent>(OnKeyUpDeleteSelected);

            _sceneElementsListViewRefresh = new SceneElementsListViewRefresh(_root);

            Services.Add<SceneElementsController>(this);

            InitializeContextualMenuManipulator();

            OnShow = AddRefreshViewSignal;
            OnHide = RemoveRefreshViewSignal;
        }

        public void Dispose()
        {
            Services.Remove<SceneElementsController>();

            _view.CloseAllButton.clicked -= CloseAll;
            _view.AddButton.clicked -= DataSolver.CreateSceneAssetAtPath;
            _view.RemoveButton.clicked -= DeleteSelection;
            _view.MoreInformationButton.clicked -= _moreInformationController.Show;

            _listView.ClearSelection();
            _listView.UnregisterCallback<KeyUpEvent>(OnKeyUpDeleteSelected);

            _moreInformationController.Dispose();
        }

        public void ScrollToItemIndexThenFocusTextField(int index)
        {
            _root.Root().schedule.Execute(() =>
            {
                _listViewHandler.ListView.ScrollToItem(index);
                _listViewHandler.ListView.SetSelection(index);

                var itemAtIndex = _listViewHandler.ListView.itemsSource[index];
                var dataName = ((SceneAssetDataBinder)itemAtIndex).Data.Name;
                var controller =
                    _listViewHandler.GetFirstVisualElementWhere(element => element.IsTextFieldValue(dataName));
                controller.FocusTextField();
            });
        }

        public void ScrollToItemIndex(int index)
        {
            _root.Root().schedule.Execute(() =>
            {
                _listViewHandler.ListView.ScrollToItem(index);
                _listViewHandler.ListView.SetSelection(index);
                var itemAtIndex = _listViewHandler.ListView.itemsSource[index];
                var dataName = ((SceneAssetDataBinder)itemAtIndex).Data.Name;
                var controller =
                    _listViewHandler.GetFirstVisualElementWhere(element => element.IsTextFieldValue(dataName));
                controller.Focus();
            });
        }

        public void RebuildActiveListing()
        {
            if (_dropdownSearchField.RebuildActiveListing()) return;
            _listViewHandler.ForceRebuild();
        }

        protected override void Show(ViewArgs args)
        {
            _view.Show();
            _sceneElementsListViewRefresh.StartOrRefreshDelayedRebuild();

            var sceneElementsViewArgs = args as SceneElementsViewArgs;
            if (sceneElementsViewArgs?.Binder == null) return;
            _root.schedule.Execute(() =>
            {
                var index = ResourcesLocator.GetSceneAssetDataBinders().IndexOf(sceneElementsViewArgs.Binder);
                ScrollToItemIndex(index);
            });
        }

        protected override void Hide()
        {
            _view.Hide();
        }

        private void AddRefreshViewSignal()
        {
            Signals.Add<UISignals.RefreshView>(RebuildActiveListing);
        }

        private void RemoveRefreshViewSignal()
        {
            Signals.Remove<UISignals.RefreshView>(RebuildActiveListing);
        }

        private SceneElementController[] GetSceneElementControllers()
        {
            var enumerableSelection = _listViewHandler.GetSelectedVisualElements();
            return enumerableSelection as SceneElementController[] ?? enumerableSelection.ToArray();
        }

        private List<SceneElementController> GetSceneElementControllersWithoutActiveScene()
        {
            var enumerableSelection = _listViewHandler.GetSelectedVisualElements();
            return enumerableSelection as List<SceneElementController> ?? enumerableSelection.ToList();
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

        private void InitializeContextualMenuManipulator()
        {
            _listViewHandler.ListView.AddManipulator(new ContextualMenuManipulator(context =>
            {
                var controllers = GetSceneElementControllersInHierarchy();

                var defaultValidation = _listViewHandler.ListView.selectedIndices.Any();
                var onlyOneSceneInHierarchy = SceneManager.sceneCount == 1;
                var selectionNotInHierarchy = controllers.Count == 0;

                var defaultStatus =
                    !defaultValidation
                        ? DropdownMenuAction.Status.Disabled
                        : DropdownMenuAction.Status.Normal;

                var isCount = controllers.Count == SceneManager.sceneCount;
                var destructiveStatus =
                    onlyOneSceneInHierarchy || !defaultValidation || selectionNotInHierarchy || isCount
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

            var modifiedScenes = controllers.Where(controller => controller.Scene.isDirty).ToList();

            if (modifiedScenes.Count > 0)
            {
                var namesAsStrings = "";
                foreach (var controller in modifiedScenes)
                {
                    namesAsStrings += controller.Scene.name + "\n";
                }

                if (!EditorUtility.DisplayDialog(
                        "Scene(s) Have Been Modified",
                        $"Do you want to save the changes you made in the scenes:\n{namesAsStrings}",
                        "Save and Unload", "Cancel"))
                {
                    return;
                }

                foreach (var controller in modifiedScenes)
                {
                    var sceneReference = controller.Scene;
                    if (sceneReference.isDirty) EditorSceneManager.SaveScene(sceneReference);
                }
            }

            foreach (var controller in controllers)
            {
                controller.UnloadSceneAdditive();
            }
        }

        private void OpenSelectionAdditive(DropdownMenuAction _)
        {
            var controllers = GetSceneElementControllersWithoutActiveScene();
            foreach (var controller in controllers)
            {
                controller.OpenSceneAdditive();
            }
        }

        private void RemoveSelectionAdditive(DropdownMenuAction _)
        {
            var controllers = GetSceneElementControllersInHierarchy();
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

        /// <summary> If no item is selected, deletes the last item. </summary>
        private void DeleteSelection()
        {
            if (_listViewHandler.ListView.itemsSource.Count == 0) return;

            var arrayOfElements = _listViewHandler.GetSelectedData().ToArray();

            if (!arrayOfElements.Any())
            {
                var lastData = _listViewHandler.GetLastData();

                if (EditorSceneManagerHelper.IsPathOutsideAssets(lastData.Data.Path))
                {
                    Debug.LogWarning(
                        $"Suspicious deletion | Path was outside the Assets folder | {lastData.Data.Path}");
                }

                AssetDatabase.DeleteAsset(lastData.Data.Path);

                _listView.ClearSelection();
                return;
            }

            for (var i = arrayOfElements.Length - 1; i >= 0; i--)
            {
                var path = arrayOfElements[i].Data.Path;

                if (EditorSceneManagerHelper.IsPathOutsideAssets(arrayOfElements[i].Data.Path))
                {
                    Debug.LogWarning($"Suspicious deletion | Path was outside the Assets folder | {path}");
                }

                AssetDatabase.DeleteAsset(path);
            }

            _listView.ClearSelection();
        }

        private void CloseAll()
        {
            if (_sceneAssetDataBinders == null || _sceneAssetDataBinders.Elements == null) return;
            foreach (var item in _listViewHandler.Data)
            {
                var id = item.Data.GUID;
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
                new ListViewHandler<SceneElementController, SceneAssetDataBinder>(listView,
                    _sceneAssetDataBinders.Elements);
        }

        private void InitializeSearchField(VisualElement root, DropdownSearchField dropdownSearchField)
        {
            dropdownSearchField.BindToListView(_listViewHandler.ListView);

            var tagSearchIcon =
                Resources.Load<Texture2D>(Path.Combine("Sprites", nameof(DropdownSearchField), TagIconName));
            var searchListingStrategy = new ListingStrategy(TagListingStrategyName, tagSearchIcon, ListTag);

            var hierarchyListingIcon =
                Resources.Load<Texture2D>(Path.Combine("Sprites", nameof(DropdownSearchField), HierarchyIconName));
            _hierarchyListingStrategy =
                new ListingStrategy("loaded scenes", hierarchyListingIcon, ListHierarchy, true);

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
                foreach (var tag in (data as SceneAssetDataBinder).Tags)
                {
                    if (!string.Equals(tag.ID, searchFieldValue, StringComparison.InvariantCultureIgnoreCase)) continue;
                    filteredList.Add(data as IListable);
                    break;
                }
            }

            return filteredList;
        }

        private List<IListable> ListHierarchy(string searchFieldValue, IList boundItemSource)
        {
            List<IListable> filteredList = new();

            List<SceneRuntime> scenesInHierarchy = new();
            for (var i = 0; i < SceneManager.sceneCount; i++)
            {
                scenesInHierarchy.Add(SceneManager.GetSceneAt(i));
            }

            var itemSourceAsData = boundItemSource.Cast<SceneAssetDataBinder>();

            foreach (var sceneInHierarchy in scenesInHierarchy)
            {
                foreach (var sceneAssetDataBinder in itemSourceAsData)
                {
                    if (!sceneInHierarchy.path.Contains(sceneAssetDataBinder.Data.Path)) continue;
                    if (string.IsNullOrEmpty(searchFieldValue) || string.IsNullOrWhiteSpace(searchFieldValue))
                    {
                        filteredList.Add(sceneAssetDataBinder);
                        break;
                    }

                    if (!sceneAssetDataBinder.Data.Name.Contains(searchFieldValue)) continue;
                    filteredList.Add(sceneAssetDataBinder);
                    break;
                }
            }

            return filteredList;
        }
    }
}
