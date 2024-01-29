using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Ludwell.Scene
{
    public class ListViewInitializer<TVisualElement, TListElement>
        where TVisualElement : VisualElement, new()
        where TListElement : new()
    {
        private readonly List<TListElement> _data;

        public ListView ListView { get; private set; }

        public ListViewInitializer(ListView listView, List<TListElement> data)
        {
            _data = data;
            ListView = listView;
            ListView.itemsSource = _data;
            ListView.makeItem = CreateElement;
            ListView.bindItem = OnElementScrollIntoView;
            ListView.itemsAdded += _ => ForceRebuild(ListView);
            ListView.itemsRemoved += _ => ForceRebuild(ListView);

            // todo: replace workaround for the ListView visual bug concerning dynamically sized element rendering.
            listView.RegisterCallback<GeometryChangedEvent>(_ => { ListView.Rebuild(); });
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