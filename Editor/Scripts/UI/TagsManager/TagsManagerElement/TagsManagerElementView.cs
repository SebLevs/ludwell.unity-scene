using System;
using UnityEngine.UIElements;

namespace Ludwell.Scene.Editor
{
    public class TagsManagerElementView
    {
        private const string AddButtonName = "button__add";
        private const string RemoveButtonName = "button__remove";

        private const string TagTextFieldName = "tag";

        private readonly VisualElement _root;

        public Action OnAdd;
        public Action OnRemove;
        public Action<string> OnValueChanged;

        private readonly TextField _textField;

        public string Value => _textField.value;

        public TagsManagerElementView(VisualElement root)
        {
            _root = root;

            var addButton = _root.Q<Button>(AddButtonName);
            addButton.clicked += AddButtonAction;

            var removeButton = _root.Q<Button>(RemoveButtonName);
            removeButton.clicked += RemoveButtonAction;

            _textField = _root.Q<TextField>(TagTextFieldName);
            _textField.RegisterValueChangedCallback(ValueChangedAction);
        }

        ~TagsManagerElementView()
        {
            var addButton = _root.Q<Button>(AddButtonName);
            addButton.clicked -= AddButtonAction;

            var removeButton = _root.Q<Button>(RemoveButtonName);
            removeButton.clicked -= RemoveButtonAction;

            _textField.UnregisterValueChangedCallback(ValueChangedAction);
        }

        public void SetValue(string value)
        {
            _textField.value = value;
        }

        public void FocusTextField()
        {
            _textField.Focus();
        }

        private void AddButtonAction()
        {
            OnAdd?.Invoke();
        }

        private void RemoveButtonAction()
        {
            OnRemove?.Invoke();
        }

        private void ValueChangedAction(ChangeEvent<string> evt)
        {
            OnValueChanged?.Invoke(evt.newValue);
        }
    }
}