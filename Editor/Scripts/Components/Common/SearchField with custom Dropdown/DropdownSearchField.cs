using System;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ludwell.Scene
{
    /// <summary>
    /// InitDropdownElementBehaviour() must be called to initialize the dropdown elements' action.<br/>
    /// <list type="number">
    /// <item>The dropdown will be populated from the specified Listview elements.</item>
    /// <item>The specified action will be invoked on mouse up of the dropdown element.</item>
    /// </list>
    /// </summary>
    public class DropdownSearchField : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<DropdownSearchField, UxmlTraits> { }

        private const string UxmlPath = "Uxml/" + nameof(DropdownSearchField) + "/search-field-dropdown";
        private const string UssPath = "Uss/" + nameof(DropdownSearchField) + "/search-field-dropdown";

        private const string SearchFieldName = "toolbar-search-field";

        private ToolbarSearchField _searchField;
        private Dropdown _dropdown;

        private const float BorderRadius = 3;

        public DropdownSearchField()
        {
            this.SetHierarchyFromUxml(UxmlPath);
            this.AddStyleFromUss(UssPath);

            InitDropDown();
            InitSearchField();
            OnClickedSearchFieldRefreshDropdown();
        }

        public void InitDropdownElementBehaviour(ListView populateFrom, Action<int> actionAtIndex)
        {
            InitPlaceDropdown();

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
            _dropdown.PlaceUnder(this);
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

        private void InitDropDown()
        {
            RegisterCallback<AttachToPanelEvent>(evt =>
            {
                _dropdown = new();
                _dropdown.Hide();
                this.Root().Add(_dropdown);
                // this.parent.parent.parent.parent.Add(_dropdown);
            });
        }
        
        private void InitPlaceDropdown()
        {
            this.Root().RegisterCallback<GeometryChangedEvent>(evt =>
            {
                _dropdown.PlaceUnder(this);
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