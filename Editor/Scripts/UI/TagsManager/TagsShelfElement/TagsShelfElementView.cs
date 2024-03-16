using System.IO;
using Ludwell.Scene.Editor;
using UnityEngine.UIElements;

namespace Ludwell.Scene
{
    public class TagsShelfElementView : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<TagsShelfElementView, UxmlTraits>
        {
        }

        private static readonly string UxmlPath =
            Path.Combine("Uxml", nameof(TagsManagerView), nameof(TagsShelfElementView));

        private static readonly string UssPath =
            Path.Combine("Uss", nameof(TagsManagerView), nameof(TagsShelfElementView));

        private const string RemoveButtonName = "button-remove";
        private const string MainButtonName = "button-main";
        private const string SearchButtonName = "button-search";

        private static TagsShelfElementView _currentSelection;

        private Button _removeButton;
        private Button _mainButton;
        private Button _searchButton;

        private TagsShelfElementController _controller;

        public string Value => _mainButton.text;

        public TagsShelfElementView()
        {
            this.AddHierarchyFromUxml(UxmlPath);
            this.AddStyleFromUss(UssPath);

            SetReferences();
            SetButtonEvents();

            ToggleBehaviourButtons(DisplayStyle.None);
        }

        ~TagsShelfElementView()
        {
            _controller.RemoveValueChangedCallback(SetValue);
            RemoveButtonEvents();
        }
        
        public void SetTagShelfController(TagsShelfController tagsShelfController)
        {
            _controller.SetTagShelfController(tagsShelfController);
        }

        public void SetValue(string text)
        {
            _mainButton.text = text;
        }

        public void UpdateCache(Tag tag)
        {
            _controller.RemoveValueChangedCallback(SetValue);
            _controller.UpdateTag(tag);
            _controller.SetValue(this);
            _controller.AddValueChangedCallback(SetValue);
        }

        private void SetReferences()
        {
            _removeButton = this.Q<Button>(RemoveButtonName);
            _mainButton = this.Q<Button>(MainButtonName);
            _searchButton = this.Q<Button>(SearchButtonName);

            _controller = new TagsShelfElementController(this);
        }

        private void SetButtonEvents()
        {
            _mainButton.clicked += SelectSelf;

            _removeButton.clicked += _controller.RemoveFromController;

            _searchButton.clicked += _controller.SearchWithData;
            _searchButton.clicked += ToggleVisual;
        }

        private void RemoveButtonEvents()
        {
            _mainButton.clicked -= SelectSelf;
            
            _removeButton.clicked -= _controller.RemoveFromController;

            _searchButton.clicked -= _controller.SearchWithData;
            _searchButton.clicked -= ToggleVisual;
        }

        private void SelectSelf()
        {
            if (_currentSelection == this)
            {
                ToggleVisual();
                return;
            }

            _currentSelection?.ToggleBehaviourButtons(DisplayStyle.None);
            _currentSelection = this;
            _currentSelection?.ToggleBehaviourButtons(DisplayStyle.Flex);
        }

        private void ToggleVisual()
        {
            var reverseDisplay = GetReverseDisplayStyle();
            _currentSelection.ToggleBehaviourButtons(reverseDisplay);
        }

        private DisplayStyle GetReverseDisplayStyle()
        {
            return _currentSelection._removeButton.style.display == DisplayStyle.None
                ? DisplayStyle.Flex
                : DisplayStyle.None;
        }

        private void ToggleBehaviourButtons(DisplayStyle displayStyle)
        {
            _removeButton.style.display = displayStyle;
            _searchButton.style.display = displayStyle;
        }
    }
}