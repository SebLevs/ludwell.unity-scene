using System;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Ludwell.Scene
{
    public class DropdownSearchField : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<DropdownSearchField, UxmlTraits> { }

        public new class UxmlTraits : VisualElement.UxmlTraits { }

        private const string UxmlPath = "Uxml/search-field-dropdown";
        private const string UssPath = "Uss/search-field-dropdown";

        private const string SearchFieldName = "toolbar-search-field";

        private ToolbarSearchField _searchField;
        private Dropdown _dropdown;

        private const float BorderRadius = 3;

        public DropdownSearchField()
        {
            this.SetHierarchyFromUxml(UxmlPath);
            this.AddStyleFromUss(UssPath);

            InitDropdown();
            InitSearchField();

            HideDropdown();
        }

        public void BindToListView(ListView listView, Action<int> action)
        {
            _searchField.RegisterValueChangedCallback(evt =>
            {
                _dropdown.Clear();

                if (evt.newValue == string.Empty)
                {
                    HideDropdown();
                    return;
                }

                var itemsSource = listView.itemsSource;
                for (int i = 0; i < itemsSource.Count; i++)
                {
                    var dataName = (itemsSource[i] as LoaderListViewElementData).Name;
                    if (dataName.ToLower().Contains(evt.newValue.ToLower()))
                    {
                        var i1 = i;
                        _dropdown.Add(
                            i + ". " + dataName,
                            () => { listView.ScrollToItem(i1); });
                    }
                }

                if (_dropdown.Count == 0) return;
                ShowDropdown();
            });
        }

        private void InitDropdown()
        {
            _dropdown = new();
            Add(_dropdown);
        }

        private void InitSearchField()
        {
            _searchField = this.Q<ToolbarSearchField>(SearchFieldName);
        }

        public void ShowDropdown()
        {
            SetBottomBorderRadii(0f);
            _dropdown.Show();
        }

        public void HideDropdown()
        {
            SetBottomBorderRadii(BorderRadius);
            _dropdown.Hide();
        }

        private void SetBottomBorderRadii(float radius)
        {
            _searchField.style.borderBottomLeftRadius = radius;
            _searchField.style.borderBottomRightRadius = radius;
        }
    }
}