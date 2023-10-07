namespace Ludwell.Scene
{
    public interface IBindableListViewElement<T>
    {
        public void InitDataValues(T data);
        
        public void BindElementToData(T data);
        
        public void SetElementFromData(T data);
    }
}
