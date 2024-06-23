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

            var textField = this.Q<TextField>();
            textField.RegisterCallback<BlurEvent>(SolveBlured);
            textField.RegisterCallback<KeyDownEvent>(evt =>
            {
                if (evt.keyCode == KeyCode.Return) UpdateAssetName(textField.value);
            });
        }

        ~TagsManagerElementController()
        {
            _view.OnAdd -= ExecuteOnAdd;
            _view.OnRemove -= ExecuteOnRemove;

            var textField = this.Q<TextField>();
            textField.UnregisterCallback<BlurEvent>(SolveBlured); // todo: return to previous name
            textField.UnregisterCallback<KeyDownEvent>(evt =>
            {
                if (evt.keyCode == KeyCode.Return) UpdateAssetName(textField.value);
            });
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

        private void SolveBlured(BlurEvent evt)
        {
            if (_view.TextField.value != _model.Name) _view.TextField.value = _model.Name;
            if (string.IsNullOrEmpty(_view.TextField.value)) ResourcesLocator.GetTagContainer().RemoveTag(_model);
        }

        public void UpdateAssetName(string value)
        {
            _model.Name = value;
            if (!ResourcesLocator.GetTagContainer().HandleTagValidity(_model)) return; // todo: remove???
            ResourcesLocator.GetTagContainer().Tags.Sort();
            Signals.Dispatch<UISignals.RefreshView>();

            var tagsManagerController = ResourcesLocator.TagsManagerController;
            var index = ResourcesLocator.GetTagContainer().Tags.FindIndex(x => x == _model);
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
    }
}