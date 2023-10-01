using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ludwell.Scene
{
    public class ScenesListView
    {
        private const string ListViewName = "scenes__list";
        private ListView _listView;
        
        public List<SceneLoaderElement> ItemsSource { get; private set; } = new();

        public void Init(VisualElement queryFrom)
        {
            _listView = queryFrom.Q<ListView>(ListViewName);
            _listView.itemsSource = ItemsSource;
            _listView.makeItem = OnMakeItem;
            _listView.bindItem = OnBindItem;
            _listView.unbindItem = OnUnbindItem;
        }

        public void CloseAll()
        {
            foreach (var item in ItemsSource)
            {
                Debug.LogError(item);
                item.FoldoutElement.value = false;
            }
        }

        private SceneLoaderElement OnMakeItem()
        {
            Debug.LogError(nameof(OnMakeItem));
            SceneLoaderElement element = new SceneLoaderElement();
            return element;
        }

        private void OnBindItem(VisualElement element, int index)
        {
            Debug.LogError(nameof(OnBindItem));
            Debug.LogError(element);
        }
        
        private void OnUnbindItem(VisualElement element, int index)
        {
            Debug.LogError(nameof(OnUnbindItem));
        }
    }
}