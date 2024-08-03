using System;
using UnityEngine;

namespace Ludwell.Scene
{
    [Serializable]
    public class SceneAssetReference
    {
        /// <summary>
        /// The SceneAsset path GUID.<br/>
        /// Used in <see cref="Data"/> to return information about the referenced SceneAsset.
        /// </summary>
        [SerializeField] private string Key;

        public bool IsValid => string.IsNullOrEmpty(Key) && SceneAssetDataBinders.Instance.ContainsWithId(Key);

        public SceneAssetData Data()
        {
            if (string.IsNullOrEmpty(Key)) return null;

            if (SceneAssetDataBinders.Instance.TryGetDataFromId(Key, out var data)) return data;

            Debug.LogError($"No {nameof(SceneAssetData)} could be found from key: {Key}");
            return null;
        }
    }
}