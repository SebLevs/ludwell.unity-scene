using System;
using UnityEngine;

namespace Ludwell.Scene
{
    public class DualStateButtonState
    {
        public Action OnClick;

        private DualStateButton _context;
        private Sprite _icon;

        public DualStateButtonState(DualStateButton context, Action onClick, Sprite icon = null)
        {
            _context = context;
            OnClick = onClick;
            _icon = icon;
        }

        public void Enter()
        {
            _context.clicked += OnClick;
            _context.SetIcon(_icon);
        }

        public void Exit()
        {
            _context.clicked -= OnClick;
        }
    }
}
