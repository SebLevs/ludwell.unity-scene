using System;
using UnityEngine.UIElements;

namespace Ludwell.Scene.Editor
{
    internal class TagsShelfElementView : IDisposable
    {
        public Action OnRemoveButtonClicked;
        public Action OnMainButtonClicked;
        public Action OnSearchButtonClicked;

        private const string RemoveButtonName = "button-remove";
        private const string MainButtonName = "button-main";
        private const string SearchButtonName = "button-search";

        private readonly Button _removeButton;
        private readonly Button _mainButton;
        private readonly Button _searchButton;

        private readonly VisualElement _view;

        public string Value => _mainButton.text;

        public TagsShelfElementView(VisualElement view)
        {
            _view = view;

            _removeButton = _view.Q<Button>(RemoveButtonName);
            _removeButton.clicked += RemoveButtonAction;

            _mainButton = _view.Q<Button>(MainButtonName);
            _mainButton.clicked += MainButtonAction;

            _searchButton = _view.Q<Button>(SearchButtonName);
            _searchButton.clicked += SearchButtonAction;
        }

        public void Dispose()
        {
            _removeButton.clicked -= RemoveButtonAction;
            _mainButton.clicked -= MainButtonAction;
            _searchButton.clicked -= SearchButtonAction;
        }

        public void SetValue(string text)
        {
            _mainButton.text = text;
        }

        public void SetButtonsStyle(DisplayStyle displayStyle)
        {
            _removeButton.style.display = displayStyle;
            _searchButton.style.display = displayStyle;
        }

        public void ToggleVisual()
        {
            var reverseDisplay = GetReverseDisplayStyle();
            SetButtonsStyle(reverseDisplay);
        }

        private DisplayStyle GetReverseDisplayStyle()
        {
            return _removeButton.style.display == DisplayStyle.None
                ? DisplayStyle.Flex
                : DisplayStyle.None;
        }

        private void RemoveButtonAction()
        {
            OnRemoveButtonClicked?.Invoke();
        }

        private void MainButtonAction()
        {
            OnMainButtonClicked?.Invoke();
        }

        private void SearchButtonAction()
        {
            OnSearchButtonClicked?.Invoke();
        }
    }
}
