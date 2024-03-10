using UnityEngine.UIElements;

namespace Ludwell.Scene.Editor
{
    public class TagsManagerElementController
    {
        private TagWithSubscribers _data;
        
        private readonly TagsManager _tagsManager;
        
        public TagsManagerElementController(VisualElement view)
        {
            _tagsManager = view.GetFirstAncestorOfType<TagsManager>();
        }

        public void UpdateData(TagWithSubscribers data)
        {
            _data = data;
        }
        
        public void AddToController()
        {
            _tagsManager.AddTagToController(_data);
        }
        
        public void RemoveFromController()
        {
            _tagsManager.RemoveTagFromController(_data);
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
            _tagsManager.SetPreviousTarget(view);

        }
        
        public void HandleInvalidTag()
        {
            if (!string.IsNullOrEmpty(_data.Name) && !_tagsManager.IsTagDuplicate(_data)) return;
            _tagsManager.RemoveInvalidTagElement(_data);
            _data.RemoveFromAllSubscribers();
        }
        
        public void OnEditCompleted()
        {
            _tagsManager.SortTags();
            _tagsManager.SetPreviousTarget(null);
        }
    }
}
