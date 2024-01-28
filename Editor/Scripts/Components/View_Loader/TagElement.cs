using UnityEngine.UIElements;

namespace Ludwell.Scene
{
    public class TagElement : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<TagElement, UxmlTraits>
        {
        }

        private const string UxmlPath = "Uxml/" + nameof(LoaderController) + "/" + nameof(TagElement);
        private const string UssPath = "Uss/" + nameof(LoaderController) + "/" + nameof(TagElement);

        private const string RemoveButtonName = "button-remove";
        private const string MainButtonName = "button-main";
        private const string SearchButtonName = "button-search";

        private static TagElement _currentSelection;

        private Button _removeButton;
        private Button _mainButton;
        private Button _searchButton;

        private TagsController _tagsController;

        public TagElement()
        {
            this.AddHierarchyFromUxml(UxmlPath);
            this.AddStyleFromUss(UssPath);

            SetReferences();
            SetButtonEvents();

            ToggleBehaviourButtons(DisplayStyle.None);
        }

        public string Value => _mainButton.text;

        public void SetTagName(string value)
        {
            _mainButton.text = value;
        }

        private void SetReferences()
        {
            _removeButton = this.Q<Button>(RemoveButtonName);
            _mainButton = this.Q<Button>(MainButtonName);
            _searchButton = this.Q<Button>(SearchButtonName);

            RegisterCallback<AttachToPanelEvent>(_ =>
            {
                _tagsController = GetFirstAncestorOfType<TagsController>();
            });
        }

        private void SetButtonEvents()
        {
            _mainButton.RegisterCallback<ClickEvent>(_ => { SelectTag(this); });

            _removeButton.RegisterCallback<ClickEvent>(_ => { _tagsController.Remove(Value); });

            _searchButton.RegisterCallback<ClickEvent>(_ => { _tagsController.ShowElementsWithTag(Value); });
        }

        private static void SelectTag(TagElement tagElement)
        {
            if (_currentSelection == tagElement)
            {
                var reverseDisplay = GetReverseDisplayStyle();
                _currentSelection.ToggleBehaviourButtons(reverseDisplay);
                return;
            }

            _currentSelection?.ToggleBehaviourButtons(DisplayStyle.None);
            _currentSelection = tagElement;
            _currentSelection?.ToggleBehaviourButtons(DisplayStyle.Flex);
        }

        private static DisplayStyle GetReverseDisplayStyle()
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
    }
}