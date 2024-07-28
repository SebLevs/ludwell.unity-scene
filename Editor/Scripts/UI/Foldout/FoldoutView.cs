using System;
using Ludwell.UIToolkitUtilities;
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

        public Func<IEventHandler, bool> OnPreventHeaderClick;
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

        private void ExecuteTitleValueChangedCallback(ChangeEvent<string> evt) =>
            OnTitleValueChanged?.Invoke(evt.newValue);

        private void OnClickStopPropagation(ClickEvent evt) => evt.StopPropagation();

        public FoldoutView(VisualElement root)
        {
            _root = root;

            _header = _root.Q<VisualElement>(HeaderName);
            _header.RegisterCallback<ClickEvent>(ExecuteHeaderClickedCallback);

            _root.RegisterCallback<KeyDownEvent>(OnKeyDownSimulateHeaderClick);

            _icon = _root.Q<VisualElement>(ToggleIconName);

            Title = _root.Q<TextField>(TitleName);
            Title.RegisterValueChangedCallback(ExecuteTitleValueChangedCallback);
            Title.RegisterCallback<ClickEvent>(SetStyleSelectedTextField);
            Title.RegisterCallback<ClickEvent>(OnClickStopPropagation);
            Title.RegisterCallback<BlurEvent>(SetStyleUnselectedTextField);

            _content = _root.Q<VisualElement>(ContentName);

            _root.RegisterCallback<DetachFromPanelEvent>(Dispose);
        }

        public void FocusTextField()
        {
            // todo: find a solution to this hack
            Title.FocusOnNextEditorFrame();
        }

        public void SetContentVisibility(bool state)
        {
            _content.style.display = state ? DisplayStyle.Flex : DisplayStyle.None;

            ToggleFoldoutStyle(state);

            if (!state)
            {
                _icon.RemoveFromClassList(IconRotationOpenedClassName);
                _icon.AddToClassList(IconRotationClosedClassName);
                return;
            }

            _icon.RemoveFromClassList(IconRotationClosedClassName);
            _icon.AddToClassList(IconRotationOpenedClassName);
        }

        public void ToggleFoldoutStyle(bool value)
        {
            var borderTopWidth = value ? 1 : 0;
            _root.Q(FooterName).style.borderTopWidth = borderTopWidth;
        }

        private void ExecuteHeaderClickedCallback(ClickEvent evt)
        {
            // if (evt.target is Button) return;
            if (OnPreventHeaderClick?.Invoke(evt.target) == true) return;
            OnHeaderClicked?.Invoke();
        }

        private void OnKeyDownSimulateHeaderClick(KeyDownEvent evt)
        {
            if (evt.keyCode != KeyCode.Space) return;
            ExecuteHeaderClickedCallback(null);
        }

        private void SetStyleSelectedTextField(ClickEvent _)
        {
            Title.RemoveFromClassList(TextFieldUnselectedClass);
            Title.AddToClassList(TextFieldSelectedClass);
        }

        private void SetStyleUnselectedTextField(BlurEvent _)
        {
            Title.RemoveFromClassList(TextFieldSelectedClass);
            Title.AddToClassList(TextFieldUnselectedClass);
        }

        private void Dispose(DetachFromPanelEvent _)
        {
            _root.UnregisterCallback<DetachFromPanelEvent>(Dispose);

            _header.UnregisterCallback<ClickEvent>(ExecuteHeaderClickedCallback);

            _root.UnregisterCallback<KeyDownEvent>(OnKeyDownSimulateHeaderClick);

            Title.UnregisterValueChangedCallback(ExecuteTitleValueChangedCallback);
            Title.UnregisterCallback<ClickEvent>(SetStyleSelectedTextField);
            Title.UnregisterCallback<ClickEvent>(OnClickStopPropagation);
            Title.UnregisterCallback<BlurEvent>(SetStyleUnselectedTextField);
        }
    }
}