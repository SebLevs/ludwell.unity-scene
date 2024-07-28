using System;
using UnityEngine;

namespace Ludwell.UIToolkitElements.Editor
{
    public class DualStateButtonState
    {
        public Action OnClick;

        private DualStateButton _context;
        private Sprite _icon;

        public DualStateButtonState(DualStateButton context, Sprite icon, Action onClick = null)
        {
            _context = context;
            _icon = icon;
            if (onClick == null) return;
            OnClick = onClick;
        }

        public void Enter()
        {
            _context.SetIcon(_icon);
            _context.clicked += OnClick;
        }

        public void Exit()
        {
            _context.clicked -= OnClick;
        }
    }
}
