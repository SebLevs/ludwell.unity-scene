using System;
using Ludwell.UIToolkitUtilities;
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

        public TextField TextField { get; }

        public string Value => TextField.value;

        public TagsManagerElementView(VisualElement root)
        {
            _root = root;

            var addButton = _root.Q<Button>(AddButtonName);
            addButton.clicked += AddButtonAction;

            var removeButton = _root.Q<Button>(RemoveButtonName);
            removeButton.clicked += RemoveButtonAction;

            TextField = _root.Q<TextField>(TagTextFieldName);
            TextField.RegisterValueChangedCallback(ValueChangedAction);
        }

        public void Dispose()
        {
            var addButton = _root.Q<Button>(AddButtonName);
            addButton.clicked -= AddButtonAction;

            var removeButton = _root.Q<Button>(RemoveButtonName);
            removeButton.clicked -= RemoveButtonAction;

            TextField.UnregisterValueChangedCallback(ValueChangedAction);
        }

        public void SetValue(string value)
        {
            TextField.value = value;
        }

        public void FocusTextFieldWithoutNotify()
        {
            TextField.Focus();
        }

        public void FocusTextField()
        {
            // todo: find a solution to this hack
            TextField.FocusOnNextEditorFrame();
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