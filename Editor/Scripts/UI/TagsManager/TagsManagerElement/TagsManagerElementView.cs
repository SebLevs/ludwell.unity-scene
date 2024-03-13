using System;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ludwell.Scene.Editor
{
    public class TagsManagerElementView : VisualElement, IListViewVisualElement<TagWithSubscribers>
    {
        public new class UxmlFactory : UxmlFactory<TagsManagerElementView, UxmlTraits>
        {
        }

        private static readonly string UxmlPath =
            Path.Combine("Uxml", nameof(TagsManagerView), nameof(TagsManagerElementView));

        private static readonly string UssPath =
            Path.Combine("Uss", nameof(TagsManagerView), nameof(TagsManagerElementView));

        public Action<TagWithSubscribers> OnAdd;
        public Action<TagWithSubscribers> OnRemove;
        public Action OnTextEditEnd;

        private const string AddButtonName = "button__add";
        private const string RemoveButtonName = "button__remove";

        private const string TagTextFieldName = "tag";

        private Button _addButton;
        private Button _removeButton;

        private TextField _textField;

        private TagsManagerElementController _controller;

        public TagsManagerElementView()
        {
            this.AddHierarchyFromUxml(UxmlPath);
            this.AddStyleFromUss(UssPath);

            SetReferences();
            InitializeButtons();
            InitializeValidityHandlingEvents();
        }

        public void CacheData(TagWithSubscribers data)
        {
            _controller.Data = data;
        }

        public void BindElementToCachedData()
        {
            _textField.RegisterValueChangedCallback(BindTextField);
        }

        public void SetElementFromCachedData()
        {
            _controller.SetValue(this);
            _controller.FocusTextField(this, _textField);
        }

        // todo: refactor on second draft
        public void HandleInvalidTag()
        {
            _controller.HandleInvalidTag();
        }

        public void SetText(string value)
        {
            _textField.value = value;
        }

        private void SetReferences()
        {
            _addButton = this.Q<Button>(AddButtonName);
            _removeButton = this.Q<Button>(RemoveButtonName);

            _textField = this.Q<TextField>(TagTextFieldName);

            RegisterCallback<AttachToPanelEvent>(_ => _controller = new TagsManagerElementController(this));
        }

        private void InitializeButtons()
        {
            _addButton.clicked += AddBehaviour;
            _removeButton.clicked += RemoveBehaviour;
        }

        private void AddBehaviour()
        {
            OnAdd?.Invoke(_controller.Data);
        }

        private void RemoveBehaviour()
        {
            OnRemove?.Invoke(_controller.Data);
        }

        private void InitializeValidityHandlingEvents()
        {
            _textField.RegisterCallback<BlurEvent>(_ => _controller.HandleInvalidTag());
            RegisterCallback<AttachToPanelEvent>(_ => _textField.RegisterCallback<KeyDownEvent>(OnKeyDown));
        }

        private void OnKeyDown(KeyDownEvent evt)
        {
            if (evt.keyCode != KeyCode.Return) return;
            OnTextEditEnd?.Invoke();
        }

        private void BindTextField(ChangeEvent<string> evt)
        {
            _controller.UpdateValue(evt.newValue);
        }
    }
}