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

        public void FocusTextField() => _view.FocusTextField();

        public TagsManagerElementController()
        {
            this.AddHierarchyFromUxml(UxmlPath);
            this.AddStyleFromUss(UssPath);

            _view = new TagsManagerElementView(this);
            _view.OnAdd += ExecuteOnAdd;
            _view.OnRemove += ExecuteOnRemove;

            _view.TextField.RegisterCallback<BlurEvent>(SolveBlurred);
            _view.TextField.RegisterCallback<KeyDownEvent>(OnReturnKeyHandleAssetName);

            RegisterCallback<DetachFromPanelEvent>(Dispose);
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

        private void SolveBlurred(BlurEvent evt)
        {
            if (_view.TextField.value != _model.Name) _view.TextField.value = _model.Name;
            if (string.IsNullOrEmpty(_view.TextField.value)) ResourcesLocator.GetTagContainer().RemoveTag(_model);
        }

        private void OnReturnKeyHandleAssetName(KeyDownEvent evt)
        {
            switch (evt.keyCode)
            {
                case KeyCode.Return:
                    UpdateAssetName(_view.TextField.value);
                    break;
                case KeyCode.Z when (evt.modifiers & EventModifiers.Control) != 0:
                case KeyCode.Escape:
                    _view.SetValue(_model.Name);
                    evt.StopPropagation();
                    break;
            }
        }

        public void UpdateAssetName(string value)
        {
            if (_model.Name == value) return;
            _model.Name = value;
            if (!ResourcesLocator.GetTagContainer().HandleTagValidity(_model)) return;
            ResourcesLocator.GetTagContainer().Tags.Sort();
            Signals.Dispatch<UISignals.RefreshView>();

            var index = ResourcesLocator.GetTagContainer().Tags.FindIndex(x => x == _model);
            var tagsManagerController = ServiceLocator.Get<TagsManagerController>();
            tagsManagerController.ScrollToItemIndex(index);
            ResourcesLocator.SaveTagContainer();
        }

        private void ExecuteOnAdd()
        {
            OnAdd?.Invoke(_model);
        }

        private void ExecuteOnRemove()
        {
            OnRemove?.Invoke(_model);
        }

        private void Dispose(DetachFromPanelEvent _)
        {
            UnregisterCallback<DetachFromPanelEvent>(Dispose);
            _view.OnAdd -= ExecuteOnAdd;
            _view.OnRemove -= ExecuteOnRemove;

            _view.TextField.UnregisterCallback<BlurEvent>(SolveBlurred);
            _view.TextField.UnregisterCallback<KeyDownEvent>(OnReturnKeyHandleAssetName);
        }
    }
}