using UnityEngine.UIElements;

namespace Ludwell.Scene.Editor
{
    public class TagsShelfElementView
    {
        private const string RemoveButtonName = "button-remove";
        private const string MainButtonName = "button-main";
        private const string SearchButtonName = "button-search";

        private readonly Button _removeButton;
        private readonly Button _mainButton;
        private readonly Button _searchButton;

        public TagsShelfElementView(VisualElement view)
        {
            _removeButton = view.Q<Button>(RemoveButtonName);
            _mainButton = view.Q<Button>(MainButtonName);
            _searchButton = view.Q<Button>(SearchButtonName);
        }

        public void SetValue(string text)
        {
            _mainButton.text = text;
        }

        public void SetButtonsStyle(DisplayStyle displayStyle)
        {
            _removeButton.style.display = displayStyle;
            _searchButton.style.display = displayStyle;
        }

        public void ToggleVisual()
        {
            var reverseDisplay = GetReverseDisplayStyle();
            SetButtonsStyle(reverseDisplay);
        }

        private DisplayStyle GetReverseDisplayStyle()
        {
            return _removeButton.style.display == DisplayStyle.None
                ? DisplayStyle.Flex
                : DisplayStyle.None;
        }
    }
}