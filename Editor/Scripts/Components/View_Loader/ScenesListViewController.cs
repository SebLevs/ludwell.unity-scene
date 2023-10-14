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
            foreach (var item in _listView.Query(ListViewElementName).ToList())
            {
                (item as LoaderListViewElement)?.SetFoldoutValue(false);
            }
        }

        private LoaderListViewElement AddElement()
        {
            var element = new LoaderListViewElement
            {
                name = ListViewElementName
            };
            return element;
        }

        private void OnElementScrollIntoView(VisualElement element, int index)
        {
            var elementAsDataType = element as IBindableListViewElement<LoaderListViewElementData>;
            if (_loaderSceneData.Elements[index] == null)
            {
                _loaderSceneData.Elements[index] = new LoaderListViewElementData();
                elementAsDataType?.InitDataValues(_loaderSceneData.Elements[index]);
                elementAsDataType?.BindElementToData(_loaderSceneData.Elements[index]);
                return;
            }

            elementAsDataType?.SetElementFromData(_loaderSceneData.Elements[index]);
        }
    }
}