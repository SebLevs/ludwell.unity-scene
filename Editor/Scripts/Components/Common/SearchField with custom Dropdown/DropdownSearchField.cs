using System;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Ludwell.Scene
{
    /// <summary>
    /// InitDropdownElementBehaviour() must be called to initialize the dropdown elements' action.<br/>
    /// 1 - Will automatically populate the dropdown from the specified Listview elements.<br/>
    /// 2 - Will invoke the specified action on mouse up of the dropdown element.
    /// </summary>
    public class DropdownSearchField : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<DropdownSearchField, UxmlTraits> { }

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
            OnClickedSearchFieldRefreshDropdown();

            HideDropdown();
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
            SetBottomBorderRadius(0f);
            _dropdown.Show();
        }

        public void HideDropdown()
        {
            SetBottomBorderRadius(BorderRadius);
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

        // todo: investigate better solution & remove line ~61 "if (itemsSource[i] == null) break;"
        // note: issue stems from buttons consuming events, so the OnMouseUpHideDropdown is never called
        private void OnEventCaptureHideDropdown(VisualElement registerFrom)
        {
            registerFrom.RegisterCallback<MouseCaptureEvent>(evt =>
            {
                if (_dropdown.IsHidden) return;

                if (IsTargetFromSelf(evt)) return;
                if (IsTargetFromDropdown(evt)) return;

                HideDropdown();
            });
        }

        private bool IsTargetFromSelf(MouseCaptureEvent evt)
        {
            return evt.target == _searchField.Q(UiToolkitNames.UnitySearch) ||
                   evt.target == _searchField.Q(UiToolkitNames.UnityTextInput).ElementAt(0) ||
                   evt.target == _searchField.Q(UiToolkitNames.UnityTextInput).ElementAt(0);
        }

        private bool IsTargetFromDropdown(MouseCaptureEvent evt)
        {
            return (evt.target as VisualElement)?.name == DropdownElement.Name ||
                   evt.target == _dropdown.Q(UiToolkitNames.UnityDragContainer) ||
                   evt.target == _dropdown.Q(UiToolkitNames.UnityLowButton) ||
                   evt.target == _dropdown.Q(UiToolkitNames.UnityHighButton);
        }

        private void SetBottomBorderRadius(float radius)
        {
            _searchField.style.borderBottomLeftRadius = radius;
            _searchField.style.borderBottomRightRadius = radius;
        }
    }
}