using System;
using UnityEngine.UIElements;

namespace Ludwell.Scene.Editor
{
    public class PresetManagerView
    {
        private const string ReferenceName = "reference-name";
        private readonly Label _referenceName;
        
        private const string ReturnButtonName = "button__return";
        private readonly Button _returnButton;
        private readonly Action _onReturnButtonClicked;
        
        private const string AddButtonName = "add";
        private readonly Button _addButton;
        
        private const string RemoveButtonName = "remove";
        private readonly Button _removeButton;

        private VisualElement _root;

        public PresetManagerView(VisualElement root, Action onReturnButtonClicked)
        {
            _root = root;

            _referenceName = _root.Q<Label>(ReferenceName);

            _returnButton = _root.Q<Button>(ReturnButtonName);
            _onReturnButtonClicked = onReturnButtonClicked;
            _returnButton.clicked += ExecuteReturnButtonClicked;

            _addButton = _root.Q<Button>(AddButtonName);
            _addButton.clicked += ExecuteAddButtonClicked;

            _removeButton = _root.Q<Button>(RemoveButtonName);
            _removeButton.clicked += ExecuteRemoveButtonClicked;
        }

        ~PresetManagerView()
        {
            _returnButton.clicked -= ExecuteReturnButtonClicked;
        }

        public void Show()
        {
            _root.style.display = DisplayStyle.Flex;
        }

        public void Hide()
        {
            _root.style.display = DisplayStyle.None;
        }

        public void SetReferenceText(string value)
        {
            _referenceName.text = value;
        }

        private void ExecuteReturnButtonClicked()
        {
            _onReturnButtonClicked?.Invoke();
        }
        
        private void ExecuteAddButtonClicked() // todo: remove?
        {
            // _listViewHandler.ListView.itemsSource.Add();
        }
        
        private void ExecuteRemoveButtonClicked() // todo: remove?
        {
            _onReturnButtonClicked?.Invoke();
        }
    }
}