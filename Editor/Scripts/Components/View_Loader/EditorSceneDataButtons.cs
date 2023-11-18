using System;
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
            UxmlColorAttributeDescription m_loadButtonColor = new() { name = "loadIconColor" };
            UxmlColorAttributeDescription m_openButtonColor = new() { name = "openIconColor" };

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                var self = ve as EditorSceneDataButtons;

                self.ButtonSize = m_buttonSize.GetValueFromBag(bag, cc);
                self.LoadButtonColor = m_loadButtonColor.GetValueFromBag(bag, cc);
                self.OpenButtonColor = m_openButtonColor.GetValueFromBag(bag, cc);
            }
        }

        private const string _buttonUxmlPath = "Uxml/" + "Common/" + "button__open-load";
        private const string _buttonUssPath = "Uss/" + "Common/" + "button__open-load";

        private readonly Button _loadButton;
        private readonly Button _openButton;

        private int _buttonSize;
        private Color _loadButtonColor;
        private Color _openButtonColor;

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

        public Color LoadButtonColor
        {
            get => _loadButtonColor;
            set
            {
                _loadButtonColor = value;
                _loadButton.Q<VisualElement>("icon").style.unityBackgroundImageTintColor = new StyleColor(value);
            }
        }

        public Color OpenButtonColor
        {
            get => _openButtonColor;
            set
            {
                _openButtonColor = value;
                _openButton.Q<VisualElement>("icon").style.unityBackgroundImageTintColor = new StyleColor(value);
            }
        }

        public EditorSceneDataButtons()
        {
            this.AddStyleFromUss(_buttonUssPath);
            _loadButton = this.AddHierarchyFromUxml(_buttonUxmlPath).Q<Button>();
            _loadButton.name = "button__load";
            _openButton = this.AddHierarchyFromUxml(_buttonUxmlPath).Q<Button>();
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