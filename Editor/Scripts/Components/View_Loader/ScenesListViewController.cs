using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ludwell.Scene
{
    [Serializable]
    public class ScenesListViewController
    {
        private const string ListViewName = "scenes__list";
        private const string LoaderSceneDataPath = "Scriptables/" + nameof(LoaderSceneData);
        private ListView _listView;

        private LoaderSceneData _loaderSceneData;
        
        public ScenesListViewController(VisualElement queryFrom)
        {
            _loaderSceneData = Resources.Load<LoaderSceneData>(LoaderSceneDataPath);
            _listView = queryFrom.Q<ListView>(ListViewName);
            _listView.itemsSource = _loaderSceneData.Elements;
            _listView.makeItem = AddElement;
            _listView.bindItem = OnElementScrollIntoView;
        }
        
        public void CloseAll()
        {
            foreach (var item in _listView.Children())
            {
                Debug.LogError(item);
                (item as Foldout).value = false;
            }
        }
        
        private LoaderListViewElement AddElement()
        {
            return new LoaderListViewElement();
        }

        private void OnElementScrollIntoView(VisualElement element, int index)
        {
            if (_loaderSceneData.Elements[index] == null)
            {
                _loaderSceneData.Elements[index] = new LoaderListViewElementData();
                (element as LoaderListViewElement)?.BindElementToData(_loaderSceneData.Elements[index]);
                return;
            }

            (element as LoaderListViewElement)?.SetElementFromData(_loaderSceneData.Elements[index]);
        }
    }
}