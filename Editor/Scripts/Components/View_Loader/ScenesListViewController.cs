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
            InitLoaderListView(queryFrom, _loaderSceneData);
        }

        private const string ListViewName = "scenes__list";
        private const string LoaderSceneDataPath = "Scriptables/" + nameof(LoaderSceneData);

        private LoaderSceneData _loaderSceneData;
        private ListView _listView;
        private ListViewInitializer<LoaderListViewElement, LoaderListViewElementData> _listViewInitializer;

        private void InitLoaderListView(VisualElement queryFrom, LoaderSceneData data)
        {
            _listView = queryFrom.Q<ListView>(ListViewName);
            _listViewInitializer = new (_listView, data.Elements);
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
    }
}