using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

namespace Ludwell.Scene.Editor
{
    public class TagsShelfController : IDisposable
    {
        // private TagSubscriberWithTags _data = new();
        private TagSubscriberWithTags _data;

        private readonly TagsShelfView _view;

        public int IndexOf(Tag tag) => _data.Tags.IndexOf(tag);

        public TagsShelfController(VisualElement parent, Action onOptionClicked)
        {
            _view = new TagsShelfView(parent);
            _view.OnOptionClicked += onOptionClicked;
        }

        public void Dispose()
        {
            _view.Dispose();
        }

        public void Add(Tag tag)
        {
            if (Contains(tag)) return;
            _data.Tags.Add(tag);
            _view.Add(ConstructTagElement(tag));
            Sort();

            ResourcesLocator.SaveSceneAssetDataBindersAndTagsDelayed();
        }

        public void Remove(Tag tag)
        {
            if (!Contains(tag)) return;

            _view.RemoveAt(IndexOf(tag));
            _data.Tags.Remove(tag);

            ResourcesLocator.SaveSceneAssetDataBindersAndTagsDelayed();
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

        private IEnumerable<TagsShelfElementController> ConstructTagElements(List<Tag> tags)
        {
            return tags.Select(ConstructTagElement);
        }

        private TagsShelfElementController ConstructTagElement(Tag tag)
        {
            TagsShelfElementController tagsShelfElementController = new();
            tagsShelfElementController.UpdateCache(tag);
            tagsShelfElementController.SetTagShelfController(this);
            return tagsShelfElementController;
        }
    }
}