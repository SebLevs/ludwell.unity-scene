namespace Ludwell.UIToolkitUtilities.Editor
{
    public interface IListViewVisualElement<TData>
    {
        public void CacheData(TData data);

        public void BindElementToCachedData();

        public void SetElementFromCachedData();
    }
}
