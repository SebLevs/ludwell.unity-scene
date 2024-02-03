using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Ludwell.Scene
{
    public class ListViewInitializer<TVisualElement, TListElement>
        where TVisualElement : VisualElement, new()
        where TListElement : new()
    {
        public ListView ListView { get; }

        public ListViewInitializer(ListView listView, List<TListElement> data)
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
        
        public void UpdateItemsSource(List<TListElement> itemsSource)
        {
            ListView.itemsSource = itemsSource;
        }

        private TVisualElement CreateElement()
        {
            return new TVisualElement();
        }

        private void OnElementScrollIntoView(VisualElement element, int index)
        {
            var elementAsDataType = element as IBindableListViewElement<TListElement>;

            var itemsSourceAsList = ListView.itemsSource as List<TListElement>;
            itemsSourceAsList[index] ??= new();

            elementAsDataType?.CacheData(itemsSourceAsList[index]);
            elementAsDataType?.BindElementToCachedData();
            elementAsDataType?.SetElementFromCachedData();
        }
    }
}