using System;
using Ludwell.UIToolkitElements;
using UnityEngine;

namespace Ludwell.Scene
{
    [Serializable]
    public class SceneAssetDataBinder : TagSubscriberWithTags, IListable, IComparable
    {
        /// <summary> AssetPathToGUID </summary>
        [field: SerializeField]
        public string ID { get; set; }

        public SceneAssetData Data;

        public override string GetID() => Data.Name;

        public string GetListableId() => Data.Name;

        public int CompareTo(object obj)
        {
            // if (obj == null) return 1;
            //
            // var otherAsType = (SceneAssetDataBinder)obj;
            // return string.Compare(ID, otherAsType.ID, StringComparison.InvariantCultureIgnoreCase);

            if (obj is not SceneAssetDataBinder otherAsType) return 1;
            return string.Compare(Data.Name, otherAsType.Data.Name, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}