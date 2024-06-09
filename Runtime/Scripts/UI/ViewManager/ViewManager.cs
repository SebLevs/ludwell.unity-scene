using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Ludwell.Scene
{
    public class ViewManager : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<ViewManager>
        {
        }

        private readonly Dictionary<Type, AViewable> _views = new();

        private Stack<AViewable> _previousViews = new();
        private AViewable _currentView;

        public void Reset()
        {
            _currentView = null;
            _views.Clear();
        }

        public void Add(AViewable view)
        {
            _views.Add(view.GetType(), view);
        }

        public void Remove(AViewable view)
        {
            _views.Remove(view.GetType());
        }

        public void TransitionToFirstViewOfType<T>(ViewArgs showArgs = null) where T : AViewable
        {
            if (_currentView != null && _currentView.GetType() == typeof(T)) return;

            if (_currentView != null)
            {
                _previousViews.Push(_currentView);
            }

            _currentView?.HideView();
            _currentView = _views[typeof(T)];
            _currentView.ShowView(showArgs);
        }

        public void TransitionToPreviousView(ViewArgs showArgs = null)
        {
            if (_previousViews.Count == 0) return;

            var previousView = _previousViews.Pop();
            _currentView.HideView();
            _currentView = previousView;
            previousView.ShowView(showArgs);
        }
    }
}