using System;
using Ludwell.UIToolkitElements;

namespace Ludwell.SceneManagerToolkit
{
    [Serializable]
    public class SceneAssetDataBinder : TagSubscriberWithTags, IListable, IComparable
    {
        public SceneAssetData Data;

        public override string GetTagSubscriberWithTagID() => Data.Name;

        public string GetListableId() => Data.Name;

        public int CompareTo(object obj)
        {
            if (obj is not SceneAssetDataBinder otherAsType) return 1;
            return string.Compare(Data.Name, otherAsType.Data.Name, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
