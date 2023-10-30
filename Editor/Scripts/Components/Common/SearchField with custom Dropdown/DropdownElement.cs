using UnityEngine.UIElements;

namespace Ludwell.Scene
{
    public class DropdownElement : VisualElement, IBindableListViewElement<DropdownData>
    {
        public const string Name = "dropdown__element";
        private const string UxmlPath = "Uxml/" + nameof(DropdownSearchField) + "/" + nameof(DropdownElement);
        private const string UssPath = "Uss/" + nameof(DropdownSearchField) + "/" + nameof(DropdownElement);

        private readonly Button _button;

        public DropdownElement()
        {
            this.SetHierarchyFromUxml(UxmlPath);
            this.AddStyleFromUss(UssPath);
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