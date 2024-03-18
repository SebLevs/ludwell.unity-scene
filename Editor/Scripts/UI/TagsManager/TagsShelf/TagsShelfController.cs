using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ludwell.Scene.Editor
{
    public class TagsShelfController
    {
        private TagSubscriberWithTags _data = new();

        private readonly TagsShelfView _view;

        private ListViewHandler<TagsShelfElementController, Tag> _listViewHandler;

        public TagsShelfController(VisualElement parent, EventCallback<ClickEvent> onOptionClicked)
        {
            _listViewHandler = new ListViewHandler<TagsShelfElementController, Tag>(parent.Q<ListView>(), _data.Tags);
            _listViewHandler.OnItemMade += OnItemMadeSetTagShelfController;
            PreventWheelCallbackPropagation();
            
            _view = new TagsShelfView(parent, onOptionClicked);
        }

        public void Add(TagWithSubscribers tag)
        {
            if (Contains(tag)) return;
            _listViewHandler.ListView.itemsSource.Add(tag);
            tag.AddSubscriber(_data);
            Sort();

            DataFetcher.SaveEveryScriptableDelayed();
        }

        public void Remove(TagWithSubscribers tagWithSubscribers)
        {
            if (!Contains(tagWithSubscribers)) return;
            
            _listViewHandler.ListView.itemsSource.Remove(tagWithSubscribers);
            tagWithSubscribers.RemoveSubscriber(_data);
            
            _listViewHandler.ForceRebuild();
            DataFetcher.SaveEveryScriptableDelayed();
        }

        private void Sort()
        {
            ((List<Tag>)_listViewHandler.ListView.itemsSource).Sort();
            _listViewHandler.ForceRebuild();
        }

        public void UpdateData(TagSubscriberWithTags data)
        {
            _listViewHandler.ListView.itemsSource = data.Tags;
            _listViewHandler.ForceRebuild();
        }

        public void OverrideIconTooltip(string value)
        {
            _view.OverrideIconTooltip(value);
        }

        private bool Contains(Tag tag)
        {
            return _listViewHandler.ListView.itemsSource.Contains(tag);
        }
        
        private void OnItemMadeSetTagShelfController(TagsShelfElementController controller)
        {
            controller.SetTagShelfController(this);
        }
        
        private void PreventWheelCallbackPropagation()  
        {  
            var scroller = _listViewHandler.ListView.Q<Scroller>();  
            _listViewHandler.ListView.RegisterCallback<WheelEvent>(evt =>  
            {  
                if (scroller.style.display == DisplayStyle.None) return;
                if (evt.delta.y < 0 && Mathf.Approximately(scroller.value, scroller.lowValue))  
                {            
                    evt.StopPropagation();  
                }        
                else if (evt.delta.y > 0 && Mathf.Approximately(scroller.value, scroller.highValue))  
                {            
                    evt.StopPropagation();  
                } 
                
            });
        }
    }
}