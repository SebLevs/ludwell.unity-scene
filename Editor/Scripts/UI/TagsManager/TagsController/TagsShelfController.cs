namespace Ludwell.Scene.Editor
{
    public class TagsShelfController
    {
        private TagSubscriberWithTags _data = new();

        public int Count => _data.Tags.Count;
        public int IndexOf(TagWithSubscribers tagWithSubscribers) => _data.Tags.IndexOf(tagWithSubscribers);

        public TagsShelfController()
        {
        }

        public bool Contains(Tag tag)
        {
            return _data.Tags.Contains(tag);
        }

        public void Add(Tag tag)
        {
            _data.Tags.Add(tag);
        }

        public void Remove(TagWithSubscribers tagWithSubscribers)
        {
            _data.Tags.Remove(tagWithSubscribers);
            tagWithSubscribers.RemoveSubscriber(_data);
        }

        public void Sort()
        {
            _data.Tags.Sort();
        }

        public void UpdateData(TagSubscriberWithTags data)
        {
            _data = data;
        }

        public void PopulateContainer(TagsShelfView view)
        {
            view.ClearContainer();
            view.Populate(_data.Tags);
        }
    }
}