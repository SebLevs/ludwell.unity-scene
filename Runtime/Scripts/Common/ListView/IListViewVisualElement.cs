namespace Ludwell.Scene
{
    public interface IListViewVisualElement<TData>
    {
        public TData Cache { get; set; }

        public void CacheData(TData data)
        {
            Cache = data;
        }

        public void BindElementToCachedData();

        public void SetElementFromCachedData();
    }
}