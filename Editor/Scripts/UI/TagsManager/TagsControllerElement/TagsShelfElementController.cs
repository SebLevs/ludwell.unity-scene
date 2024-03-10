using System;
using UnityEngine.UIElements;

namespace Ludwell.Scene.Editor
{
    public class TagsShelfElementController
    {
        private Tag _data;
        
        private TagsShelfView _tagsShelfView;
        
        private string _listingStrategyName = "tag";
        private DropdownSearchField _dropdownSearchField;
        
        public TagsShelfElementController(VisualElement view)
        {
            
            view.RegisterCallback<AttachToPanelEvent>(_ =>
            {
                _tagsShelfView = view.GetFirstAncestorOfType<TagsShelfView>();
                _dropdownSearchField = view.FindInAncestors<DropdownSearchField>();
                _listingStrategyName = _dropdownSearchField.HasSearchStrategy(_listingStrategyName)
                    ? _listingStrategyName
                    : DropdownSearchField.DefaultSearchName;
            });
        }

        public void RemoveFromController()
        {
            _tagsShelfView.Remove(_data as TagWithSubscribers);
        }

        public void UpdateTag(Tag tag)
        {
            _data = tag;
        }

        public void SetValue(TagsShelfElementView view)
        {
            view.SetValue(_data.Name);
            _tagsShelfView?.Rebuild();
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
