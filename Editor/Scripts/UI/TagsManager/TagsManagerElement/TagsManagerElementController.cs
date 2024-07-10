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
            _view.TextField.RegisterCallback<KeyDownEvent>(OnKeyRevertName);

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
            var isTextFieldValid = !string.IsNullOrEmpty(_view.TextField.value) &&
                                    !string.IsNullOrWhiteSpace(_view.TextField.value);
            var isModelNameValid = !string.IsNullOrEmpty(_model.Name) &&
                                    !string.IsNullOrWhiteSpace(_model.Name);
            if (!isTextFieldValid && !isModelNameValid) ResourcesLocator.GetTagContainer().RemoveTag(_model);

            if (_model.Name == _view.TextField.value) return;

            if (ResourcesLocator.GetTagContainer().ContainsTagWithName(_view.TextField.value))
            {
                _view.TextField.value = _model.Name;
                return;
            }
            
            UpdateAssetName();
        }

        private void OnKeyRevertName(KeyDownEvent evt)
        {
            if (evt.keyCode != KeyCode.Z || (evt.modifiers & EventModifiers.Control) == 0) return;
            _view.SetValue(_model.Name);
            evt.StopPropagation();
        }

        private void UpdateAssetName()
        {
            _model.Name = _view.TextField.value;
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
            _view.TextField.UnregisterCallback<KeyDownEvent>(OnKeyRevertName);
        }
    }
}