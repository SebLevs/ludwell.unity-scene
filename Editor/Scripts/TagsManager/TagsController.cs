using System;
using System.Collections.Generic;
using Ludwell.Scene.Editor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ludwell.Scene
{
    public class TagsController : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<TagsController, UxmlTraits>
        {
        }

        private const string UxmlPath = "Uxml/" + nameof(TagsManager) + "/" + nameof(TagsController);
        private const string UssPath = "Uss/" + nameof(TagsManager) + "/" + nameof(TagsController);

        private const string AddButtonName = "tags__button-add";
        private const string TagsContainerName = "tags-container";
        private const string NotTaggedName = "not-tagged";
        private const string IconName = "icon";
        private const string IconButtonName = "tags__button-add";

        private List<Tag> _cachedTags = new();

        private Button _manageTagsButton;
        private VisualElement _tagsContainer;
        private Label _notTaggedLabel;

        public TagsController()
        {
            this.AddHierarchyFromUxml(UxmlPath);
            this.AddStyleFromUss(UssPath);

            SetReferences();
        }

        public void OverrideIcon(Texture2D icon)
        {
            this.Q(IconName).style.backgroundImage = new StyleBackground(icon);
        }

        public void OverrideIconTooltip(string value)
        {
            this.Q(IconButtonName).tooltip = value;
        }

        public TagsController WithTagList(List<Tag> tags)
        {
            _cachedTags = tags;
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
            if (_cachedTags.Contains(tag)) return;

            _cachedTags.Add(tag);
            _cachedTags.Sort();
            _tagsContainer.Add(CreateTagElement(tag));
            SortTagElements();
            HandleNotTaggedState();

#if UNITY_EDITOR
            LoaderSceneDataHelper.SaveChange();
#endif
        }

        public void Remove(Tag tag)
        {
            if (!_cachedTags.Contains(tag)) return;

            _tagsContainer.RemoveAt(_cachedTags.IndexOf(tag));
            _cachedTags.Remove(tag);
            HandleNotTaggedState();

#if UNITY_EDITOR
            LoaderSceneDataHelper.SaveChange();
#endif
        }

        public void Refresh()
        {
            _tagsContainer.Clear();

            foreach (var tag in _cachedTags)
            {
                _tagsContainer.Add(CreateTagElement(tag));
            }

            HandleNotTaggedState();
        }

        private void SetReferences()
        {
            _manageTagsButton = this.Q<Button>(AddButtonName);
            _tagsContainer = this.Q<VisualElement>(TagsContainerName);
            _notTaggedLabel = this.Q<Label>(NotTaggedName);
        }

        private TagElement CreateTagElement(Tag tag)
        {
            TagElement tagElement = new();
            tagElement.SetTagName(tag);
            return tagElement;
        }

        private void SortTagElements()
        {
            _tagsContainer.Sort((a, b) =>
            {
                var aValue = (a as TagElement).Value;
                var bValue = (b as TagElement).Value;
                return string.Compare(aValue, bValue);
            });
        }

        private void HandleNotTaggedState()
        {
            _notTaggedLabel.style.display = _cachedTags.Count == 0 ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }
}