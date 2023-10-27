using System;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Ludwell.Scene
{
    /// <summary>
    /// InitDropdownElementBehaviour() must be called to initialize the dropdown elements' action.<br/>
    /// 1 - Will automatically populate the dropdown from the specified Listview elements.<br/>
    /// 2 - Will invoke the action of the clicked element on mouse up.
    /// </summary>
    public class DropdownSearchField : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<DropdownSearchField, UxmlTraits> { }

        public new class UxmlTraits : VisualElement.UxmlTraits { }

        private const string UxmlPath = "Uxml/search-field-dropdown";
        private const string UssPath = "Uss/search-field-dropdown";

        private const string SearchFieldName = "toolbar-search-field";

        private const string UnitySearchName = "unity-search";
        private const string UnityTextInputName = "unity-text-input";
        private const string UnityHighButtonName = "unity-high-button";
        private const string UnityLowButtonName = "unity-low-button";
        private const string UnityDragContainerName = "unity-drag-container";

        private ToolbarSearchField _searchField;
        private Dropdown _dropdown;

        private const float BorderRadius = 3;

        public DropdownSearchField()
        {
            this.SetHierarchyFromUxml(UxmlPath);
            this.AddStyleFromUss(UssPath);

            InitDropdown();
            InitSearchField();
            OnClickedSearchFieldRefreshDropdown();

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

        // todo: remove parameter when z-index is implemented to instead use .Root() & place in constructor instead
        public void InitMouseEvents(VisualElement registerFrom)
        {
            OnMouseUpHideDropdown(registerFrom);
            OnEventCaptureHideDropdown(registerFrom);
        }

        private void OnMouseUpHideDropdown(VisualElement registerFrom)
        {
            registerFrom.RegisterCallback<MouseUpEvent>(evt =>
            {
                if (_dropdown.IsHidden) return;

                HideDropdown();
            });
        }

        // todo: investigate better solution to replace this hack & remove line 124 "if (itemsSource[i] == null) break;"
        // note: issue stems from buttons consuming events, so the OnMouseUpHideDropdown is never called (signals instead?)
        private void OnEventCaptureHideDropdown(VisualElement registerFrom)
        {
            registerFrom.RegisterCallback<MouseCaptureEvent>(evt =>
            {
                if (_dropdown.IsHidden) return;

                if (evt.target == _searchField.Q(UnitySearchName)) return;
                if (evt.target == _searchField.Q(UnityTextInputName).ElementAt(0)) return;
                if (evt.target == _dropdown.Q(UnityDragContainerName)) return;
                if (evt.target == _dropdown.Q(UnityLowButtonName)) return;
                if (evt.target == _dropdown.Q(UnityHighButtonName)) return;

                HideDropdown();
                _dropdown.ClearData();
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
                    if (itemsSource[i] == null) break;
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