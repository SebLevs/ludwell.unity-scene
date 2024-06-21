using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Ludwell.Scene
{
    /// <typeparam name="TVisualElement">
    /// The VisualElement must be a IListViewVisualElement, as it's methods are used for binding and setting data.
    /// </typeparam>
    /// <typeparam name="TData">
    /// The data type needed for the IList element bound to the ListView.
    /// </typeparam>
    public class ListViewHandler<TVisualElement, TData>
        where TVisualElement : VisualElement, IListViewVisualElement<TData>, new()
        where TData : new()
    {
        public Action<TVisualElement> OnItemMade;

        private readonly Dictionary<int, TVisualElement> _visibleElements = new();

        public ListView ListView { get; }
        public IEnumerable<TVisualElement> GetVisualElements() => _visibleElements.Values;

        public ListViewHandler(ListView listView, List<TData> data)
        {
            ListView = listView;
            ListView.itemsSource = data;
            ListView.makeItem = CreateElement;
            ListView.bindItem = OnElementScrollIntoView;
            ListView.unbindItem = OnElementScrollOutOfView;
            ListView.itemsAdded += _ => ForceRebuild();
            ListView.itemsRemoved += _ => ForceRebuild();

            // todo: replace workaround for the ListView visual issue concerning dynamically sized element rendering.
            listView.RegisterCallback<GeometryChangedEvent>(_ => { ListView.Rebuild(); });
        }

        /// <summary> Workaround for a ListView visual issue concerning dynamically sized element rendering. </summary>
        public void ForceRebuild()
        {
            ListView.Rebuild();
        }


        public TData GetSelectedElementData()
        {
            return (TData)ListView.selectedItem;
        }

        public TData GetLastData()
        {
            var lastIndex = ListView.itemsSource.Count - 1;
            return (TData)ListView.itemsSource[lastIndex];
        }

        /// <returns>Note that the provided VisualElement might not be in view.</returns>
        public TVisualElement GetVisualElementAt(int index)
        {
            _visibleElements.TryGetValue(index, out var value);
            return value;
        }

        public void RemoveSelectedElement()
        {
            if (ListView.selectedItem == null) return;
            RemoveElement((TData)ListView.selectedItem);
        }

        public void RemoveElement(TData element)
        {
            ListView.itemsSource.Remove(element);
            ForceRebuild();
        }

        private TVisualElement CreateElement()
        {
            var visualElement = new TVisualElement();
            OnItemMade?.Invoke(visualElement);
            return visualElement;
        }

        private void OnElementScrollIntoView(VisualElement element, int index)
        {
            var elementAsDataType = element as TVisualElement;
            _visibleElements.Add(index, elementAsDataType);

            ListView.itemsSource[index] ??= new TData();

            elementAsDataType?.CacheData((TData)ListView.itemsSource[index]);
            elementAsDataType?.BindElementToCachedData();
            elementAsDataType?.SetElementFromCachedData();
        }

        private void OnElementScrollOutOfView(VisualElement element, int index)
        {
            _visibleElements.Remove(index);
        }
    }
}