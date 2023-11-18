using System;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ludwell.Scene
{
    /// <summary>
    /// InitDropdownElementBehaviour()
    /// must be called to initialize the dropdown elements' action.<br/>
    /// <list type="number">
    /// <item>The dropdown will be populated from the specified Listview elements.</item>
    /// <item>The action will be invoked on mouse up from any drop down element.</item>
    /// </list>
    /// </summary>
    public class DropdownSearchField : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<DropdownSearchField, UxmlTraits> { }

        private const string UxmlPath = "Uxml/" + nameof(DropdownSearchField) + "/" + nameof(DropdownSearchField);
        private const string UssPath = "Uss/" + nameof(DropdownSearchField) + "/" + nameof(DropdownSearchField);

        private const string SearchFieldName = "toolbar-search-field";

        private ToolbarSearchField _searchField;
        private DropdownListView _dropdownListView;

        private const float BorderRadius = 3;

        public DropdownSearchField()
        {
            this.AddHierarchyFromUxml(UxmlPath);
            this.AddStyleFromUss(UssPath);

            InitDropDown();
            InitSearchField();
            OnClickedSearchFieldRefreshDropdown();
            
            RegisterCallback<MouseLeaveWindowEvent>(_ =>
            {
                Debug.LogError("leave window");
                HideDropdown();
            });
        }

        public void InitDropdownElementBehaviour(ListView populateFrom, Action<int> actionAtIndex)
        {
            InitPlaceDropdown();

            var itemsSource = populateFrom.itemsSource;

            _searchField.RegisterValueChangedCallback(evt =>
            {
                _dropdownListView.ClearData();

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
                        _dropdownListView.Add(
                            dataName,
                            () => actionAtIndex.Invoke(index));
                    }
                }

                if (_dropdownListView.Count == 0) return;
                ShowDropdown();
            });
        }

        public void ShowDropdown()
        {
            SetBottomBorderRadii(0f);
            _dropdownListView.PlaceUnder(this);
            _dropdownListView.Show();
        }

        public void HideDropdown()
        {
            SetBottomBorderRadii(BorderRadius);
            _dropdownListView.Hide();
        }

        public List<DropdownElement> GetDropdownElements()
        {
            return _dropdownListView.GetElements();
        }

        public void ClearDropdownData()
        {
            _dropdownListView.ClearData();
        }

        private void InitSearchField()
        {
            _searchField = this.Q<ToolbarSearchField>(SearchFieldName);
        }

        private void OnClickedSearchFieldRefreshDropdown()
        {
            RegisterCallback<MouseUpEvent>(evt =>
            {
                if (!_dropdownListView.IsHidden) return;
                if (_searchField.value == string.Empty) return;

                var value = _searchField.value;
                _searchField.value = string.Empty;
                _searchField.value = value;
            });
        }

        private void InitDropDown()
        {
            RegisterCallback<AttachToPanelEvent>(_ =>
            {
                _dropdownListView = new DropdownListView();
                var rootVisualContainer = this.Root().FindFirstChildWhereNameContains(UiToolkitNames.RootVisualContainer);
                rootVisualContainer.Add(_dropdownListView);
                
                _dropdownListView.Hide();
            });
        }

        private void InitPlaceDropdown()
        {
            this.Root().RegisterCallback<GeometryChangedEvent>(_ => _dropdownListView.PlaceUnder(this));
        }

        public void InitMouseEvents(VisualElement registerFrom)
        {
            OnMouseUpHideDropdown(registerFrom);
            OnEventCaptureHideDropdown(registerFrom);
        }

        private void OnMouseUpHideDropdown(VisualElement registerFrom)
        {
            registerFrom.RegisterCallback<MouseUpEvent>(_ =>
            {
                if (_dropdownListView.IsHidden) return;

                HideDropdown();
            });
        }

        // todo: investigate better solution & remove line ~61 "if (itemsSource[i] == null) break;"
        // note: issue stems from buttons consuming events, so the OnMouseUpHideDropdown is never called
        private void OnEventCaptureHideDropdown(VisualElement registerFrom)
        {
            registerFrom.RegisterCallback<MouseCaptureEvent>(evt =>
            {
                if (_dropdownListView.IsHidden) return;

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
            return evt.target is DropdownElement ||
                   evt.target == _dropdownListView.Q(UiToolkitNames.UnityDragContainer) ||
                   evt.target == _dropdownListView.Q(UiToolkitNames.UnityLowButton) ||
                   evt.target == _dropdownListView.Q(UiToolkitNames.UnityHighButton);
        }

        private void SetBottomBorderRadii(float radius)
        {
            _searchField.style.borderBottomLeftRadius = radius;
            _searchField.style.borderBottomRightRadius = radius;
        }
    }
}