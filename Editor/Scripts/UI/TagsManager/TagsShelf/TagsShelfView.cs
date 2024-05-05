using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Ludwell.Scene.Editor
{
    public class TagsShelfView
    {
        private const string AddButtonName = "tags__button-add";
        private const string TagsContainerName = "tags-container";
        private const string NotTaggedName = "not-tagged";
        private const string IconButtonName = "tags__button-add";

        private readonly Action _onOptionClicked;

        private Button _optionsButton;
        private ScrollView _container;
        private Label _notTaggedLabel;

        private VisualElement _root;

        private int ElementsCount => _container.childCount;

        public TagsShelfView(VisualElement parent, Action onOptionClicked)
        {
            _root = parent.Q(nameof(TagsShelfView));
            SetReferences();
            _onOptionClicked = onOptionClicked;
            _optionsButton.clicked += ExecuteOptionBehaviour;
        }

        ~TagsShelfView()
        {
            _optionsButton.clicked -= ExecuteOptionBehaviour;
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
            HandleUntaggedState();
        }

        public void RemoveAt(int index)
        {
            _container.RemoveAt(index);
            HandleUntaggedState();
        }

        public void ClearContainer()
        {
            _container.Clear();
            HandleUntaggedState();
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
            _container = _root.Q<ScrollView>(TagsContainerName);
            _notTaggedLabel = _root.Q<Label>(NotTaggedName);
        }

        private void HandleUntaggedState()
        {
            _notTaggedLabel.style.display = ElementsCount == 0 ? DisplayStyle.Flex : DisplayStyle.None;
            _container.style.display = ElementsCount == 0 ? DisplayStyle.None : DisplayStyle.Flex;
        }

        private void ExecuteOptionBehaviour()
        {
            _onOptionClicked?.Invoke();
        }
    }
}