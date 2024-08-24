using System;
using UnityEngine;

namespace Ludwell.EditorUtilities.Editor
{
    /// <summary>
    /// A button with tooltip and icon options.<br/>
    /// Must be <see cref="Build"/> to render.
    /// </summary>
    public class EditorButton
    {
        private Rect _rect;
        public const float Size = 16f;

        private readonly Action _behaviour;

        private readonly GUIContent _content;

        private float _iconSize = 8f;
        private Texture2D _icon;

        public EditorButton(Rect position, Action behaviour)
        {
            _rect = new Rect(position.x, position.y, Size, Size);

            _behaviour = behaviour;

            _content = new GUIContent();
        }

        public EditorButton WithIcon(string path, float iconSize = -1)
        {
            _icon = Resources.Load<Texture2D>(path);
            _iconSize = iconSize < 0 ? _iconSize : iconSize;
            return this;
        }

        public EditorButton WithTooltip(string tooltip)
        {
            _content.tooltip = tooltip;
            return this;
        }

        public void Build()
        {
            if (GUI.Button(_rect, _content))
            {
                _behaviour?.Invoke();
            }

            if (_icon)
            {
                var xPosition = _rect.x + (_rect.width - _iconSize) * 0.5f;
                var yPosition = _rect.y + (_rect.height - _iconSize) * 0.5f;
                var textureRect = new Rect(xPosition, yPosition, _iconSize, _iconSize);
                GUI.DrawTexture(textureRect, _icon);
            }
        }
    }
}
