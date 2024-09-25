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
        /// Used in the methods of this class to return information about the referenced SceneAsset.
        /// </summary>
        [SerializeField] private string _guid;

        private SceneAssetDataBinder _binderCache;

        public bool IsEmpty => string.IsNullOrEmpty(_guid);

        public bool IsValid => !IsEmpty && SceneAssetDataBinders.Instance.ContainsWithGuid(_guid);

        public SceneAssetData Data => CacheBinder() == null ? null : _binderCache.Data;

        public List<Tag> Tags => CacheBinder() == null ? null : _binderCache.Tags;

        private SceneAssetDataBinder CacheBinder()
        {
            if (IsEmpty) return null;

            if (_binderCache != null) return _binderCache;

            if (!SceneAssetDataBinders.Instance.TryGetBinderFromGuid(_guid, out var data)) return null;
            _binderCache = data;
            return data;
        }
    }
}
