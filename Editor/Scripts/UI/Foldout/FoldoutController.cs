using System;
using UnityEngine.UIElements;

namespace Ludwell.Scene.Editor
{
    public class FoldoutController
    {
        public Action<bool> OnHeaderClickedCallback;

        private readonly VisualElement _root;
        private readonly FoldoutView _view;

        private bool _isOpen;

        public Action<string> OnTitleValueChanged => _view.OnTitleValueChanged;

        public bool IsOpen
        {
            get => _isOpen;
            set
            {
                _isOpen = value;
                _view.SetContentVisibility(_isOpen);
            }
        }

        public string Title
        {
            get => _view.Title.value;
            set
            {
                if (value == Title) return;
                _view.Title.value = value;
            }
        }

        public void FocusTextField() => _view.FocusTextField();

        public TextField TitleTextField => _view.Title;

        public void SetOnPreventHeaderClick(Func<IEventHandler, bool> condition) =>
            _view.OnPreventHeaderClick = condition;

        public FoldoutController(VisualElement root, bool startOpen)
        {
            _root = root;
            _view = new FoldoutView(root);

            OnHeaderClickedCallback += _view.ToggleFoldoutStyle;
            _view.OnHeaderClicked += ToggleContentVisibility;

            IsOpen = startOpen;
            _view.ToggleFoldoutStyle(startOpen);
            
            _root.RegisterCallback<DetachFromPanelEvent>(Dispose);
        }

        private void ToggleContentVisibility()
        {
            IsOpen = !IsOpen;
            ExecuteHeaderClickedCallback();
        }

        private void ExecuteHeaderClickedCallback()
        {
            OnHeaderClickedCallback?.Invoke(IsOpen);
        }

        private void Dispose(DetachFromPanelEvent _)
        {
            _root.UnregisterCallback<DetachFromPanelEvent>(Dispose);
            
            OnHeaderClickedCallback -= _view.ToggleFoldoutStyle;
            _view.OnHeaderClicked -= ToggleContentVisibility;
        }
    }
}