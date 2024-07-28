using System.IO;
using Ludwell.UIToolkitUtilities;
using UnityEngine.UIElements;

namespace Ludwell.Scene.Editor
{
    public class TagsShelfElementController : VisualElement
    {
        private static readonly string UxmlPath =
            Path.Combine("UI", nameof(TagsShelfElementView), "Uxml_" + nameof(TagsShelfElementView));

        private static readonly string UssPath =
            Path.Combine("UI", nameof(TagsShelfElementView), "Uss_" + nameof(TagsShelfElementView));

        private static TagsShelfElementView _currentSelection;

        private readonly TagsShelfElementView _view;

        private Tag _model;

        private string _listingStrategyName = "tag";
        private DropdownSearchField _dropdownSearchField;

        private TagsShelfController _tagShelfController;

        public string Value => _view.Value;

        public TagsShelfElementController()
        {
            this.AddHierarchyFromUxml(UxmlPath);
            this.AddStyleFromUss(UssPath);

            _view = new TagsShelfElementView(this);
            _view.OnRemoveButtonClicked += RemoveFromController;
            _view.OnMainButtonClicked += SelectSelf;
            _view.OnSearchButtonClicked += SearchWithData;
            _view.OnSearchButtonClicked += _view.ToggleVisual;
            _view.SetButtonsStyle(DisplayStyle.None);

            RegisterCallback<AttachToPanelEvent>(InitializeDropdown);

            RegisterCallback<DetachFromPanelEvent>(Dispose);
        }

        public void SetTagShelfController(TagsShelfController tagsShelfController)
        {
            _tagShelfController = tagsShelfController;
        }

        public void UpdateCache(Tag tag)
        {
            _model = tag;
            _view.SetValue(_model.ID);
        }

        private void RemoveFromController()
        {
            _tagShelfController.Remove(_model);
        }

        private void SelectSelf()
        {
            if (_currentSelection == _view)
            {
                _currentSelection.ToggleVisual();
                return;
            }

            _currentSelection?.SetButtonsStyle(DisplayStyle.None);
            _currentSelection = _view;
            _currentSelection.SetButtonsStyle(DisplayStyle.Flex);
        }

        private void InitializeDropdown(AttachToPanelEvent _)
        {
            _dropdownSearchField = this.FindInAncestors<DropdownSearchField>();
            _listingStrategyName = _dropdownSearchField.HasSearchStrategy(_listingStrategyName)
                ? _listingStrategyName
                : DropdownSearchField.DefaultSearchName;
        }

        private void SearchWithData()
        {
            _dropdownSearchField.ListWithStrategy(_listingStrategyName, _model.ID);
        }
        
        private void Dispose(DetachFromPanelEvent _)
        {
            UnregisterCallback<DetachFromPanelEvent>(Dispose);

            _currentSelection = null;

            _view.OnRemoveButtonClicked -= RemoveFromController;
            _view.OnMainButtonClicked -= SelectSelf;
            _view.OnSearchButtonClicked -= SearchWithData;
            _view.OnSearchButtonClicked -= _view.ToggleVisual;

            UnregisterCallback<AttachToPanelEvent>(InitializeDropdown);
        }
    }
}