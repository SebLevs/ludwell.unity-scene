namespace Ludwell.Scene
{
    public interface IListViewVisualElement<TData>
    {
        public void CacheData(TData data);

        public void BindElementToCachedData();

        public void SetElementFromCachedData();
    }
}