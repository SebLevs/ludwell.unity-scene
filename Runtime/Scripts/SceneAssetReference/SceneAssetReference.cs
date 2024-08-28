using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Ludwell.SceneManagerToolkit
{
#if UNITY_EDITOR
    public partial class SceneAssetReference
    {
        /// <summary> Required for the property drawer binding. </summary>
        [SerializeField] private SceneAsset _reference;
    }
#endif

    [Serializable]
    public partial class SceneAssetReference
    {
        /// <summary>
        /// The SceneAsset GUID.<br/>
        /// Used in <see cref="GetData"/> to return information about the referenced SceneAsset.
        /// </summary>
        [SerializeField] private string _guid;

        public bool IsValid => !string.IsNullOrEmpty(_guid) && SceneAssetDataBinders.Instance.ContainsWithId(_guid);

        public List<Tag> GetTags()
        {
            if (string.IsNullOrEmpty(_guid)) return null;

            if (SceneAssetDataBinders.Instance.TryGetBinderFromId(_guid, out var binder)) return binder.Tags;

            Debug.LogError($"No {nameof(SceneAssetDataBinder)} could be found from key: {_guid}");
            return null;
        }

        public SceneAssetData GetData()
        {
            if (string.IsNullOrEmpty(_guid)) return null;

            if (SceneAssetDataBinders.Instance.TryGetDataFromId(_guid, out var data)) return data;

            Debug.LogError($"No {nameof(SceneAssetData)} could be found from key: {_guid}");
            return null;
        }
    }
}
