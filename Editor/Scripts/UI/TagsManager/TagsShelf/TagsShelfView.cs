using UnityEngine.UIElements;

namespace Ludwell.Scene
{
    public class TagsShelfView
    {
        private const string AddButtonName = "tags__button-add";
        private const string IconButtonName = "tags__button-add";

        private Button _optionsButton;

        private VisualElement _root;

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

        private void SetReferences()
        {
            _optionsButton = _root.Q<Button>(AddButtonName);
        }
    }
}