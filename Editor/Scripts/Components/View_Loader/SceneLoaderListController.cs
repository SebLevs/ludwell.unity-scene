using System.Collections;
using System.Collections.Generic;
using Ludwell.Scene.Editor;
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

        private const string UxmlPath = "Uxml/" + nameof(LoaderController) + "/" + nameof(SceneLoaderListController);
        private const string UssPath = "Uss/" + nameof(LoaderController) + "/" + nameof(SceneLoaderListController);

        private const string ListViewName = "scenes__list";
        private const string LoaderSceneDataPath = "Scriptables/" + nameof(LoaderSceneData);

        private const string ButtonCloseAll = "button__close-all";
        private const string ButtonCloseAllClicked = "button__close-all-clicked";

        private const string TagIconName = "icon_tag";

        private LoaderSceneData _loaderSceneData;
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
            _listView.itemsRemoved += _ => LoaderSceneDataHelper.SaveChangeDelayed();
        }

        private void InitializeSearchField()
        {
            _dropdownSearchField = this.Q<DropdownSearchField>();
            _dropdownSearchField.BindToListView(_listView);

            var icon = Resources.Load<Texture2D>("Sprites/" + TagIconName);
            var searchListingStrategy = new ListingStrategy(icon, ListTag);

            _dropdownSearchField
                .WithResizableParent(this)
                .WithDropdownBehaviour(index =>
                {
                    _dropdownSearchField.HideDropdown();
                    _listView.ScrollToItem(index);
                })
                .WithCyclingListingStrategy(searchListingStrategy);
        }

        private List<ISearchFieldListable> ListTag(string searchFieldValue, IList boundItemSource)
        {
            List<ISearchFieldListable> filteredList = new();

            foreach (var listViewElement in boundItemSource)
            {
                foreach (var tag in (listViewElement as LoaderListViewElementData).Tags)
                {
                    if (tag != searchFieldValue) continue;
                    filteredList.Add(listViewElement as LoaderListViewElementData);
                    break;
                }
            }

            return filteredList;
        }
    }
}