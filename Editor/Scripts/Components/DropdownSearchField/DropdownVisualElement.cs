using System.IO;
using UnityEngine.UIElements;

namespace Ludwell.Scene
{
    public class DropdownVisualElement : VisualElement, IListViewVisualElement<DropdownData>
    {
        private static readonly string UxmlPath = Path.Combine("Uxml", nameof(DropdownSearchField), nameof(DropdownVisualElement));
        private static readonly string UssPath = Path.Combine("Uss", nameof(DropdownSearchField), nameof(DropdownVisualElement));

        private readonly Button _button;

        public DropdownVisualElement()
        {
            this.AddHierarchyFromUxml(UxmlPath);
            this.AddStyleFromUss(UssPath);
            _button = this.Q<Button>();
        }

        public DropdownData Cache { get; set; }

        public void BindElementToCachedData()
        {
            Cache.VisualElement = this;
        }

        public void SetElementFromCachedData()
        {
            _button.text = Cache.Name;
            _button.clicked += Cache.Action;
        }
    }
}