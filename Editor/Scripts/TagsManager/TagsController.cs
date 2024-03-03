using System;
using System.IO;
using Ludwell.Scene.Editor;
using UnityEngine.UIElements;

namespace Ludwell.Scene
{
    public class TagsController : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<TagsController, UxmlTraits>
        {
        }

        private static readonly string UxmlPath = Path.Combine("Uxml", nameof(TagsManager), nameof(TagsController));
        private static readonly string UssPath = Path.Combine("Uss", nameof(TagsManager), nameof(TagsController));

        private const string AddButtonName = "tags__button-add";
        private const string TagsContainerName = "tags-container";
        private const string NotTaggedName = "not-tagged";
        private const string IconButtonName = "tags__button-add";

        private Button _manageTagsButton;
        private VisualElement _tagsContainer;
        private TagSubscriberWithTags _cache = new();
        private Label _notTaggedLabel;

        public TagsController()
        {
            this.AddHierarchyFromUxml(UxmlPath);
            this.AddStyleFromUss(UssPath);

            SetReferences();
        }

        public void OverrideIconTooltip(string value)
        {
            this.Q(IconButtonName).tooltip = value;
        }

        public TagsController WithTagSubscriber(TagSubscriberWithTags tagSubscriber)
        {
            _cache = tagSubscriber;
            return this;
        }

        public TagsController WithOptionButtonEvent(Action callback)
        {
            _manageTagsButton.style.display = DisplayStyle.Flex;
            _manageTagsButton.RegisterCallback<ClickEvent>(_ => { callback.Invoke(); });
            return this;
        }

        public void Add(Tag tag)
        {
            if (_cache.Tags.Contains(tag)) return;

            _cache.Tags.Add(tag);
            _tagsContainer.Add(ConstructTagElement(tag));
            Rebuild();

            DataFetcher.SaveEveryScriptableDelayed();
        }

        public void Remove(TagWithSubscribers tagWithSubscribers)
        {
            if (!_cache.Tags.Contains(tagWithSubscribers)) return;
            _tagsContainer.RemoveAt(_cache.Tags.IndexOf(tagWithSubscribers));
            _cache.Tags.Remove(tagWithSubscribers);
            tagWithSubscribers.RemoveSubscriber(_cache);
            HandleUntaggedState();

            DataFetcher.SaveEveryScriptableDelayed();
        }

        public void Rebuild()
        {
            _cache.Tags.Sort();
            Sort();
            HandleUntaggedState();
        }

        public void Populate()
        {
            _tagsContainer.Clear();
            foreach (var tag in _cache.Tags)
            {
                _tagsContainer.Add(ConstructTagElement(tag));
            }

            HandleUntaggedState();
        }

        private void SetReferences()
        {
            _manageTagsButton = this.Q<Button>(AddButtonName);
            _tagsContainer = this.Q<VisualElement>(TagsContainerName);
            _notTaggedLabel = this.Q<Label>(NotTaggedName);
        }

        private TagElement ConstructTagElement(Tag tag)
        {
            TagElement tagElement = new();
            tagElement.UpdateCache(tag);
            return tagElement;
        }

        private void Sort()
        {
            _tagsContainer.Sort((a, b) =>
            {
                var aValue = (a as TagElement).Value;
                var bValue = (b as TagElement).Value;
                return string.Compare(aValue, bValue, StringComparison.InvariantCulture);
            });
        }

        private void HandleUntaggedState()
        {
            _notTaggedLabel.style.display = _cache.Tags.Count == 0 ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }
}