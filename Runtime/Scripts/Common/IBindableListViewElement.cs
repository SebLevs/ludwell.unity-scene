namespace Ludwell.Scene
{
    public interface IBindableListViewElement<T>
    {
        public T Cache { get; set; }

        public void CacheData(T data);

        public void BindElementToCachedData();

        public void SetElementFromCachedData();
    }
}