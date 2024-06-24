using System;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ludwell.Scene.Editor
{
    public class TagsManagerElementController : VisualElement, IListViewVisualElement<TagWithSubscribers>
    {
        private static readonly string UxmlPath =
            Path.Combine("UI", nameof(TagsManagerElementView), "Uxml_" + nameof(TagsManagerElementView));

        private static readonly string UssPath =
            Path.Combine("UI", nameof(TagsManagerElementView), "Uss_" + nameof(TagsManagerElementView));

        public Action<TagWithSubscribers> OnAdd;
        public Action<TagWithSubscribers> OnRemove;

        private readonly TagsManagerElementView _view;

        private TagWithSubscribers _model;

        public TextField TextField => _view.TextField;

        public void FocusTextField() => _view.FocusTextField();

        public TagsManagerElementController()
        {
            this.AddHierarchyFromUxml(UxmlPath);
            this.AddStyleFromUss(UssPath);

            _view = new TagsManagerElementView(this);
            _view.OnAdd += ExecuteOnAdd;
            _view.OnRemove += ExecuteOnRemove;

            _view.TextField.RegisterCallback<BlurEvent>(UpdateAssetName);
            _view.TextField.RegisterCallback<KeyDownEvent>(HandleKeyPress);
        }

        public void CacheData(TagWithSubscribers data)
        {
            _model = data;
        }

        public void BindElementToCachedData()
        {
        }

        public void SetElementFromCachedData()
        {
            _view.SetValue(_model.Name);
            if (string.IsNullOrEmpty(_view.Value)) _view.FocusTextFieldWithoutNotify();
        }

        private void UpdateAssetName(BlurEvent evt)
        {
            var isNullOrEmptyOrWhiteSpace = string.IsNullOrEmpty(_model.Name) || string.IsNullOrWhiteSpace(_model.Name);

            if (!isNullOrEmptyOrWhiteSpace && _model.Name == _view.TextField.value) return;

            var previousName = _model.Name;
            _model.Name = _view.TextField.value;

            if (isNullOrEmptyOrWhiteSpace)
            {
                ResourcesLocator.GetTagContainer().RemoveTag(_model);
                Signals.Dispatch<UISignals.RefreshView>();
                return;
            }

            if (ResourcesLocator.GetTagContainer().IsTagDuplicate(_model))
            {
                _view.TextField.value = previousName;
                _model.Name = previousName;
                Signals.Dispatch<UISignals.RefreshView>();
                return;
            }

            ResourcesLocator.GetTagContainer().Tags.Sort();
            Signals.Dispatch<UISignals.RefreshView>();

            var tagsManagerController = ResourcesLocator.TagsManagerController;
            var index = ResourcesLocator.GetTagContainer().Tags.FindIndex(x => x == _model);
            tagsManagerController.ScrollToItemIndex(index);
            ResourcesLocator.SaveTagContainer();
            Focus();
        }

        private void ExecuteOnAdd()
        {
            OnAdd?.Invoke(_model);
        }

        private void ExecuteOnRemove()
        {
            OnRemove?.Invoke(_model);
        }

        private void HandleKeyPress(KeyDownEvent evt)
        {
            switch (evt.keyCode)
            {
                case KeyCode.Return:
                    Focus();
                    break;
                case KeyCode.Z when (evt.modifiers & EventModifiers.Control) != 0:
                case KeyCode.Escape:
                    _view.SetValue(_model.Name);
                    if (_model.Name != _view.TextField.value) evt.PreventDefault();
                    Focus();
                    break;
            }
        }
    }
}