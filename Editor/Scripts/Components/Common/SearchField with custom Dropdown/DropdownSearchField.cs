using System;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Ludwell.Scene
{
    /// <summary>
    /// InitDropdownElementBehaviour() must be called to initialize the dropdown behaviour.<br/>
    /// 1 - Will automatically populate the dropdown from a Listview.<br/>
    /// 2 - Will invoke an action when an element is clicked at a specific index.
    /// </summary>
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
            InitCallbacks();

            HideDropdown();
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

        private void InitCallbacks()
        {
            OnClickedSearchFieldRefreshDropdown();
        }

        private void OnClickedSearchFieldRefreshDropdown()
        {
            RegisterCallback<MouseUpEvent>(evt =>
            {
                if (!_dropdown.IsHidden) return;
                if (_searchField.value == string.Empty) return;

                var value = _searchField.value;
                _searchField.value = string.Empty;
                _searchField.value = value;
            });
        }

        public void InitDropdownElementBehaviour(ListView populateFrom, Action<int> actionAtIndex)
        {
            var itemsSource = populateFrom.itemsSource;

            _searchField.RegisterValueChangedCallback(evt =>
            {
                _dropdown.ClearData();

                if (evt.newValue == string.Empty)
                {
                    HideDropdown();
                    return;
                }

                for (var i = 0; i < itemsSource.Count; i++)
                {
                    var dataName = (itemsSource[i] as LoaderListViewElementData).Name;
                    if (dataName.ToLower().Contains(evt.newValue.ToLower()))
                    {
                        var index = i;
                        _dropdown.Add(
                            dataName,
                            () => actionAtIndex.Invoke(index));
                    }
                }

                if (_dropdown.Count == 0) return;
                ShowDropdown();
            });
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

        public List<DropdownElement> GetDropdownElements()
        {
            return _dropdown.GetElements();
        }

        public void ClearDropdownData()
        {
            _dropdown.ClearData();
        }

        private void SetBottomBorderRadii(float radius)
        {
            _searchField.style.borderBottomLeftRadius = radius;
            _searchField.style.borderBottomRightRadius = radius;
        }
    }
}