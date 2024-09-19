using System;
using UnityEngine.UIElements;

namespace Ludwell.SceneManagerToolkit.Editor
{
    internal class ListFooterController
    {
        private readonly ListFooterView _view;
        
        public ListFooterController(VisualElement root)
        {
            _view = new ListFooterView(root);
        }

        public void SubscribeToAddButtonClicked(Action callback)
        {
            _view.AddButton.clicked += callback;
        }
        
        public void UnsubscribeFromAddButtonClicked(Action callback)
        {
            _view.AddButton.clicked -= callback;
        }
        
        public void SubscribeToRemoveButtonClicked(Action callback)
        {
            _view.RemoveButton.clicked += callback;
        }
        
        public void UnsubscribeFromRemoveButtonClicked(Action callback)
        {
            _view.RemoveButton.clicked -= callback;
        }
    }
}
