using System.Collections;
using System.Collections.Generic;
using System.IO;
using Ludwell.Scene.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ludwell.Scene
{
    public class QuickLoadController : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<QuickLoadController, UxmlTraits>
        {
        }


        private static readonly string UxmlPath =
            Path.Combine("Uxml", nameof(SceneDataController), nameof(QuickLoadController));

        private static readonly string UssPath =
            Path.Combine("Uss", nameof(SceneDataController), nameof(QuickLoadController));

        private const string ListViewName = "scenes__list";

        private const string ButtonAddName = "add";
        private const string ButtonRemoveName = "remove";

        private const string TagListingStrategyName = "tag";
        private const string TagIconName = "icon_tag";

        private readonly QuickLoadElements _quickLoadElements;
        private ListView _listView;
        private ListViewHandler<QuickLoadElement, LoaderListViewElementData> _listViewHandler;
        private DropdownSearchField _dropdownSearchField;

        public QuickLoadController()
        {
            this.AddHierarchyFromUxml(UxmlPath);
            this.AddStyleFromUss(UssPath);

            _quickLoadElements = DataFetcher.GetQuickLoadElements();
            InitializeButtonCloseAll();
            InitializeLoaderListView();

            InitializeListViewKeyUpEvents();

            InitializeSearchField();
            InitializeAddRemoveButtons();

            Signals.Add<UISignals.RefreshQuickLoadListView>(ForceRebuildListView);
        }

        ~QuickLoadController()
        {
            Signals.Remove<UISignals.RefreshQuickLoadListView>(ForceRebuildListView);

            this.Q<Button>(ButtonAddName).clicked -= SceneDataGenerator.CreateSceneAssetAtPath;
            this.Q<Button>(ButtonRemoveName).clicked -= DeleteSceneAtPath;
        }


        private void InitializeButtonCloseAll()
        {
            var closeAllButton = this.Q<Button>();
            closeAllButton.RegisterCallback<MouseUpEvent>(_ => CloseAll());
        }

        private void CloseAll()
        {
            if (_quickLoadElements == null || _quickLoadElements.Elements == null) return;

            foreach (var element in _quickLoadElements.Elements)
            {
                element.IsOpen = false;
            }

            foreach (var item in _listView.Query<QuickLoadElement>().ToList())
            {
                item.SetFoldoutValue(false);
            }
        }

        private void InitializeLoaderListView()
        {
            _listView = this.Q<ListView>(ListViewName);
            _listViewHandler = new(_listView, _quickLoadElements.Elements);
            _listView.itemsRemoved += indexEnumerable =>
            {
                foreach (var index in indexEnumerable)
                {
                    var element = _quickLoadElements.Elements[index] as TagSubscriberWithTags;
                    element.RemoveFromAllTags();
                }

                DataFetcher.SaveEveryScriptableDelayed();
            };
        }

        private void InitializeListViewKeyUpEvents()
        {
            _listViewHandler.ListView.RegisterCallback<KeyUpEvent>(OnKeyUpDeleteSelected);
        }

        private void OnKeyUpDeleteSelected(KeyUpEvent keyUpEvent)
        {
            if (_listViewHandler.ListView.selectedItem == null) return;

            if (!((keyUpEvent.ctrlKey || keyUpEvent.commandKey) && keyUpEvent.keyCode == KeyCode.Delete)) return;

            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(_listViewHandler.GetSelectedElementData().MainScene));
        }

        private void InitializeSearchField()
        {
            _dropdownSearchField = this.Q<DropdownSearchField>();
            _dropdownSearchField.BindToListView(_listView);

            var icon = Resources.Load<Texture2D>(Path.Combine("Sprites", TagIconName));
            var searchListingStrategy = new ListingStrategy(TagListingStrategyName, icon, ListTag);

            _dropdownSearchField
                .WithResizableParent(this)
                .WithDropdownBehaviour(index =>
                {
                    _dropdownSearchField.HideDropdown();
                    _listView.ScrollToItem(index);
                })
                .WithCyclingListingStrategy(searchListingStrategy);
        }

        private List<IListable> ListTag(string searchFieldValue, IList boundItemSource)
        {
            List<IListable> filteredList = new();

            foreach (var listViewElement in boundItemSource)
            {
                foreach (var tag in (listViewElement as LoaderListViewElementData).Tags)
                {
                    if (tag.Name != searchFieldValue) continue;
                    filteredList.Add(listViewElement as IListable);
                    break;
                }
            }

            return filteredList;
        }

        private void InitializeAddRemoveButtons()
        {
            this.Q<Button>(ButtonAddName).clicked += SceneDataGenerator.CreateSceneAssetAtPath;
            this.Q<Button>(ButtonRemoveName).clicked += DeleteSceneAtPath;
        }

        private void DeleteSceneAtPath()
        {
            if (_listViewHandler.ListView.itemsSource.Count == 0) return;

            SceneData sceneData;

            var selectedIndex = _listViewHandler.ListView.selectedIndex;
            LoaderListViewElementData elementToDelete;
            if (selectedIndex == -1)
            {
                elementToDelete = _listViewHandler.ListView.itemsSource[^1] as LoaderListViewElementData;
                sceneData = elementToDelete.MainScene;
                _listViewHandler.ListView.itemsSource.Remove(sceneData);
                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(sceneData));
                return;
            }

            elementToDelete = _listViewHandler.ListView.itemsSource[selectedIndex] as LoaderListViewElementData;
            sceneData = elementToDelete.MainScene;
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(sceneData));
        }

        private void ForceRebuildListView()
        {
            _listViewHandler.ForceRebuild();
            _dropdownSearchField.RebuildActiveListing();
        }
    }
}