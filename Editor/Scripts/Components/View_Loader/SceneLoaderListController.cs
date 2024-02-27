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
    public class SceneLoaderListController : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<SceneLoaderListController, UxmlTraits>
        {
        }

        public const string TagSearchName = "Tag";

        private static readonly string UxmlPath =
            Path.Combine("Uxml", nameof(LoaderController), nameof(SceneLoaderListController));

        private static readonly string UssPath =
            Path.Combine("Uss", nameof(LoaderController), nameof(SceneLoaderListController));

        private const string ListViewName = "scenes__list";
        private static readonly string LoaderSceneDataPath = Path.Combine("Scriptables", nameof(LoaderSceneData));

        private const string ButtonCloseAll = "button__close-all";
        private const string ButtonCloseAllClicked = "button__close-all-clicked";

        private const string ButtonAddName = "add";
        private const string ButtonRemoveName = "remove";

        private const string TagIconName = "icon_tag";

        private readonly LoaderSceneData _loaderSceneData;
        private ListView _listView;
        private ListViewInitializer<LoaderListViewElement, LoaderListViewElementData> _listViewInitializer;
        private DropdownSearchField _dropdownSearchField;

        public SceneLoaderListController()
        {
            this.AddHierarchyFromUxml(UxmlPath);
            this.AddStyleFromUss(UssPath);

            _loaderSceneData = Resources.Load<LoaderSceneData>(LoaderSceneDataPath);
            InitializeButtonCloseAll();
            InitializeLoaderListView();
            InitializeSearchField();
            InitializeAddRemoveButtons();

            Signals.Add<UISignals.RefreshQuickLoadListView>(ForceRebuildListView);
        }

        ~SceneLoaderListController()
        {
            Signals.Remove<UISignals.RefreshQuickLoadListView>(ForceRebuildListView);
            
            this.Q<Button>(ButtonAddName).clicked -= SceneDataGenerator.CreateSceneAssetAtPath;
            this.Q<Button>(ButtonRemoveName).clicked -= DeleteSceneAtPath;
        }

        private void InitializeButtonCloseAll()
        {
            var closeAllButton = this.Q<ToolbarButton>();

            closeAllButton.RegisterCallback<MouseDownEvent>(_ =>
            {
                closeAllButton.RemoveFromClassList(ButtonCloseAll);
                closeAllButton.AddToClassList(ButtonCloseAllClicked);
            }, TrickleDown.TrickleDown);

            closeAllButton.RegisterCallback<MouseUpEvent>(_ =>
            {
                CloseAll();
                closeAllButton.RemoveFromClassList(ButtonCloseAllClicked);
                closeAllButton.AddToClassList(ButtonCloseAll);
            });
        }

        private void CloseAll()
        {
            if (_loaderSceneData == null || _loaderSceneData.Elements == null) return;

            foreach (var element in _loaderSceneData.Elements)
            {
                element.IsOpen = false;
            }

            foreach (var item in _listView.Query<LoaderListViewElement>().ToList())
            {
                item.SetFoldoutValue(false);
            }
        }

        private void InitializeLoaderListView()
        {
            _listView = this.Q<ListView>(ListViewName);
            _listViewInitializer = new(_listView, _loaderSceneData.Elements);
            _listView.itemsRemoved += indexEnumerable =>
            {
                foreach (var index in indexEnumerable)
                {
                    var element = _loaderSceneData.Elements[index] as TagSubscriberWithTags;
                    element.RemoveFromAllTags();
                }

                LoaderSceneDataHelper.SaveChangeDelayed();
            };
        }

        private void InitializeSearchField()
        {
            _dropdownSearchField = this.Q<DropdownSearchField>();
            _dropdownSearchField.BindToListView(_listView);

            var icon = Resources.Load<Texture2D>(Path.Combine("Sprites", TagIconName));
            var searchListingStrategy = new ListingStrategy(TagSearchName, icon, ListTag);

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
            if (_listViewInitializer.ListView.itemsSource.Count == 0) return;

            SceneData sceneData;
            
            var selectedIndex = _listViewInitializer.ListView.selectedIndex;
            LoaderListViewElementData elementToDelete;
            if (selectedIndex == -1)
            {
                elementToDelete = _listViewInitializer.ListView.itemsSource[^1] as LoaderListViewElementData;
                sceneData = elementToDelete.MainScene;
                _listViewInitializer.ListView.itemsSource.Remove(sceneData);
                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(sceneData));
                return;
            }
            
            elementToDelete = _listViewInitializer.ListView.itemsSource[selectedIndex] as LoaderListViewElementData;
            sceneData = elementToDelete.MainScene;
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(sceneData));
        }

        private void ForceRebuildListView()
        {
            _listViewInitializer.ForceRebuild();
            _dropdownSearchField.RebuildListing();
        }
    }
}