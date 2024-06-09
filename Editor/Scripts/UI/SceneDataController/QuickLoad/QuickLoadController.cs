using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ludwell.Scene.Editor
{
    public class QuickLoadController
    {
        private const string TagListingStrategyName = "tag";
        private const string TagIconName = "icon_tag";

        private readonly QuickLoadElements _quickLoadElements;
        private ListViewHandler<QuickLoadElementView, QuickLoadElementData> _listViewHandler;

        private readonly QuickLoadView _view;
        private ListView _listView;
        private DropdownSearchField _dropdownSearchField;

        public QuickLoadController(VisualElement parent)
        {
            var root = parent.Q(nameof(QuickLoadView));
            _view = new QuickLoadView(root, CloseAll, SceneDataGenerator.CreateSceneAssetAtPath, DeleteSelection);

            _listView = root.Q<ListView>();
            _dropdownSearchField = root.Q<DropdownSearchField>();

            _quickLoadElements = DataFetcher.GetQuickLoadElements();

            InitializeListViewHandler(root.Q<ListView>());
            InitializeSearchField(root, root.Q<DropdownSearchField>());
            InitializeListViewKeyUpEvents();
        }

        /// <summary> If no item is selected, deletes the last item. </summary>
        public void DeleteSelection()
        {
            if (_listViewHandler.ListView.itemsSource.Count == 0) return;

            var selectedElementData = _listViewHandler.GetSelectedElementData() != null
                ? _listViewHandler.GetSelectedElementData()
                : _listViewHandler.GetLastData();

            var sceneDataPath = AssetDatabase.GetAssetPath(selectedElementData.SceneData);

            AssetDatabase.DeleteAsset(sceneDataPath);

            if (selectedElementData.IsOutsideAssetsFolder)
            {
                Debug.LogWarning($"Suspicious delete action | Path was outside the Assets folder | {sceneDataPath}");
            }

            _listViewHandler.ForceRebuild();
        }

        private void CloseAll()
        {
            if (_quickLoadElements == null || _quickLoadElements.Elements == null) return;

            foreach (var element in _quickLoadElements.Elements)
            {
                element.IsOpen = false;
            }

            foreach (var item in _listViewHandler.ListView.Query<QuickLoadElementView>().ToList())
            {
                item.SetIsOpen(false);
            }

            DataFetcher.SaveQuickLoadElementsDelayed();
        }

        private void InitializeListViewHandler(ListView listView)
        {
            _listViewHandler = new(listView, _quickLoadElements.Elements);

            _listViewHandler.ListView.itemsRemoved += indexEnumerable =>
            {
                foreach (var index in indexEnumerable)
                {
                    var element = _quickLoadElements.Elements[index] as TagSubscriberWithTags;
                    element.RemoveFromAllTags();
                }

                DataFetcher.SaveQuickLoadElementsAndTagContainerDelayed();
            };
        }

        private void InitializeSearchField(VisualElement root, DropdownSearchField dropdownSearchField)
        {
            dropdownSearchField.BindToListView(_listViewHandler.ListView);

            var icon = Resources.Load<Texture2D>(Path.Combine("Sprites", TagIconName));
            var searchListingStrategy = new ListingStrategy(TagListingStrategyName, icon, ListTag);

            dropdownSearchField
                .WithResizableParent(root)
                .WithDropdownBehaviour(index =>
                {
                    dropdownSearchField.HideDropdown();
                    _listViewHandler.ListView.ScrollToItem(index);
                })
                .WithCyclingListingStrategy(searchListingStrategy);
        }

        private void InitializeListViewKeyUpEvents()
        {
            _listView.RegisterCallback<KeyUpEvent>(OnKeyUpDeleteSelected);
        }

        private void OnKeyUpDeleteSelected(KeyUpEvent keyUpEvent)
        {
            if (_listView.selectedItem == null) return;
            if (!((keyUpEvent.ctrlKey || keyUpEvent.commandKey) && keyUpEvent.keyCode == KeyCode.Delete)) return;

            DeleteSelection();
        }

        public void ForceRebuildListView()
        {
            _listViewHandler.ForceRebuild();
            _dropdownSearchField.RebuildActiveListing();
        }

        private List<IListable> ListTag(string searchFieldValue, IList boundItemSource)
        {
            List<IListable> filteredList = new();

            foreach (var listViewElement in boundItemSource)
            {
                foreach (var tag in (listViewElement as QuickLoadElementData).Tags)
                {
                    if (tag.Name != searchFieldValue) continue;
                    filteredList.Add(listViewElement as IListable);
                    break;
                }
            }

            return filteredList;
        }
    }
}