using UnityEngine.UIElements;

namespace Ludwell.Scene.Editor
{
    public class TagsManagerElementController
    {
        private TagWithSubscribers _data;
        
        private readonly TagsManagerView _tagsManagerView;
        
        public TagsManagerElementController(VisualElement view)
        {
            _tagsManagerView = view.GetFirstAncestorOfType<TagsManagerView>();
        }

        public void UpdateData(TagWithSubscribers data)
        {
            _data = data;
        }
        
        public void AddToController()
        {
            _tagsManagerView.AddTagToShelfDelegated(_data);
        }
        
        public void RemoveFromController()
        {
            _tagsManagerView.RemoveTagFromShelfDelegated(_data);
        }

        public void UpdateValue(string value)
        {
            _data.Name = value;
            DataFetcher.SaveEveryScriptableDelayed();
        }

        public void SetValue(TagsManagerElementView view)
        {
            view.SetText(_data.Name);
        }

        public void FocusTextField(TagsManagerElementView view, TextField textField)
        {
            if (!string.IsNullOrEmpty(_data.Name)) return;
            textField.Focus();
            _tagsManagerView.SetPreviousTarget(view);

        }
        
        public void HandleInvalidTag()
        {
            if (!string.IsNullOrEmpty(_data.Name) && !_tagsManagerView.IsTagDuplicate(_data)) return;
            _tagsManagerView.RemoveInvalidTagElement(_data);
            _data.RemoveFromAllSubscribers();
        }
        
        public void OnEditCompleted()
        {
            _tagsManagerView.SortTags();
            _tagsManagerView.SetPreviousTarget(null);
        }
    }
}
