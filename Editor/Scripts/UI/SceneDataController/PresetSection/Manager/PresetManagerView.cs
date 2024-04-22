using System;
using UnityEngine.UIElements;

namespace Ludwell.Scene.Editor
{
    public class PresetManagerView
    {
        private const string ReferenceName = "reference-name";
        private const string ReturnButtonName = "button__return";

        private VisualElement _root;

        private readonly Label _referenceName;
        private readonly Button _returnButton;
        private readonly Action _onReturnButtonClicked;

        public PresetManagerView(VisualElement root, Action onReturnButtonClicked)
        {
            _root = root;

            _referenceName = _root.Q<Label>(ReferenceName);

            _returnButton = _root.Q<Button>(ReturnButtonName);
            _onReturnButtonClicked = onReturnButtonClicked;
            _returnButton.clicked += ExecuteButtonClicked;
        }

        ~PresetManagerView()
        {
            _returnButton.clicked -= ExecuteButtonClicked;
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

        private void ExecuteButtonClicked()
        {
            _onReturnButtonClicked?.Invoke();
        }
    }
}