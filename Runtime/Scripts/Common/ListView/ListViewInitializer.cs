using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Ludwell.Scene
{
    public class ListViewInitializer<TVisualElement, TListElement>
        where TVisualElement : VisualElement, new()
        where TListElement : new()
    {
        private readonly List<TListElement> _data;

        public ListViewInitializer(ListView listView, List<TListElement> data)
        {
            _data = data;
            listView.itemsSource = _data;
            listView.makeItem = CreateElement;
            listView.bindItem = OnElementScrollIntoView;
            listView.itemsAdded += _ => ForceRebuild(listView);
            listView.itemsRemoved += _ => ForceRebuild(listView);
            
            // todo: replace workaround for the ListView visual bug concerning dynamically sized element rendering.
            listView.RegisterCallback<GeometryChangedEvent>(evt =>
            {
                listView.Rebuild();
            });
        }

        private TVisualElement CreateElement()
        {
            return new TVisualElement();
        }

        private void OnElementScrollIntoView(VisualElement element, int index)
        {
            var elementAsDataType = element as IBindableListViewElement<TListElement>;

            _data[index] ??= new();

            elementAsDataType?.CacheData(_data[index]);
            elementAsDataType?.BindElementToCachedData();
            elementAsDataType?.SetElementFromCachedData();
        }

        /// <summary> Workaround for a ListView visual bug concerning dynamically sized element rendering. </summary>
        private void ForceRebuild(ListView listView)
        {
            listView.Rebuild();
        }
    }
}