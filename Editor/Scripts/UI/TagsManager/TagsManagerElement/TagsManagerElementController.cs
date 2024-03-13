using UnityEngine.UIElements;

namespace Ludwell.Scene.Editor
{
    public class TagsManagerElementController
    {
        public TagWithSubscribers Data;

        private readonly TagsManagerView _tagsManagerView;

        private readonly TagContainer _tagContainer;

        public TagsManagerElementController(VisualElement view)
        {
            _tagsManagerView = view.GetFirstAncestorOfType<TagsManagerView>();
            _tagContainer = DataFetcher.GetTagContainer();
        }

        public void UpdateValue(string value)
        {
            Data.Name = value;
            DataFetcher.SaveEveryScriptableDelayed();
        }

        public void SetValue(TagsManagerElementView view)
        {
            view.SetText(Data.Name);
        }
    }
}