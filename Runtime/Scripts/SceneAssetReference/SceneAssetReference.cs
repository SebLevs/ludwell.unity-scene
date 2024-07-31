using System;
using UnityEngine;

namespace Ludwell.Scene
{
    [Serializable]
    public class SceneAssetReference
    {
        /// <summary>
        /// The SceneAsset path GUID.<br/>
        /// Used in <see cref="Value"/> to return information about the referenced SceneAsset.
        /// </summary>
        [SerializeField] private string Key;

        public bool IsKeyEmpty => string.IsNullOrEmpty(Key);

        public bool IsValid => SceneAssetDataContainer.Instance.Contains(Key);

        public void SetKey(string key)
        {
            Key = key;
        }

        public SceneAssetData Value()
        {
            if (string.IsNullOrEmpty(Key)) return null;

            if (SceneAssetDataContainer.Instance.TryGetValue(Key, out var data)) return data;

            Debug.LogError($"No {nameof(SceneAssetData)} could be found from key: {Key}");
            return null;
        }
    }
}