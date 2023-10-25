using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ludwell.Scene
{
    [Serializable]
    public class ScenesListViewController
    {
        private const string ListViewName = "scenes__list";
        private const string ToolbarSearchFieldName = "search__scene-loader-element";
        private const string LoaderSceneDataPath = "Scriptables/" + nameof(LoaderSceneData);

        private LoaderSceneData _loaderSceneData;
        private ListView _listView;
        private ListViewInitializer<LoaderListViewElement, LoaderListViewElementData> _listViewInitializer;
        private DropdownSearchField _dropdownSearchField;

        public ScenesListViewController(VisualElement queryFrom)
        {
            _loaderSceneData = Resources.Load<LoaderSceneData>(LoaderSceneDataPath);
            InitLoaderListView(queryFrom, _loaderSceneData);
            InitSearchField(queryFrom);
        }

        public void CloseAll()
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

        private void InitLoaderListView(VisualElement queryFrom, LoaderSceneData data)
        {
            _listView = queryFrom.Q<ListView>(ListViewName);
            _listViewInitializer = new(_listView, data.Elements);
        }

        private void InitSearchField(VisualElement queryFrom)
        {
            _dropdownSearchField = queryFrom.Q<DropdownSearchField>(ToolbarSearchFieldName);
            _dropdownSearchField.InitDropdownElementBehaviour(_listView, _listView.ScrollToItem);

            queryFrom.RegisterCallback<MouseUpEvent>(evt =>
            {
                if (evt.currentTarget == _dropdownSearchField) return;
                HideDropdown();
                _dropdownSearchField.ClearDropdownData();
            });
        }

        private void HideDropdown()
        {
            _dropdownSearchField.HideDropdown();
        }
    }
}