using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ludwell.Scene
{
    public class SceneLoaderListController: VisualElement
    {
        public new class UxmlFactory : UxmlFactory<SceneLoaderListController, UxmlTraits> { }

        private const string UxmlPath = "Uxml/ViewLoader/scene-loader-list";
        private const string UssPath = "Uss/ViewLoader/scene-loader-list";
        
        private const string ListViewName = "scenes__list";
        private const string LoaderSceneDataPath = "Scriptables/" + nameof(LoaderSceneData);
        
        private LoaderSceneData _loaderSceneData;
        private ListView _listView;
        private ListViewInitializer<LoaderListViewElement, LoaderListViewElementData> _listViewInitializer;
        private DropdownSearchField _dropdownSearchField;

        public SceneLoaderListController()
        {
            this.SetHierarchyFromUxml(UxmlPath);
            this.AddStyleFromUss(UssPath);
            
            _loaderSceneData = Resources.Load<LoaderSceneData>(LoaderSceneDataPath);
            InitButtonCloseAll();
            InitLoaderListView();
            InitSearchField();
        }

        private void InitButtonCloseAll()
        {
            this.Q<ToolbarButton>().clicked += CloseAll;
        }

        private void CloseAll()
        {
            foreach (var element in _loaderSceneData.Elements)
            {
                element.IsOpen = false;
            }

            foreach (var item in _listView.Query<LoaderListViewElement>().ToList())
            {
                item.SetFoldoutValue(false);
            }
        }

        private void InitLoaderListView()
        {
            _listView = this.Q<ListView>(ListViewName);
            _listViewInitializer = new(_listView, _loaderSceneData.Elements);
        }

        private void InitSearchField()
        {
            _dropdownSearchField = this.Q<DropdownSearchField>();
            _dropdownSearchField.InitDropdownElementBehaviour(_listView, (index) =>
            {
                _dropdownSearchField.HideDropdown();
                _listView.ScrollToItem(index);
            });

            _dropdownSearchField.InitMouseEvents(this.Root());
        }
    }
}