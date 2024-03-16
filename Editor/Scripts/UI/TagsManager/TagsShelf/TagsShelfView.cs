using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Ludwell.Scene
{
    public class TagsShelfView
    {
        private const string AddButtonName = "tags__button-add";
        private const string TagsContainerName = "tags-container";
        private const string NotTaggedName = "not-tagged";
        private const string IconButtonName = "tags__button-add";

        private Button _optionsButton;
        private VisualElement _container;
        private Label _notTaggedLabel;

        private VisualElement _root;

        private int ElementsCount => _container.contentContainer.childCount;

        public TagsShelfView(VisualElement parent, EventCallback<ClickEvent> onOptionClicked)
        {
            _root = parent.Q(nameof(TagsShelfView));
            SetReferences();
            _optionsButton.RegisterCallback(onOptionClicked);
        }

        public void OverrideIconTooltip(string value)
        {
            _root.Q(IconButtonName).tooltip = value;
        }

        public void Add(TagsShelfElementController tagShelfElementController)
        {
            _container.Add(tagShelfElementController);
            Sort();

            if (ElementsCount > 1) return;
            SetNotTaggedLabelDisplay(DisplayStyle.None);
        }

        public void RemoveAt(int index)
        {
            _container.RemoveAt(index);
            HandleUntaggedState();
        }

        public void ClearContainer()
        {
            _container.Clear();
            SetNotTaggedLabelDisplay(DisplayStyle.None);
        }

        private void SetNotTaggedLabelDisplay(DisplayStyle displayStyle)
        {
            _notTaggedLabel.style.display = displayStyle;
        }

        public void Populate(IEnumerable<TagsShelfElementController> tagElements)
        {
            foreach (var tag in tagElements)
            {
                _container.Add(tag);
            }

            HandleUntaggedState();
        }

        public void Sort()
        {
            _container.Sort((a, b) =>
            {
                var aValue = (a as TagsShelfElementController).Value;
                var bValue = (b as TagsShelfElementController).Value;
                return string.Compare(aValue, bValue, StringComparison.InvariantCulture);
            });
        }

        private void SetReferences()
        {
            _optionsButton = _root.Q<Button>(AddButtonName);
            _container = _root.Q<VisualElement>(TagsContainerName);
            _notTaggedLabel = _root.Q<Label>(NotTaggedName);
        }

        private void HandleUntaggedState()
        {
            _notTaggedLabel.style.display = ElementsCount == 0 ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }
}