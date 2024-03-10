using System;
using System.Collections.Generic;
using System.IO;
using Ludwell.Scene.Editor;
using UnityEngine.UIElements;

namespace Ludwell.Scene
{
    public class TagsShelfView : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<TagsShelfView, UxmlTraits>
        {
        }

        private static readonly string UxmlPath = Path.Combine("Uxml", nameof(TagsManager), nameof(TagsShelfView));
        private static readonly string UssPath = Path.Combine("Uss", nameof(TagsManager), nameof(TagsShelfView));

        private const string AddButtonName = "tags__button-add";
        private const string TagsContainerName = "tags-container";
        private const string NotTaggedName = "not-tagged";
        private const string IconButtonName = "tags__button-add";

        private Button _manageTagsButton;
        private VisualElement _container;
        private Label _notTaggedLabel;

        private TagsShelfController _controller;

        public TagsShelfView()
        {
            this.AddHierarchyFromUxml(UxmlPath);
            this.AddStyleFromUss(UssPath);

            SetReferences();
        }

        public void OverrideIconTooltip(string value)
        {
            this.Q(IconButtonName).tooltip = value;
        }

        public TagsShelfView WithTagSubscriber(TagSubscriberWithTags tagSubscriber)
        {
            _controller.UpdateData(tagSubscriber);
            return this;
        }

        public TagsShelfView WithOptionButtonEvent(Action callback)
        {
            _manageTagsButton.style.display = DisplayStyle.Flex;
            _manageTagsButton.RegisterCallback<ClickEvent>(_ => { callback.Invoke(); });
            return this;
        }

        public bool ContainsData(Tag tag)
        {
            return _controller.Contains(tag);
        }

        public void Add(Tag tag)
        {
            if (_controller.Contains(tag)) return;

            _controller.Add(tag);
            _container.Add(ConstructTagElement(tag));
            Rebuild();

            DataFetcher.SaveEveryScriptableDelayed();
        }

        public void Remove(TagWithSubscribers tagWithSubscribers)
        {
            if (!_controller.Contains(tagWithSubscribers)) return;
            _container.RemoveAt(_controller.IndexOf(tagWithSubscribers));
            _controller.Remove(tagWithSubscribers);
            HandleUntaggedState();

            DataFetcher.SaveEveryScriptableDelayed();
        }

        public void Rebuild()
        {
            _controller.Sort();
            Sort();
            HandleUntaggedState();
        }

        public void PopulateContainer()
        {
            _controller.PopulateContainer(this);
            HandleUntaggedState();
        }

        public void ClearContainer()
        {
            _container.Clear();
        }

        public void Populate(List<Tag> tags)
        {
            foreach (var tag in tags)
            {
                _container.Add(ConstructTagElement(tag));
            }
        }

        private void SetReferences()
        {
            _manageTagsButton = this.Q<Button>(AddButtonName);
            _container = this.Q<VisualElement>(TagsContainerName);
            _notTaggedLabel = this.Q<Label>(NotTaggedName);

            _controller = new TagsShelfController();
        }

        private TagsShelfElementView ConstructTagElement(Tag tag)
        {
            TagsShelfElementView tagsShelfElementView = new();
            tagsShelfElementView.UpdateCache(tag);
            return tagsShelfElementView;
        }

        private void Sort()
        {
            _container.Sort((a, b) =>
            {
                var aValue = (a as TagsShelfElementView).Value;
                var bValue = (b as TagsShelfElementView).Value;
                return string.Compare(aValue, bValue, StringComparison.InvariantCulture);
            });
        }

        private void HandleUntaggedState()
        {
            _notTaggedLabel.style.display = _controller.Count == 0 ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }
}