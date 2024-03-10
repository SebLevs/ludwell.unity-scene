using UnityEngine.UIElements;

namespace Ludwell.Scene.Editor
{
    public class TagsManagerElementController
    {
        public TagWithSubscribers Cache;
        
        private TagsManager _tagsManager;

        
        public TagsManagerElementController(VisualElement view)
        {
            _tagsManager = view.GetFirstAncestorOfType<TagsManager>();
        }
        
        public void AddToController()
        {
            _tagsManager.AddTagToController(Cache);
        }
        
        public void RemoveFromController()
        {
            _tagsManager.RemoveTagFromController(Cache);
        }

        public void UpdateValue(string value)
        {
            Cache.Name = value;
            DataFetcher.SaveEveryScriptableDelayed();
        }

        public void SetValue(TagsManagerElementView view)
        {
            view.SetText(Cache.Name);
        }

        public void FocusTextField(TagsManagerElementView view, TextField textField)
        {
            if (!string.IsNullOrEmpty(Cache.Name)) return;
            textField.Focus();
            _tagsManager.SetPreviousTarget(view);

        }
        
        public void HandleInvalidTag()
        {
            if (!string.IsNullOrEmpty(Cache.Name) && !_tagsManager.IsTagDuplicate(Cache)) return;
            _tagsManager.RemoveInvalidTagElement(Cache);
            Cache.RemoveFromAllSubscribers();
        }
        
        public void OnEditCompleted()
        {
            _tagsManager.SortTags();
            _tagsManager.SetPreviousTarget(null);
        }
    }
}
