using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ludwell.Scene
{
    [Serializable]
    public class ScenesListViewController
    {
        public ScenesListViewController(VisualElement queryFrom)
        {
            _loaderSceneData = Resources.Load<LoaderSceneData>(LoaderSceneDataPath);
            InitLoaderListView(queryFrom);
        }

        private const string ListViewName = "scenes__list";
        public const string ListViewElementName = "loader-list-view-element";
        private const string LoaderSceneDataPath = "Scriptables/" + nameof(LoaderSceneData);

        private ListView _listView;
        private LoaderSceneData _loaderSceneData;

        private void InitLoaderListView(VisualElement queryFrom)
        {
            _listView = queryFrom.Q<ListView>(ListViewName);
            _listView.itemsSource = _loaderSceneData.Elements;
            _listView.makeItem = AddElement;
            _listView.bindItem = OnElementScrollIntoView;
        }

        public void CloseAll()
        {
            foreach (var item in _listView.Query<LoaderListViewElement>().ToList())
            {
                item.SetFoldoutValue(false);
            }

            foreach (var element in _loaderSceneData.Elements)
            {
                element.IsOpen = false;
            }
        }

        private LoaderListViewElement AddElement()
        {
            return new LoaderListViewElement();
        }

        private void OnElementScrollIntoView(VisualElement element, int index)
        {
            var elementAsDataType = element as IBindableListViewElement<LoaderListViewElementData>;

            _loaderSceneData.Elements[index] ??= new();

            elementAsDataType?.CacheData(_loaderSceneData.Elements[index]);
            elementAsDataType?.BindElementToCachedData();
            elementAsDataType?.SetElementFromCachedData();
        }
    }
}