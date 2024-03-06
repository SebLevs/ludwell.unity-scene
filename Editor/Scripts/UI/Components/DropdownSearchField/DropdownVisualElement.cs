using System.IO;
using UnityEngine.UIElements;

namespace Ludwell.Scene
{
    public class DropdownVisualElement : VisualElement, IListViewVisualElement<DropdownData>
    {
        private static readonly string UxmlPath = Path.Combine("Uxml", nameof(DropdownSearchField), nameof(DropdownVisualElement));
        private static readonly string UssPath = Path.Combine("Uss", nameof(DropdownSearchField), nameof(DropdownVisualElement));

        private readonly Button _button;
        
        private DropdownData _cache;

        public DropdownVisualElement()
        {
            this.AddHierarchyFromUxml(UxmlPath);
            this.AddStyleFromUss(UssPath);
            _button = this.Q<Button>();
        }

        public void CacheData(DropdownData data)
        {
            _cache = data;
        }

        public void BindElementToCachedData()
        {
            _cache.VisualElement = this;
        }

        public void SetElementFromCachedData()
        {
            _button.text = _cache.Name;
            _button.clicked += _cache.Action;
        }
    }
}