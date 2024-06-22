using System;
using UnityEngine.UIElements;

namespace Ludwell.Scene.Editor
{
    public class FoldoutController
    {
        public Action<bool> OnHeaderClickedCallback;

        private FoldoutView _view;

        private bool _isOpen;

        public Action<ChangeEvent<string>> OnTitleValueChanged
        {
            get => _view.OnTitleValueChanged;
            set => _view.OnTitleValueChanged += value;
        }

        public bool IsOpen
        {
            get => _isOpen;
            set
            {
                _isOpen = value;
                _view.SetContentVisibility(_isOpen);
            }
        }

        public void FocusTextField() => _view.FocusTextField();

        public string Title
        {
            get => _view.Title.value;
            set
            {
                if (value == Title) return;
                _view.Title.value = value;
            }
        }

        public FoldoutController(VisualElement root, bool startOpen)
        {
            _view = new FoldoutView(root);

            OnHeaderClickedCallback += _view.ToggleFoldoutStyle;
            _view.OnHeaderClicked += ToggleContentVisibility;

            SetIsOpen(startOpen);
            _view.ToggleFoldoutStyle(startOpen);
        }

        public void SetIsOpen(bool state)
        {
            IsOpen = state;
            _view.SetContentVisibility(IsOpen);
        }

        private void ToggleContentVisibility()
        {
            SetIsOpen(!IsOpen);
            ExecuteHeaderClickedCallback();
        }

        private void ExecuteHeaderClickedCallback()
        {
            OnHeaderClickedCallback?.Invoke(IsOpen);
        }
    }
}