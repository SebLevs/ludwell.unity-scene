using UnityEngine.UIElements;

namespace Ludwell.Scene
{
    public class DropdownElement : VisualElement, IBindableListViewElement<DropdownData>
    {
        public const string Name = "dropdown-element";
        private const string UxmlPath = "Uxml/dropdown__element";

        private readonly Button _button;

        public DropdownElement()
        {
            this.SetHierarchyFromUxml(UxmlPath);
            _button = this.Q<Button>();
        }

        public DropdownData Cache { get; set; }

        public void BindElementToCachedData()
        {
            Cache.Element = this;
        }

        public void SetElementFromCachedData()
        {
            _button.text = Cache.Name;
            _button.clicked += Cache.Action;
        }
    }
}