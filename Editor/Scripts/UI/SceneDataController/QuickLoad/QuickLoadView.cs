using System;
using Ludwell.Scene.Editor;
using UnityEngine.UIElements;

namespace Ludwell.Scene
{
    public class QuickLoadView
    {
        private const string ButtonAddName = "add";
        private const string ButtonRemoveName = "remove";

        private readonly VisualElement _root;

        private readonly Action _onCloseAll;
        private readonly Action _onAdd;
        private readonly Action _onRemove;

        public QuickLoadView(VisualElement root, Action onCloseAll, Action onAdd, Action onRemove)
        {
            _root = root;

            _onCloseAll = onCloseAll;
            _onAdd = onAdd;
            _onRemove = onRemove;

            InitializeButtonCloseAll();
            InitializeAddRemoveButtons();
        }

        ~QuickLoadView()
        {
            ClearAddRemoveButtons();
        }

        private void InitializeButtonCloseAll()
        {
            var closeAllButton = _root.Q<Button>();
            closeAllButton.clicked += _onCloseAll.Invoke;
        }

        private void InitializeAddRemoveButtons()
        {
            _root.Q<Button>(ButtonAddName).clicked += _onAdd.Invoke;
            _root.Q<Button>(ButtonRemoveName).clicked += _onRemove.Invoke;
        }

        private void ClearAddRemoveButtons()
        {
            _root.Q<Button>(ButtonAddName).clicked -= _onAdd.Invoke;
            _root.Q<Button>(ButtonRemoveName).clicked -= _onRemove.Invoke;
        }
    }
}