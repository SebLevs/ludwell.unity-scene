using System;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ludwell.Scene.Editor
{
    public class TagsManagerElementController : VisualElement, IListViewVisualElement<TagWithSubscribers>
    {
        public new class UxmlFactory : UxmlFactory<TagsManagerElementController, UxmlTraits>
        {
        }

        public Action<TagWithSubscribers> OnAdd;
        public Action<TagWithSubscribers> OnRemove;
        public Action<string> OnValueChanged;

        private static readonly string UxmlPath =
            Path.Combine("UI", nameof(TagsManagerElementView), "Uxml_" + nameof(TagsManagerElementView));

        private static readonly string UssPath =
            Path.Combine("UI", nameof(TagsManagerElementView), "Uss_" + nameof(TagsManagerElementView));

        private readonly TagsManagerElementView _view;

        private TagWithSubscribers _data;

        public TagsManagerElementController()
        {
            this.AddHierarchyFromUxml(UxmlPath);
            this.AddStyleFromUss(UssPath);

            _view = new TagsManagerElementView(this);
            _view.OnAdd += AddAction;
            _view.OnRemove += RemoveAction;
            _view.OnValueChanged += OnValueChanged;

            var textField = this.Q<TextField>();
            textField.RegisterCallback<BlurEvent>(_ => DataFetcher.GetTagContainer().HandleUpdatedTag(_data));
            textField.RegisterCallback<KeyDownEvent>(OnReturnKeyDownEndTextEdit);
        }

        ~TagsManagerElementController()
        {
            _view.OnAdd -= AddAction;
            _view.OnRemove -= RemoveAction;
            _view.OnValueChanged -= OnValueChanged;
            
            var textField = this.Q<TextField>();
            textField.UnregisterCallback<BlurEvent>(_ => DataFetcher.GetTagContainer().HandleUpdatedTag(_data));
            textField.UnregisterCallback<KeyDownEvent>(OnReturnKeyDownEndTextEdit);
        }

        public void CacheData(TagWithSubscribers data)
        {
            _data = data;
        }

        public void BindElementToCachedData()
        {
            _view.OnValueChanged += UpdateDataValue;
        }

        public void SetElementFromCachedData()
        {
            _view.SetValue(_data.Name);
            
            if (!string.IsNullOrEmpty(_view.Value)) return;
            _view.FocusTextField();
        }

        private void OnReturnKeyDownEndTextEdit(KeyDownEvent evt)
        {
            if (evt.keyCode != KeyCode.Return) return;
            UpdateDataValue(_view.Value);
        }

        private void UpdateDataValue(string value)
        {
            _data.Name = value;
            DataFetcher.SaveTagContainerDelayed();
        }
        
        private void AddAction()
        {
            OnAdd?.Invoke(_data);
        }
        
        private void RemoveAction()
        {
            OnRemove?.Invoke(_data);
        }
    }
}