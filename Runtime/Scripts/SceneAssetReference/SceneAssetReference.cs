using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Ludwell.Scene
{
    [Serializable]
    public class SceneAssetReference
    {
        /// <summary>
        /// The SceneAsset GUID.<br/>
        /// Used in <see cref="Data"/> to return information about the referenced SceneAsset.
        /// </summary>
        [SerializeField] private string _guid;

        /// <summary> Required for the property drawer binding. </summary>
        [SerializeField] private Object _sceneAsset;

        public bool IsValid => !string.IsNullOrEmpty(_guid) && SceneAssetDataBinders.Instance.ContainsWithId(_guid);

        public SceneAssetData Data()
        {
            if (string.IsNullOrEmpty(_guid)) return null;

            if (SceneAssetDataBinders.Instance.TryGetDataFromId(_guid, out var data)) return data;

            Debug.LogError($"No {nameof(SceneAssetData)} could be found from key: {_guid}");
            return null;
        }
    }
}