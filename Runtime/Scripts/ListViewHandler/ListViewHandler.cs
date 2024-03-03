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
        public ListView ListView { get; }

        public ListViewHandler(ListView listView, List<TData> data)
        {
            ListView = listView;
            ListView.itemsSource = data;
            ListView.makeItem = CreateElement;
            ListView.bindItem = OnElementScrollIntoView;
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
            return new TVisualElement();
        }

        private void OnElementScrollIntoView(VisualElement element, int index)
        {
            var elementAsDataType = element as IListViewVisualElement<TData>;

            ListView.itemsSource[index] ??= new TData();

            elementAsDataType?.CacheData((TData)ListView.itemsSource[index]);
            elementAsDataType?.BindElementToCachedData();
            elementAsDataType?.SetElementFromCachedData();
        }
    }
}