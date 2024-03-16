using System;
using UnityEngine.UIElements;

namespace Ludwell.Scene.Editor
{
    public class TagsShelfElementController
    {
        private Tag _data;

        private TagsShelfController _tagShelfController;

        private string _listingStrategyName = "tag";
        private DropdownSearchField _dropdownSearchField;

        public TagsShelfElementController(VisualElement view)
        {
            view.RegisterCallback<AttachToPanelEvent>(_ =>
            {
                _dropdownSearchField = view.FindInAncestors<DropdownSearchField>();
                _listingStrategyName = _dropdownSearchField.HasSearchStrategy(_listingStrategyName)
                    ? _listingStrategyName
                    : DropdownSearchField.DefaultSearchName;
            });
        }

        public void SetTagShelfController(TagsShelfController tagsShelfController)
        {
            _tagShelfController = tagsShelfController;
        }

        public void RemoveFromController()
        {
            _tagShelfController.Remove(_data as TagWithSubscribers);
        }

        public void UpdateTag(Tag tag)
        {
            _data = tag;
        }

        public void SetValue(TagsShelfElementView view)
        {
            view.SetValue(_data.Name);
        }

        public void AddValueChangedCallback(Action<string> callback)
        {
            _data.OnValueChanged += callback;
        }

        public void RemoveValueChangedCallback(Action<string> callback)
        {
            if (_data == null) return;
            _data.OnValueChanged -= callback;
        }

        public void SearchWithData()
        {
            _dropdownSearchField.ListWithStrategy(_listingStrategyName, _data.Name);
        }
    }
}