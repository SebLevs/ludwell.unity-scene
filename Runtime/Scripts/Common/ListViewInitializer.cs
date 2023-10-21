using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Ludwell.Scene
{
    public class ListViewInitializer<TVisualElement, TListElement>
        where TVisualElement : VisualElement, new()
        where TListElement : new()
    {
        private readonly List<TListElement> m_Data;

        public ListViewInitializer(ListView listView, List<TListElement> data)
        {
            m_Data = data;
            listView.makeItem = CreateElement;
            listView.bindItem = OnElementScrollIntoView;
            listView.itemsSource = data;
        }

        public TVisualElement CreateElement()
        {
            return new TVisualElement();
        }

        private void OnElementScrollIntoView(VisualElement element, int index)
        {
            var elementAsDataType = element as IBindableListViewElement<TListElement>;

            m_Data[index] ??= new();

            elementAsDataType?.CacheData(m_Data[index]);
            elementAsDataType?.BindElementToCachedData();
            elementAsDataType?.SetElementFromCachedData();
        }
    }
}