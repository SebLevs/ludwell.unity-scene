using System;
using System.Collections;
using System.Collections.Generic;
using Ludwell.Scene.Editor;
using UnityEditor.UIElements;
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
        public new class UxmlFactory : UxmlFactory<DropdownSearchField, UxmlTraits>
        {
        }

        private const string UxmlPath = "Uxml/" + nameof(DropdownSearchField) + "/" + nameof(DropdownSearchField);
        private const string UssPath = "Uss/" + nameof(DropdownSearchField) + "/" + nameof(DropdownSearchField);

        private const string SearchFieldName = "toolbar-search-field";

        private const float BorderRadius = 3;

        private ToolbarSearchField _searchField;
        private DropdownListView _dropdownListView;

        private ListView _boundListView;
        private IList _boundItemsSource;
        private List<LoaderListViewElementData> _filteredList = new();

        public DropdownSearchField()
        {
            this.AddHierarchyFromUxml(UxmlPath);
            this.AddStyleFromUss(UssPath);

            InitDropDown();
            InitSearchField();
            InitializeSearchListing();

            KeepDropdownUnderSelf(this);
        }

        public void BindToListView(ListView listView)
        {
            _boundListView = listView;
            _boundItemsSource = listView.itemsSource;
        }

        public void WithResizableParent(VisualElement resizableParent)
        {
            UnregisterCallback<GeometryChangedEvent>(_ => PlaceUnder());
            KeepDropdownUnderSelf(resizableParent);
        }

        public void WithDropdownBehaviour(Action<int> actionAtIndex)
        {
            _searchField.RegisterValueChangedCallback(evt =>
            {
                _dropdownListView.ClearData();
                if (evt.newValue == string.Empty)
                {
                    HideDropdown();
                    return;
                }

                for (var i = 0; i < _boundItemsSource.Count; i++)
                {
                    if (_boundItemsSource[i] == null) break;
                    var dataName = (_boundItemsSource[i] as ISearchFieldListable).GetName();
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

        private void InitSearchField()
        {
            _searchField = this.Q<ToolbarSearchField>(SearchFieldName);
        }

        private void InitDropDown()
        {
            RegisterCallback<AttachToPanelEvent>(_ =>
            {
                _dropdownListView = new DropdownListView();
                _dropdownListView.SetOwner(this);
                var rootVisualContainer =
                    this.Root().FindFirstChildWhereNameContains(UiToolkitNames.RootVisualContainer);
                rootVisualContainer.Add(_dropdownListView);

                _dropdownListView.Hide();
            });
        }

        private void InitializeSearchListing()
        {
            _searchField.RegisterValueChangedCallback(evt =>
            {
                if (string.IsNullOrEmpty(evt.newValue))
                {
                    _boundListView.itemsSource = _boundItemsSource;
                    _boundListView.Rebuild();
                    return;
                }

                _filteredList = new();
                _boundListView.itemsSource = _filteredList;
                foreach (var element in _boundItemsSource)
                {
                    var dataName = (element as ISearchFieldListable).GetName();
                    if (!dataName.ToLower().Contains(evt.newValue.ToLower())) continue;
                    _filteredList.Add(element as LoaderListViewElementData);
                }

                _boundListView.Rebuild();
            });
        }

        private void KeepDropdownUnderSelf(VisualElement resizableElement)
        {
            resizableElement.RegisterCallback<GeometryChangedEvent>(_ => PlaceUnder());
        }

        private void PlaceUnder()
        {
            _dropdownListView.PlaceUnder(this);
        }

        private void SetBottomBorderRadii(float radius)
        {
            _searchField.style.borderBottomLeftRadius = radius;
            _searchField.style.borderBottomRightRadius = radius;
        }
    }
}