using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

namespace Ludwell.Scene.Editor
{
    public class TagsShelfController
    {
        private TagSubscriberWithTags _data = new();

        private readonly TagsShelfView _view;

        public int IndexOf(TagWithSubscribers tagWithSubscribers) => _data.Tags.IndexOf(tagWithSubscribers);

        public TagsShelfController(VisualElement parent, EventCallback<ClickEvent> onOptionClicked)
        {
            _view = new TagsShelfView(parent, onOptionClicked);
        }

        public void Add(TagWithSubscribers tag)
        {
            if (Contains(tag)) return;
            _data.Tags.Add(tag);
            tag.AddSubscriber(_data);
            _view.Add(ConstructTagElement(tag));
            Sort();

            DataFetcher.SaveQuickLoadElementsAndTagContainerDelayed();
        }

        public void Remove(TagWithSubscribers tagWithSubscribers)
        {
            if (!Contains(tagWithSubscribers)) return;

            _view.RemoveAt(IndexOf(tagWithSubscribers));
            _data.Tags.Remove(tagWithSubscribers);
            tagWithSubscribers.RemoveSubscriber(_data);

            DataFetcher.SaveQuickLoadElementsAndTagContainerDelayed();
        }

        private void Sort()
        {
            _data.Tags.Sort();
            _view.Sort();
        }

        public void UpdateData(TagSubscriberWithTags data)
        {
            _data = data;
        }

        public void Populate()
        {
            _view.ClearContainer();
            _view.Populate(ConstructTagElements(_data.Tags));
        }

        public void OverrideIconTooltip(string value)
        {
            _view.OverrideIconTooltip(value);
        }

        private bool Contains(Tag tag)
        {
            return _data.Tags.Contains(tag);
        }

        private TagsShelfElementController ConstructTagElement(Tag tag)
        {
            TagsShelfElementController tagsShelfElementController = new();
            tagsShelfElementController.UpdateCache(tag);
            tagsShelfElementController.SetTagShelfController(this);
            return tagsShelfElementController;
        }

        private IEnumerable<TagsShelfElementController> ConstructTagElements(List<Tag> tags)
        {
            return tags.Select(ConstructTagElement);
        }
    }
}