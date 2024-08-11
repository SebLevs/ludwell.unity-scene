using System;
using Ludwell.UIToolkitElements;

namespace Ludwell.Scene
{
    [Serializable]
    public class SceneAssetDataBinder : TagSubscriberWithTags, IListable, IComparable
    {
        /// <summary>The GUID of the SceneAsset.</summary>
        public string GUID;

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