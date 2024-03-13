using UnityEngine.UIElements;

namespace Ludwell.Scene.Editor
{
    public class TagsManagerElementController
    {
        public TagWithSubscribers Data;

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