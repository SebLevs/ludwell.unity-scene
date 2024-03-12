using UnityEngine.UIElements;

namespace Ludwell.Scene.Editor
{
    public class TagsManagerPresentor
    {
        private TagSubscriber _tagSubscriber;

        private VisualElement _previousView;
        private readonly TagsManagerView _view;

        private readonly TagsShelfView _tagsShelfView;

        public TagsManagerPresentor(TagsManagerView view)
        {
            _view = view;
            _tagsShelfView = view.Q<TagsShelfView>();

        }

        public void Show(TagSubscriberWithTags tagSubscriber, VisualElement previousView)
        {
            _tagSubscriber = tagSubscriber;
            _previousView = previousView;
            _previousView.style.display = DisplayStyle.None;

            BuildTagsController(tagSubscriber);

            _view.SetReferenceText(tagSubscriber.Name);
            _view.Show();
        }

        public void AddTagToShelf(TagWithSubscribers tag)
        {
            _tagsShelfView.Add(tag);
        }
        
        public void RemoveTagFromShelf(TagWithSubscribers tag)
        {
            _tagsShelfView.Remove(tag);
        }
        
        public void AddSubscriberToTag(TagWithSubscribers tag)
        {
            tag.AddSubscriber(_tagSubscriber);
        }
        
        public void RemoveSubscriberFromTag(TagWithSubscribers tag)
        {
            tag.RemoveSubscriber(_tagSubscriber);
        }
        
        public void RemoveTagFromAllSubscribers(TagWithSubscribers tag)
        {
            _tagsShelfView.Remove(tag);
            tag.RemoveFromAllSubscribers();
        }

        public void HandleTagController()
        {
            _tagsShelfView.OverrideIconTooltip("Return");
        }

        public void ReturnToPreviousView()
        {
            _view.style.display = DisplayStyle.None;
            _previousView.style.display = DisplayStyle.Flex;
        }

        private void BuildTagsController(TagSubscriberWithTags tagSubscriber)
        {
            _tagsShelfView
                .WithTagSubscriber(tagSubscriber)
                .WithOptionButtonEvent(ReturnToPreviousView)
                .PopulateContainer();
        }
    }
}