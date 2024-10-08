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

        private readonly SceneElementsListViewRefresh _sceneElementsListViewRefresh;

        private ListViewHandler<SceneElementController, SceneAssetDataBinder> _listViewHandler;

        private readonly ListFooterController _listFooterController;

        private readonly MoreInformationController _moreInformationController;

        private ListingStrategy _hierarchyListingStrategy;

        private SceneElementsContextualMenu _contextualMenu;

        public SceneElementsController(VisualElement parent) : base(parent)
        {
            _root = parent.Q(nameof(SceneElementsView));
            _view = new SceneElementsView(_root);
            _view.CloseAllButton.clicked += CloseAll;

            _listView = _root.Q<ListView>();
            _dropdownSearchField = _root.Q<DropdownSearchField>();

            _listFooterController = new ListFooterController(parent);
            _listFooterController.SubscribeToAddButtonClicked(DataSolver.CreateSceneAssetAtPath);
            _listFooterController.SubscribeToRemoveButtonClicked(DeleteSelection);

            _moreInformationController = new MoreInformationController(_root);
            _view.MoreInformationButton.clicked += _moreInformationController.Show;
            _moreInformationController.Hide();

            _sceneAssetDataBinders = SceneAssetDataBinders.Instance;

            InitializeListViewHandler(_root.Q<ListView>());
            InitializeSearchField(_root, _root.Q<DropdownSearchField>());
            _listView.RegisterCallback<KeyUpEvent>(OnKeyUpDeleteSelected);

            _sceneElementsListViewRefresh = new SceneElementsListViewRefresh(_root);

            Services.Add<SceneElementsController>(this);

            OnShow = AddRefreshViewSignal;
            OnHide = RemoveRefreshViewSignal;

            _contextualMenu = new SceneElementsContextualMenu(_listViewHandler);
        }

        public void Dispose()
        {
            Services.Remove<SceneElementsController>();

            _view.CloseAllButton.clicked -= CloseAll;

            _listFooterController.UnsubscribeFromAddButtonClicked(DataSolver.CreateSceneAssetAtPath);
            _listFooterController.UnsubscribeFromRemoveButtonClicked(DeleteSelection);

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
