using System;
using System.Collections;
using System.Collections.Generic;
using Ludwell.Scene.Editor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ludwell.Scene
{
    /// <summary>
    /// Default behaviour on value changed is to list the element by name<br/>
    /// <list type="number">
    /// <item>The dropdown will be populated from the specified Listview elements.</item>
    /// <item>The action will be invoked on mouse up from any drop down element.</item>
    /// </list>
    /// additional search strategies can be added and cycled through on search icon click.
    /// </summary>
    public class DropdownSearchField : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<DropdownSearchField, UxmlTraits>
        {
        }

        public const string DefaultSearchName = "Default";

        private const string UxmlPath = "Uxml/" + nameof(DropdownSearchField) + "/" + nameof(DropdownSearchField);
        private const string UssPath = "Uss/" + nameof(DropdownSearchField) + "/" + nameof(DropdownSearchField);

        private const string SearchFieldName = "toolbar-search-field";

        private const string DefaultSearchIcon = "icon_search";

        private const float BorderRadius = 3;

        private ToolbarSearchField _searchField;
        private DropdownListView _dropdownListView;

        private ListView _boundListView;
        private IList _boundItemsSource;

        private int _listingStrategyIndex;
        private readonly List<ListingStrategy> _listingStrategies = new();

        private VisualElement _icon;

        public DropdownSearchField()
        {
            this.AddHierarchyFromUxml(UxmlPath);
            this.AddStyleFromUss(UssPath);

            SetReferences();
            InitializeDropDown();
            InitializeSearchField();
            SetDefaultSearchBehaviour();
            InitializeSearchListing();
            InitializeFocusAndBlur();

            KeepDropdownUnderSelf(this);
        }

        public void BindToListView(ListView listView)
        {
            _boundListView = listView;
            _boundItemsSource = listView.itemsSource;
        }

        public DropdownSearchField WithResizableParent(VisualElement resizableParent)
        {
            UnregisterCallback<GeometryChangedEvent>(_ => PlaceUnder());
            KeepDropdownUnderSelf(resizableParent);
            return this;
        }

        public DropdownSearchField WithDropdownBehaviour(Action<int> actionAtIndex)
        {
            _searchField.RegisterValueChangedCallback(evt =>
            {
                _dropdownListView.ClearData();
                if (string.IsNullOrEmpty(evt.newValue))
                {
                    HideDropdown();
                    return;
                }

                for (var i = 0; i < _boundItemsSource.Count; i++)
                {
                    if (_boundItemsSource[i] == null) break;
                    var dataName = (_boundItemsSource[i] as IListable).GetName();
                    if (!dataName.ToLower().Contains(evt.newValue.ToLower())) continue;
                    var index = i;
                    _dropdownListView.Add(
                        dataName,
                        () => actionAtIndex.Invoke(index));
                }

                // todo: uncomment when InitializeFocusAndBlur() bug is fixed
                // if (_dropdownListView.Count == 0) return;
                // ShowDropdown();
            });

            return this;
        }

        public DropdownSearchField WithCyclingListingStrategy(ListingStrategy listingStrategy)
        {
            if (_listingStrategies.Count == 1)
            {
                _icon.AddToClassList("hover-behaviour");
            }

            _listingStrategies.Add(listingStrategy);

            this.Q(UiToolkitNames.UnitySearch).RegisterCallback<ClickEvent>(_ =>
            {
                HideDropdown();
                NextListingStrategy();
                if (!string.IsNullOrEmpty(_searchField.value))
                {
                    _boundListView.itemsSource =
                        GetCurrentListingStrategy().Execute(_searchField.value, _boundItemsSource);
                }

                _boundListView.Rebuild();
            });

            return this;
        }

        public void ListingFromStrategy(string strategyName, string listFromValue)
        {
            for (var index = 0; index < _listingStrategies.Count; index++)
            {
                if (strategyName != _listingStrategies[index].Name) continue;

                _listingStrategyIndex = index;
                _icon.style.backgroundImage = new StyleBackground(GetCurrentListingStrategy().Icon);
                ExecuteCurrentListingStrategy(listFromValue);
                _searchField.value = listFromValue;
            }
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

        private void SetReferences()
        {
            _icon = this.Q(UiToolkitNames.UnitySearch);
        }

        private void InitializeSearchField()
        {
            _searchField = this.Q<ToolbarSearchField>(SearchFieldName);
        }

        private void InitializeDropDown()
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

                ExecuteCurrentListingStrategy(evt.newValue);
            });
        }

        private void ExecuteCurrentListingStrategy(string value)
        {
            _boundListView.itemsSource = GetCurrentListingStrategy().Execute(value, _boundItemsSource);
            _boundListView.Rebuild();
        }

        private void SetDefaultSearchBehaviour()
        {
            var icon = Resources.Load<Texture2D>("Sprites/" + DefaultSearchIcon);
            var searchFieldListing = new ListingStrategy(DefaultSearchName, icon, DefaultSearchBehaviour);
            _listingStrategies.Add(searchFieldListing);
        }

        private List<IListable> DefaultSearchBehaviour(string searchFieldValue, IList defaultList)
        {
            List<IListable> cache = new();
            foreach (var element in defaultList)
            {
                var dataName = (element as IListable).GetName();
                if (!dataName.ToLower().Contains(searchFieldValue.ToLower())) continue;
                cache.Add(element as IListable);
            }

            // if (_filteredList.Count == 0) return;
            // ShowDropdown();

            return cache;
        }

        private ListingStrategy GetCurrentListingStrategy()
        {
            return _listingStrategies[_listingStrategyIndex];
        }

        private void NextListingStrategy()
        {
            _listingStrategyIndex++;

            if (_listingStrategyIndex == _listingStrategies.Count)
            {
                _listingStrategyIndex = 0;
            }

            _icon.style.backgroundImage = new StyleBackground(GetCurrentListingStrategy().Icon);
        }

        private void InitializeFocusAndBlur()
        {
            // todo: find out why the focus and blur are both called after selecting an element from the dropdown and then clicking on the search bar
            // _searchField.RegisterCallback<FocusEvent>(_ =>
            // {
            //     if (string.IsNullOrEmpty(_searchField.value)) return;
            //     Debug.LogError("FOCUS");
            //     ShowDropdown();
            // });
            //
            // _searchField.RegisterCallback<BlurEvent>(_ =>
            // {
            //     if (string.IsNullOrEmpty(_searchField.value)) return;
            //     Debug.LogError("Blur");
            //     HideDropdown();
            // });
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