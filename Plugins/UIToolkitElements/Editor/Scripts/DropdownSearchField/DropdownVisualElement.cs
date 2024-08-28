using System.IO;
using Ludwell.UIToolkitUtilities;
using Ludwell.UIToolkitUtilities.Editor;
using UnityEngine.UIElements;

namespace Ludwell.UIToolkitElements.Editor
{
    public class DropdownVisualElement : VisualElement, IListViewVisualElement<DropdownData>
    {
        private static readonly string UxmlPath =
            Path.Combine("UI", nameof(DropdownSearchField), "Uxml_" + nameof(DropdownVisualElement));

        private static readonly string UssPath =
            Path.Combine("UI", nameof(DropdownSearchField), "Uss_" + nameof(DropdownVisualElement));

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
