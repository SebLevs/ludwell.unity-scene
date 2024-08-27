using System;
using System.IO;
using Ludwell.Architecture;
using Ludwell.UIToolkitUtilities;
using Ludwell.UIToolkitUtilities.Editor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ludwell.Scene.Editor
{
    public class TagsManagerElementController : VisualElement, IListViewVisualElement<Tag>, IDisposable
    {
        private static readonly string UxmlPath =
            Path.Combine("UI", nameof(TagsManagerElementView), "Uxml_" + nameof(TagsManagerElementView));

        private static readonly string UssPath =
            Path.Combine("UI", nameof(TagsManagerElementView), "Uss_" + nameof(TagsManagerElementView));

        public Action<Tag> OnAddToShelf;
        public Action<Tag> OnRemoveFromShelf;

        private readonly TagsManagerElementView _view;

        private Tag _model;

        public void FocusTextField() => _view.FocusTextField();

        public bool IsTextFieldValue(string value) => _view.TextField.value == value;

        public TagsManagerElementController()
        {
            this.AddHierarchyFromUxml(UxmlPath);
            this.AddStyleFromUss(UssPath);

            _view = new TagsManagerElementView(this);
            _view.OnAdd += ExecuteOnAdd;
            _view.OnRemove += ExecuteOnRemove;

            _view.TextField.RegisterCallback<BlurEvent>(SolveBlurred);
            _view.TextField.RegisterCallback<KeyDownEvent>(OnKeyRevertName);

            Services.Get<Disposer>().Add(this);
        }

        public void Dispose()
        {
            _view.OnAdd -= ExecuteOnAdd;
            _view.OnRemove -= ExecuteOnRemove;

            _view.TextField.UnregisterCallback<BlurEvent>(SolveBlurred);
            _view.TextField.UnregisterCallback<KeyDownEvent>(OnKeyRevertName);

            _view.Dispose();
        }

        public void CacheData(Tag data)
        {
            _model = data;
        }

        public void BindElementToCachedData()
        {
        }

        public void SetElementFromCachedData()
        {
            _view.SetValue(_model.ID);
            if (string.IsNullOrEmpty(_view.Value)) _view.FocusTextFieldWithoutNotify();
        }

        private void SolveBlurred(BlurEvent evt)
        {
            var isTextFieldValid = !string.IsNullOrEmpty(_view.TextField.value) &&
                                   !string.IsNullOrWhiteSpace(_view.TextField.value);
            var isModelNameValid = !string.IsNullOrEmpty(_model.ID) &&
                                   !string.IsNullOrWhiteSpace(_model.ID);

            if (!isTextFieldValid && !isModelNameValid)
            {
                ResourcesLocator.GetTags().Remove(_model);
                Signals.Dispatch<UISignals.RefreshView>();
                return;
            }

            if (isModelNameValid && !isTextFieldValid)
            {
                _view.TextField.value = _model.ID;
                return;
            }

            if (_model.ID == _view.TextField.value) return;

            if (ResourcesLocator.GetTags().ContainsTagWithName(_view.TextField.value))
            {
                _view.TextField.value = _model.ID;

                if (!isModelNameValid) ResourcesLocator.GetTags().Remove(_model);
                Signals.Dispatch<UISignals.RefreshView>();
                return;
            }

            UpdateAssetName();
        }

        private void OnKeyRevertName(KeyDownEvent evt)
        {
            if (evt.keyCode != KeyCode.Z || (evt.modifiers & EventModifiers.Control) == 0) return;
            _view.SetValue(_model.ID);
            evt.StopPropagation();
        }

        private void UpdateAssetName()
        {
            _model.ID = _view.TextField.value;
            ResourcesLocator.GetTags().Elements.Sort();

            var index = ResourcesLocator.GetTags().Elements.FindIndex(x => Equals(x, _model));

            schedule.Execute(() => { Services.Get<TagsManagerController>().ScrollToItemIndex(index); });

            ResourcesLocator.SaveTags();
        }

        private void ExecuteOnAdd()
        {
            OnAddToShelf?.Invoke(_model);
        }

        private void ExecuteOnRemove()
        {
            OnRemoveFromShelf?.Invoke(_model);
        }
    }
}