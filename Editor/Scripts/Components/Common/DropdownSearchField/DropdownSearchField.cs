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

        private const string DefaultSearchIcon = "search-icon";

        private const float BorderRadius = 3;

        private ToolbarSearchField _searchField;
        private DropdownListView _dropdownListView;

        private ListView _boundListView;
        private IList _boundItemsSource;
        
        private int _searchBehaviourIndex;
        private readonly List<(Texture2D, Func<string, IList, List<ISearchFieldListable>>)> _searchBehaviours = new();

        private VisualElement _icon;

        public DropdownSearchField()
        {
            this.AddHierarchyFromUxml(UxmlPath);
            this.AddStyleFromUss(UssPath);

            SetReferences();
            InitDropDown();
            InitSearchField();
            InitializeSearchListing();
            InitializeFocusAndBlur();

            KeepDropdownUnderSelf(this);
        }

        private void SetReferences()
        {
            _icon = this.Q(UiToolkitNames.UnitySearch);
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
                    var dataName = (_boundItemsSource[i] as ISearchFieldListable).GetName();
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
        
        /// <param name="behaviour">
        /// <list type="bullet">
        /// <item>string = searchField value</item>
        /// <item>IList = complete default list</item>
        /// </list>
        /// </param>
        public DropdownSearchField WithCyclingSearchBehaviour(Texture2D icon, Func<string, IList, List<ISearchFieldListable>> behaviour)
        {
            _searchBehaviours.Add((icon, behaviour));

            this.Q(UiToolkitNames.UnitySearch).RegisterCallback<ClickEvent>(_ =>
            {
                Debug.LogError("CLICK");
                HideDropdown();
                CycleThroughSearchBehaviour();
                _boundListView.itemsSource = GetCurrentSearchBehaviour().Item2.Invoke(_searchField.value, _boundItemsSource);
                _boundListView.Rebuild();
            });

            return this;
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
            var searchIcon = Resources.Load<Texture2D>("Sprites/" + DefaultSearchIcon);
            _searchBehaviours.Add((searchIcon, DefaultSearchBehaviour));

            _searchField.RegisterValueChangedCallback(evt =>
            {
                if (string.IsNullOrEmpty(evt.newValue))
                {
                    _boundListView.itemsSource = _boundItemsSource;
                    _boundListView.Rebuild();
                    return;
                }

                _boundListView.itemsSource = GetCurrentSearchBehaviour().Item2.Invoke(evt.newValue, _boundItemsSource);
                _boundListView.Rebuild();
            });
        }

        private List<ISearchFieldListable> DefaultSearchBehaviour(string searchFieldValue, IList defaultList)
        {
            Debug.LogError(nameof(DefaultSearchBehaviour));

            List<ISearchFieldListable> cache = new();
            foreach (var element in defaultList)
            {
                var dataName = (element as ISearchFieldListable).GetName();
                if (!dataName.ToLower().Contains(searchFieldValue.ToLower())) continue;
                cache.Add(element as ISearchFieldListable);
            }

            // if (_filteredList.Count == 0) return;
            // ShowDropdown();

            return cache;
        }
        
        private (Texture2D, Func<string, IList, List<ISearchFieldListable>>) GetCurrentSearchBehaviour()
        {
            return _searchBehaviours[_searchBehaviourIndex];
        }

        private void CycleThroughSearchBehaviour()
        {
            _searchBehaviourIndex++;
            
            if (_searchBehaviourIndex == _searchBehaviours.Count)
            {
                _searchBehaviourIndex = 0;
            }

            _icon.style.backgroundImage = new StyleBackground(GetCurrentSearchBehaviour().Item1);
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