using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using SceneRuntime = UnityEngine.SceneManagement.Scene;

namespace Ludwell.Scene.Editor
{
    public class QuickLoadController
    {
        private const string TagListingStrategyName = "tag";
        private const string TagIconName = "icon_tag";
        private const string HierarchyIconName = "icon_hierarchy";

        private readonly QuickLoadElements _quickLoadElements;
        private ListViewHandler<QuickLoadElementController, QuickLoadElementData> _listViewHandler;

        private readonly VisualElement _root;
        private readonly QuickLoadView _view;
        private readonly ListView _listView;
        private readonly DropdownSearchField _dropdownSearchField;

        private readonly MoreInformationController _moreInformationController;
        
        private ListingStrategy _hierarchyListingStrategy;

        public QuickLoadController(VisualElement parent)
        {
            _root = parent.Q(nameof(QuickLoadView));
            _view = new QuickLoadView(_root);
            _view.CloseAllButton.clicked += CloseAll;
            _view.AddButton.clicked += SceneDataGenerator.CreateSceneAssetAtPath;
            _view.RemoveButton.clicked += DeleteSelection;

            _listView = _root.Q<ListView>();
            _dropdownSearchField = _root.Q<DropdownSearchField>();
            
            _moreInformationController = new MoreInformationController(_root);
            _view.MoreInformationButton.clicked += _moreInformationController.Show;

            _quickLoadElements = ResourcesLocator.GetQuickLoadElements();

            InitializeListViewHandler(_root.Q<ListView>());
            InitializeSearchField(_root, _root.Q<DropdownSearchField>());
            _listView.RegisterCallback<KeyUpEvent>(OnKeyUpDeleteSelected);

            Services.Add<QuickLoadController>(this);

            InitializeContextMenuManipulator();

            _root.RegisterCallback<DetachFromPanelEvent>(Dispose);
        }

        public void ScrollToItemIndex(int index)
        {
            var focusController = _root.focusController;
            var focusedElement = focusController?.focusedElement;
            focusedElement?.Blur();

            _listViewHandler.ListView.ScrollToItem(index);
            _listViewHandler.ListView.SetSelection(index);
            _listViewHandler.GetVisualElementAt(index)?.FocusTextField();
        }

        public void RebuildActiveListing()
        {
            if (_dropdownSearchField.RebuildActiveListing()) return;
            _listViewHandler.ForceRebuild();
        }

        private List<QuickLoadElementController> GetVisualElementsWithoutActiveScene()
        {
            var enumerableSelection = _listViewHandler.GetSelectedVisualElements();
            return enumerableSelection as List<QuickLoadElementController> ?? enumerableSelection.ToList();
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
            if (quickLoadElementControllers.Count == 1) return;
            foreach (var quickLoadElementController in quickLoadElementControllers)
            {
                if (SceneManager.sceneCount == 1) return;
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
        }
        

        private void InitializeSearchField(VisualElement root, DropdownSearchField dropdownSearchField)
        {
            dropdownSearchField.BindToListView(_listViewHandler.ListView);

            var tagSearchIcon = Resources.Load<Texture2D>(Path.Combine("Sprites", TagIconName));
            var searchListingStrategy = new ListingStrategy(TagListingStrategyName, tagSearchIcon, ListTag);

            var hierarchyListingIcon = Resources.Load<Texture2D>(Path.Combine("Sprites", HierarchyIconName));
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
                foreach (var tag in (data as QuickLoadElementData).Tags)
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

            var itemSourceAsData = boundItemSource.Cast<QuickLoadElementData>();

            foreach (var sceneInHierarchy in scenesInHierarchy)
            {
                foreach (var quickLoadElementData in itemSourceAsData)
                {
                    var sceneAssetPath =
                        SceneDataManagerEditorApplication.GetSceneAssetPath(quickLoadElementData.SceneData);
                    if (!sceneInHierarchy.path.Contains(sceneAssetPath)) continue;
                    if (string.IsNullOrEmpty(searchFieldValue) || string.IsNullOrWhiteSpace(searchFieldValue))
                    {
                        filteredList.Add(quickLoadElementData);
                        break;
                    }

                    if (!quickLoadElementData.SceneData.Name.Contains(searchFieldValue)) continue;
                    filteredList.Add(quickLoadElementData);
                    break;
                }
            }

            return filteredList;
        }

        private void Dispose(DetachFromPanelEvent _)
        {
            _root.UnregisterCallback<DetachFromPanelEvent>(Dispose);
            _view.CloseAllButton.clicked -= CloseAll;
            _view.AddButton.clicked -= SceneDataGenerator.CreateSceneAssetAtPath;
            _view.RemoveButton.clicked -= DeleteSelection;
            _view.MoreInformationButton.clicked -= _moreInformationController.Show;

            _listView.UnregisterCallback<KeyUpEvent>(OnKeyUpDeleteSelected);
            
            _moreInformationController.Dispose();
        }
    }
}