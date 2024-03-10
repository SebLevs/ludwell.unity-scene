using System;
using UnityEngine.UIElements;

namespace Ludwell.Scene.Editor
{
    public class TagControllerElementController
    {
        private Tag _data;
        
        private TagsController _tagsController;
        
        private string _listingStrategyName = "tag";
        private DropdownSearchField _dropdownSearchField;
        
        public TagControllerElementController(VisualElement view)
        {
            
            view.RegisterCallback<AttachToPanelEvent>(_ =>
            {
                _tagsController = view.GetFirstAncestorOfType<TagsController>();
                _dropdownSearchField = view.FindInAncestors<DropdownSearchField>();
                _listingStrategyName = _dropdownSearchField.HasSearchStrategy(_listingStrategyName)
                    ? _listingStrategyName
                    : DropdownSearchField.DefaultSearchName;
            });
        }

        public void RemoveFromController()
        {
            _tagsController.Remove(_data as TagWithSubscribers);
        }

        public void UpdateTag(Tag tag)
        {
            _data = tag;
        }

        public void SetValue(TagControllerElementView view)
        {
            view.SetValue(_data.Name);
            _tagsController?.Rebuild();
        }

        public void AddValueChangedCallback(Action<string> callback)
        {
            _data.AddValueChangedCallback(callback);
        }
        
        public void RemoveValueChangedCallback(Action<string> callback)
        {
            _data.RemoveValueChangedCallback(callback);
        }

        public void SearchWithData()
        {
            _dropdownSearchField.ListWithStrategy(_listingStrategyName, _data.Name);
        }
    }
}
