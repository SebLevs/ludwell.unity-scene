using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ludwell.Scene.Editor
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

        private static readonly string UxmlPath =
            Path.Combine("UI", nameof(DropdownSearchField), "Uxml_" + nameof(DropdownSearchField));

        private static readonly string UssPath =
            Path.Combine("UI", nameof(DropdownSearchField), "Uss_" + nameof(DropdownSearchField));

        private const string CyclingIconName = "icon__behaviour-cycling";
        private const string SearchFieldName = "toolbar-search-field";

        private const string DefaultSearchIcon = "icon_search";

        private const float BorderRadius = 3;

        private ToolbarSearchField _searchField;
        private DropdownListView _dropdownListView;

        private IList _baseItemsSource;
        private ListView _listView;

        private int _listingStrategyIndex;
        private readonly List<ListingStrategy> _listingStrategies = new();

        private VisualElement _searchIcon;

        public static string DefaultSearchName => "name";

        private bool IsListing => !string.IsNullOrEmpty(_searchField.value);

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
            
            RegisterCallback<DetachFromPanelEvent>(Dispose);
        }

        public bool HasSearchStrategy(string strategy)
        {
            foreach (var listingStrategy in _listingStrategies)
            {
                if (listingStrategy.Name == strategy) return true;
            }

            return false;
        }

        public void BindToListView(ListView listView)
        {
            _baseItemsSource = listView.itemsSource;

            if (_listView != null)
            {
                _listView.itemsAdded -= AddToBaseItemsSource;
                _listView.itemsRemoved -= RemoveFromBaseItemsSource;
            }

            _listView = listView;

            _listView.itemsAdded += AddToBaseItemsSource;
            _listView.itemsRemoved += RemoveFromBaseItemsSource;
        }

        public DropdownSearchField WithResizableParent(VisualElement resizableParent)
        {
            UnregisterCallback<GeometryChangedEvent>(PlaceUnder);
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

                for (var i = 0; i < _baseItemsSource.Count; i++)
                {
                    if (_baseItemsSource[i] == null) break;
                    var dataName = (_baseItemsSource[i] as IListable).Name;
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
                _searchIcon.AddToClassList("hover-behaviour");
                this.Q<VisualElement>(CyclingIconName).style.display = DisplayStyle.Flex;
            }

            _listingStrategies.Add(listingStrategy);

            this.Q(UiToolkitNames.UnitySearch).RegisterCallback<ClickEvent>(_ =>
            {
                HideDropdown();
                var newListingStrategy = NextListingStrategy();

                if (!string.IsNullOrEmpty(_searchField.value) || newListingStrategy.IsSearchEmptyString)
                {
                    _listView.itemsSource = newListingStrategy.Execute(_searchField.value, _baseItemsSource);
                }
                else if (!Equals(_listView.itemsSource, _baseItemsSource))
                {
                    _listView.itemsSource = _baseItemsSource;
                }

                _listView.Rebuild();
            });

            return this;
        }

        public void ListWithStrategy(string strategyName, string listFromValue)
        {
            for (var index = 0; index < _listingStrategies.Count; index++)
            {
                if (strategyName != _listingStrategies[index].Name) continue;

                _listingStrategyIndex = index;
                _searchIcon.style.backgroundImage = new StyleBackground(GetCurrentListingStrategy().Icon);
                ExecuteCurrentListingStrategy(listFromValue);
                _searchField.value = listFromValue;
            }
        }

        /// <returns>Was the current listing strategy executed</returns>
        public bool RebuildActiveListing()
        {
            if (!GetCurrentListingStrategy().IsSearchEmptyString && !IsListing) return false;
            
            ExecuteCurrentListingStrategy(_searchField.value);
            _listView.Rebuild();
            return true;
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
            _searchIcon = this.Q(UiToolkitNames.UnitySearch);
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
                rootVisualContainer?.Add(_dropdownListView);

                _dropdownListView.Hide();
            });
        }

        private void InitializeSearchListing()
        {
            _searchField.RegisterValueChangedCallback(evt =>
            {
                if (string.IsNullOrEmpty(evt.newValue) && !GetCurrentListingStrategy().IsSearchEmptyString)
                {
                    _listView.itemsSource = _baseItemsSource;
                    _listView.Rebuild();
                    return;
                }

                ExecuteCurrentListingStrategy(evt.newValue);
            });
        }

        private void ExecuteCurrentListingStrategy(string value)
        {
            _listView.itemsSource = GetCurrentListingStrategy().Execute(value, _baseItemsSource);
            _listView.Rebuild();
        }

        private void SetDefaultSearchBehaviour()
        {
            var icon = Resources.Load<Texture2D>("Sprites/" + DefaultSearchIcon);
            var searchFieldListing = new ListingStrategy(DefaultSearchName, icon, DefaultSearchBehaviour);
            _listingStrategies.Add(searchFieldListing);
            this.Q<VisualElement>(CyclingIconName).style.display = DisplayStyle.None;
            _searchIcon.tooltip = "Search by " + DefaultSearchName;
        }

        private List<IListable> DefaultSearchBehaviour(string searchFieldValue, IList defaultList)
        {
            List<IListable> cache = new();
            foreach (var element in defaultList)
            {
                var dataName = (element as IListable).Name;
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

        private ListingStrategy NextListingStrategy()
        {
            _listingStrategyIndex++;

            if (_listingStrategyIndex == _listingStrategies.Count)
            {
                _listingStrategyIndex = 0;
            }

            var currentStrategy = GetCurrentListingStrategy();
            _searchIcon.style.backgroundImage = new StyleBackground(currentStrategy.Icon);
            _searchIcon.tooltip = "Search by " + currentStrategy.Name;
            return currentStrategy;
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
            resizableElement.RegisterCallback<GeometryChangedEvent>(PlaceUnder);
        }

        private void AddToBaseItemsSource(IEnumerable<int> integers)
        {
            if (!IsListing) return;

            foreach (var integer in integers)
            {
                _baseItemsSource.Add(_listView.itemsSource[integer]);
            }
        }

        private void RemoveFromBaseItemsSource(IEnumerable<int> integers)
        {
            if (!IsListing) return;
            foreach (var integer in integers)
            {
                _baseItemsSource.Remove(_listView.itemsSource[integer]);
            }
        }

        private void PlaceUnder(GeometryChangedEvent evt)
        {
            if (_dropdownListView.IsHidden) return;
            _dropdownListView.PlaceUnder(this);
        }

        private void SetBottomBorderRadii(float radius)
        {
            _searchField.style.borderBottomLeftRadius = radius;
            _searchField.style.borderBottomRightRadius = radius;
        }
        
        private void Dispose(DetachFromPanelEvent evt)
        {
            UnregisterCallback<DetachFromPanelEvent>(Dispose);
            
            UnregisterCallback<GeometryChangedEvent>(PlaceUnder);

            if (_listView == null) return;
            _listView.itemsAdded -= AddToBaseItemsSource;
            _listView.itemsRemoved -= RemoveFromBaseItemsSource;
        }
    }
}