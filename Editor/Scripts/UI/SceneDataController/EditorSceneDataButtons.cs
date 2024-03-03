using System;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ludwell.Scene
{
    public enum ButtonType
    {
        Load,
        Open
    }

    public class EditorSceneDataButtons : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<EditorSceneDataButtons, UxmlTraits> { }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            UxmlIntAttributeDescription m_buttonSize = new() { name = "buttonSize", defaultValue = 32};
            UxmlColorAttributeDescription m_loadIconColor = new() { name = "loadIconColor" };
            UxmlColorAttributeDescription m_openIconColor = new() { name = "openIconColor" };

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                var self = ve as EditorSceneDataButtons;

                self.ButtonSize = m_buttonSize.GetValueFromBag(bag, cc);
                self.LoadIconColor = m_loadIconColor.GetValueFromBag(bag, cc);
                self.OpenIconColor = m_openIconColor.GetValueFromBag(bag, cc);
            }
        }

        private static readonly string ButtonUxmlPath = Path.Combine("Uxml", "Common", "button__open-load");
        private static readonly string ButtonUssPath = Path.Combine("Uss", "Common", "button__open-load");

        private readonly Button _loadButton;
        private readonly Button _openButton;

        private int _buttonSize;
        private Color _loadIconColor;
        private Color _openIconColor;

        public int ButtonSize
        {
            get => _buttonSize;
            set
            {
                _buttonSize = value;
                _loadButton.style.width = _buttonSize;
                _loadButton.style.height = _buttonSize;
                _openButton.style.width = _buttonSize;
                _openButton.style.height = _buttonSize;
            }
        }

        public Color LoadIconColor
        {
            get => _loadIconColor;
            set
            {
                _loadIconColor = value;
                _loadButton.Q<VisualElement>("icon").style.unityBackgroundImageTintColor = new StyleColor(value);
            }
        }

        public Color OpenIconColor
        {
            get => _openIconColor;
            set
            {
                _openIconColor = value;
                _openButton.Q<VisualElement>("icon").style.unityBackgroundImageTintColor = new StyleColor(value);
            }
        }

        public EditorSceneDataButtons()
        {
            this.AddStyleFromUss(ButtonUssPath);
            _loadButton = this.AddHierarchyFromUxml(ButtonUxmlPath).Q<Button>();
            _loadButton.name = "button__load";
            _openButton = this.AddHierarchyFromUxml(ButtonUxmlPath).Q<Button>();
            _openButton.name = "button__open";
        }

        public void AddAction(ButtonType button, Action action)
        {
            if (button == ButtonType.Load) _loadButton.clicked += action;
            else _openButton.clicked += action;
        }

        public void RemoveAction(ButtonType button, Action action)
        {
            if (button == ButtonType.Load) _loadButton.clicked -= action;
            else _openButton.clicked -= action;
        }
    }
}