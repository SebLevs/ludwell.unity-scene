using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

namespace Ludwell.Scene.Editor
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

        public IEnumerable<TVisualElement> VisualElements => _visibleElements.Values;

        public IEnumerable<TData> Data => (List<TData>)ListView.itemsSource;

        public ListViewHandler(ListView listView, List<TData> data)
        {
            ListView = listView;
            ListView.itemsSource = data;
            ListView.makeItem = CreateElement;
            ListView.bindItem = OnElementScrollIntoView;
            ListView.unbindItem = OnElementScrollOutOfView;
            ListView.itemsAdded += _ => ForceRebuild();
            ListView.itemsRemoved += _ => ForceRebuild();
        }

        /// <summary> Workaround for a ListView visual issue concerning dynamically sized element rendering. </summary>
        public void ForceRebuild()
        {
            ListView.Rebuild();
        }

        public TData GetFirstSelectedData()
        {
            return (TData)ListView.selectedItem;
        }

        public IEnumerable<TData> GetSelectedData()
        {
            return ListView.selectedItems.Cast<TData>();
        }

        public TData GetLastData()
        {
            var lastIndex = ListView.itemsSource.Count - 1;
            return (TData)ListView.itemsSource[lastIndex];
        }

        public IEnumerable<TVisualElement> GetSelectedVisualElements()
        {
            List<TVisualElement> elements = new();
            foreach (var selectedIndices in ListView.selectedIndices)
            {
                if (!_visibleElements.TryGetValue(selectedIndices, out var value)) continue;
                elements.Add(value);
            }

            return elements;
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