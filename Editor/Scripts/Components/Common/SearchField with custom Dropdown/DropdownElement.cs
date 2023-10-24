using UnityEngine.UIElements;

namespace Ludwell.Scene
{
    public class DropdownElement : VisualElement, IBindableListViewElement<DropdownData>
    {
        private const string UxmlPath = "Uxml/dropdown-element";

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