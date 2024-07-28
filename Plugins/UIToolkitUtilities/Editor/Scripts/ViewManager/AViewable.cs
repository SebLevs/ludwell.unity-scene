using System;
using UnityEngine.UIElements;

namespace Ludwell.UIToolkitUtilities.Editor
{
    public abstract class AViewable
    {
        public Action OnShow;
        public Action OnHide;

        private ViewManager _manager;

        protected AViewable(VisualElement root)
        {
            _manager = root.Root().Q<ViewManager>();
            _manager.Add(this);
        }

        public void ShowView(ViewArgs args)
        {
            OnShow?.Invoke();
            Show(args);
        }

        public void HideView()
        {
            OnHide?.Invoke();
            Hide();
        }

        protected abstract void Show(ViewArgs args);

        protected abstract void Hide();

        protected void ReturnToPreviousView()
        {
            _manager.TransitionToPreviousView();
        }
    }
}