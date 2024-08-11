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
        [SerializeField] private string _guid;

        public bool IsValid => string.IsNullOrEmpty(_guid) && SceneAssetDataBinders.Instance.ContainsWithId(_guid);

        public SceneAssetData Data()
        {
            if (string.IsNullOrEmpty(_guid)) return null;

            if (SceneAssetDataBinders.Instance.TryGetDataFromId(_guid, out var data)) return data;

            Debug.LogError($"No {nameof(SceneAssetData)} could be found from key: {_guid}");
            return null;
        }
    }
}