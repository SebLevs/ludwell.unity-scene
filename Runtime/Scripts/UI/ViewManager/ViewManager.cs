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
        
        private readonly Dictionary<Type, IViewable> _views = new();

        private Stack<IViewable> _previousViews = new();
        private IViewable _currentView;

        public void Reset()
        {
            _currentView = null;
            _views.Clear();
        }

        public void Add(IViewable view)
        {
            _views.Add(view.GetType(), view);
        }

        public void Remove(IViewable view)
        {
            _views.Remove(view.GetType());
        }

        public void TransitionToFirstViewOfType<T>(ViewArgs showArgs = null) where T : IViewable
        {
            if (_currentView != null && _currentView.GetType() == typeof(T)) return;

            if (_currentView != null)
            { 
                _previousViews.Push(_currentView);
            }
            
            _currentView?.Hide();
            _currentView = _views[typeof(T)];
            _currentView.Show(showArgs);
        }

        public void TransitionToPreviousView(ViewArgs showArgs = null)
        {
            if (_previousViews.Count == 0) return;

            var previousView = _previousViews.Pop();
            _currentView.Hide();
            _currentView = previousView;
            previousView.Show(showArgs);
        }
    }
}