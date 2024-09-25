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

    /// <summary>
    /// Provides access to the relevant data and tags of the referenced SceneAsset.<br/>
    /// <remarks>Equality operates with <see cref="_guid"/></remarks>
    /// </summary>
    [Serializable]
    public partial class SceneAssetReference : IEquatable<SceneAssetReference>
    {
        /// <summary>
        /// The SceneAsset GUID.<br/>
        /// Used in the methods of this class to return information about the referenced SceneAsset.
        /// </summary>
        [SerializeField] private string _guid;

        private SceneAssetDataBinder _binderCache;

        public bool IsEmpty => string.IsNullOrEmpty(_guid);

        public bool IsValid => !IsEmpty && SceneAssetDataBinders.Instance.ContainsWithGuid(_guid);

        public string Guid => _guid;

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

        public static bool operator ==(SceneAssetReference left, SceneAssetReference right)
        {
            if (left.IsEmpty || right.IsEmpty) return false;
            return left.Guid == right.Guid;
        }

        public static bool operator !=(SceneAssetReference left, SceneAssetReference right)
        {
            if (left.IsEmpty || right.IsEmpty) return true;
            return left.Guid != right.Guid;
        }

        public bool Equals(SceneAssetReference other)
        {
            if (other.IsEmpty) return false;
            if (IsEmpty) return false;
            return _guid == other._guid;
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (obj.GetType() != GetType()) return false;
            return Equals((SceneAssetReference)obj);
        }

        public override int GetHashCode()
        {
            return _guid != null ? _guid.GetHashCode() : 0;
        }
    }
}
