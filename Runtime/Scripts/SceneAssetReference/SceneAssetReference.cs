using System;
using UnityEditor;
using UnityEngine;

namespace Ludwell.Scene
{
#if UNITY_EDITOR
    public partial class SceneAssetReference
    {
        /// <summary> Required for the property drawer binding. </summary>
        public SceneAsset _reference;
    }
#endif
    
    [Serializable]
    public partial class SceneAssetReference
    {
        /// <summary>
        /// The SceneAsset GUID.<br/>
        /// Used in <see cref="Data"/> to return information about the referenced SceneAsset.
        /// </summary>
        public string _guid;
        
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