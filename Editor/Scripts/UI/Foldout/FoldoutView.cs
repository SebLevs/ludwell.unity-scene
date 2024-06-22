using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ludwell.Scene.Editor
{
    public class FoldoutView
    {
        public static readonly string HeaderName = "header";
        public static readonly string ToggleIconName = "toggle-icon";
        public static readonly string TitleName = "title";
        public static readonly string ContentName = "content";
        public static string FooterName = "footer";

        public Action<string> OnTitleValueChanged;
        public Action OnHeaderClicked;
        
        public TextField Title;
        
        private const string IconRotationClosedClassName = "icon-rotation__closed";
        private const string IconRotationOpenedClassName = "icon-rotation__opened";
        
        private const string TextFieldUnselectedClass = "scene-data__unselected";
        private const string TextFieldSelectedClass = "scene-data__selected";
        
        private readonly VisualElement _root;

        private readonly VisualElement _header;
        private readonly VisualElement _icon;
        private readonly VisualElement _content;

        private void ExecuteTitleValueChangedCallback(ChangeEvent<string> evt) => OnTitleValueChanged?.Invoke(evt.newValue);
        
        private void ExecuteHeaderClickedCallback(ClickEvent evt) => OnHeaderClicked?.Invoke();
        
        public FoldoutView(VisualElement root)
        {
            _root = root;

            _header = _root.Q<VisualElement>(HeaderName);
            _header.RegisterCallback<ClickEvent>(ExecuteHeaderClickedCallback);
            
            
            _root.RegisterCallback<KeyDownEvent>(_ =>
            {
                    Debug.LogError("space");
                if (_.keyCode == KeyCode.Space)
                {
                    ExecuteHeaderClickedCallback(null);
                }
            });
            
            _icon = _root.Q<VisualElement>(ToggleIconName);
            
            Title = _root.Q<TextField>(TitleName);
            Title.RegisterValueChangedCallback(ExecuteTitleValueChangedCallback);
            Title.RegisterCallback<ClickEvent>(_ =>
            {
                Title.RemoveFromClassList(TextFieldUnselectedClass);
                Title.AddToClassList(TextFieldSelectedClass);
            });
            Title.RegisterCallback<BlurEvent>(_ =>
            {
                Title.RemoveFromClassList(TextFieldSelectedClass);
                Title.AddToClassList(TextFieldUnselectedClass);
            });
            Title.RegisterCallback<ClickEvent>(evt => evt.StopPropagation());
            
            _content = _root.Q<VisualElement>(ContentName);
        }
        
        public void FocusTextField()
        {
            Title.Blur();
            Title.Focus();
            var textLength = Title.text.Length;
            Title.SelectRange(textLength, textLength);
        }

        public void SetContentVisibility(bool state)
        {
            _content.style.display = state ? DisplayStyle.Flex : DisplayStyle.None;

            if (!state)
            {
                _icon.RemoveFromClassList(IconRotationOpenedClassName);
                _icon.AddToClassList(IconRotationOpenedClassName);
                return;
            }
            
            _icon.RemoveFromClassList(IconRotationOpenedClassName);
            _icon.AddToClassList(IconRotationClosedClassName);
        }
        
        public void ToggleFoldoutStyle(bool value)
        {
            var borderTopWidth = value ? 1 : 0;
            _root.Q(FooterName).style.borderTopWidth = borderTopWidth;
        }
    }
}
