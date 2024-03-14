using System;
using System.Collections.Generic;

namespace Ludwell.Scene.Editor
{
    public interface IViewable
    {
        void Show();
        void Hide();
    }

    public class ViewManager
    {
        private readonly Dictionary<Type, IViewable> _views = new();

        public static ViewManager Instance { get; private set; } = new();

        public IViewable CurrentView { get; private set; }

        public void Add(IViewable view)
        {
            _views.Add(view.GetType(), view);
        }

        public void Remove(IViewable view)
        {
            _views.Remove(view.GetType());
        }

        public void TransitionToFirstViewOfType<T>() where T : IViewable
        {
            if (CurrentView != null && CurrentView.GetType() == typeof(T)) return;

            CurrentView?.Hide();
            CurrentView = _views[typeof(T)];
            CurrentView.Show();
        }
    }
}