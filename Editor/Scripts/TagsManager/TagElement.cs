using System.IO;
using UnityEngine.UIElements;

namespace Ludwell.Scene
{
    public class TagElement : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<TagElement, UxmlTraits>
        {
        }

        private static readonly string UxmlPath = Path.Combine("Uxml", nameof(TagsManager), nameof(TagElement));
        private static readonly string UssPath = Path.Combine("Uss", nameof(TagsManager), nameof(TagElement));

        private const string RemoveButtonName = "button-remove";
        private const string MainButtonName = "button-main";
        private const string SearchButtonName = "button-search";

        private static TagElement _currentSelection;

        private Button _removeButton;
        private Button _mainButton;
        private Button _searchButton;
        private string _listingStrategyName = "tag";

        private TagsController _tagsController;
        private DropdownSearchField _dropdownSearchField;

        private Tag _cache;

        public TagElement()
        {
            this.AddHierarchyFromUxml(UxmlPath);
            this.AddStyleFromUss(UssPath);

            SetReferences();
            SetButtonEvents();

            ToggleBehaviourButtons(DisplayStyle.None);
        }

        ~TagElement()
        {
            _cache.RemoveValueChangedCallback(SetText);
        }

        public string Value => _mainButton.text;

        public void UpdateCache(Tag tag)
        {
            SetText(tag.Name);
            _cache = tag;
            _cache.AddValueChangedCallback(SetText);
        }

        private void SetReferences()
        {
            _removeButton = this.Q<Button>(RemoveButtonName);
            _mainButton = this.Q<Button>(MainButtonName);
            _searchButton = this.Q<Button>(SearchButtonName);

            RegisterCallback<AttachToPanelEvent>(_ =>
            {
                _tagsController = GetFirstAncestorOfType<TagsController>();
                _dropdownSearchField = this.FindInAncestors<DropdownSearchField>();
                _listingStrategyName = _dropdownSearchField.HasSearchStrategy(_listingStrategyName)
                    ? _listingStrategyName
                    : DropdownSearchField.DefaultSearchName;
            });
        }

        private void SetButtonEvents()
        {
            _mainButton.RegisterCallback<ClickEvent>(_ => { SelectTag(this); });

            _removeButton.RegisterCallback<ClickEvent>(_ => _tagsController.Remove(_cache as TagWithSubscribers));

            _searchButton.RegisterCallback<ClickEvent>(_ =>
            {
                _dropdownSearchField.ListWithStrategy(_listingStrategyName, _mainButton.text);
                ToggleVisual();
            });
        }

        private void SelectTag(TagElement tagElement)
        {
            if (_currentSelection == tagElement)
            {
                ToggleVisual();
                return;
            }

            _currentSelection?.ToggleBehaviourButtons(DisplayStyle.None);
            _currentSelection = tagElement;
            _currentSelection?.ToggleBehaviourButtons(DisplayStyle.Flex);
        }

        private void ToggleVisual()
        {
            var reverseDisplay = GetReverseDisplayStyle();
            _currentSelection.ToggleBehaviourButtons(reverseDisplay);
        }

        private DisplayStyle GetReverseDisplayStyle()
        {
            return _currentSelection._removeButton.style.display == DisplayStyle.None
                ? DisplayStyle.Flex
                : DisplayStyle.None;
        }

        private void ToggleBehaviourButtons(DisplayStyle displayStyle)
        {
            _removeButton.style.display = displayStyle;
            _searchButton.style.display = displayStyle;
        }

        private void SetText(string text)
        {
            _mainButton.text = text;
            _tagsController?.Rebuild();
        }
    }
}