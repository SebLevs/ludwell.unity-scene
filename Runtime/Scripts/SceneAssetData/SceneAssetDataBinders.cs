using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ludwell.Scene
{
    public class SceneAssetDataBinders : ScriptableObject
    {
        private static SceneAssetDataBinders _instance;

        public List<SceneAssetDataBinder> Elements = new();

        public static SceneAssetDataBinders Instance
        {
            get
            {
                if (_instance != null) return _instance;
                _instance = Resources.Load<SceneAssetDataBinders>(nameof(SceneAssetDataBinders));
                return _instance;
            }
        }

        public int IndexOf(SceneAssetDataBinder sceneAssetDataBinder)
        {
            for (var index = 0; index < Elements.Count; index++)
            {
                var binder = Elements[index];
                if (sceneAssetDataBinder.Equals(binder)) return index;
            }

            return -1;
        }

        public bool ContainsWithId(string id)
        {
            foreach (var binder in Elements)
            {
                if (string.Equals(id, binder.ID, StringComparison.InvariantCulture)) return true;
            }

            return false;
        }

        public void Add(SceneAssetDataBinder newBinder)
        {
            foreach (var binder in Elements)
            {
                if (!string.Equals(newBinder.ID, binder.ID, StringComparison.InvariantCulture)) continue;
                return;
            }

            Elements.Add(newBinder);
            Elements.Sort();
        }

        public void Remove(string id)
        {
            foreach (var binder in Elements)
            {
                if (!string.Equals(id, binder.ID, StringComparison.InvariantCulture)) continue;
                Elements.Remove(binder);
                return;
            }
        }

        public SceneAssetDataBinder GetBinderFromId(string id)
        {
            foreach (var sceneAssetDataBinder in Elements)
            {
                if (!string.Equals(sceneAssetDataBinder.ID, id, StringComparison.InvariantCulture)) continue;
                return sceneAssetDataBinder;
            }

            return null;
        }

        public bool TryGetBinderFromId(string id, out SceneAssetDataBinder binder)
        {
            foreach (var sceneAssetDataBinder in Elements)
            {
                if (!string.Equals(sceneAssetDataBinder.ID, id, StringComparison.InvariantCulture)) continue;
                binder = sceneAssetDataBinder;
                return true;
            }

            binder = null;
            return false;
        }

        public SceneAssetDataBinder GetBinderFromPath(string path)
        {
            foreach (var sceneAssetDataBinder in Elements)
            {
                if (!string.Equals(sceneAssetDataBinder.Data.Path, path, StringComparison.InvariantCulture)) continue;
                return sceneAssetDataBinder;
            }

            return null;
        }

        public bool TryGetBinderFromPath(string path, out SceneAssetDataBinder binder)
        {
            foreach (var sceneAssetDataBinder in Elements)
            {
                if (!string.Equals(sceneAssetDataBinder.Data.Path, path, StringComparison.InvariantCulture)) continue;
                binder = sceneAssetDataBinder;
                return true;
            }

            binder = null;
            return false;
        }

        public SceneAssetData GetDataFromId(string id)
        {
            foreach (var sceneAssetDataBinder in Elements)
            {
                if (!string.Equals(sceneAssetDataBinder.ID, id, StringComparison.InvariantCulture)) continue;
                return sceneAssetDataBinder.Data;
            }

            return null;
        }

        public bool TryGetDataFromId(string id, out SceneAssetData data)
        {
            foreach (var sceneAssetDataBinder in Elements)
            {
                if (!string.Equals(sceneAssetDataBinder.ID, id, StringComparison.InvariantCulture)) continue;
                data = sceneAssetDataBinder.Data;
                return true;
            }

            data = null;
            return false;
        }

        public SceneAssetData GetDataFromPath(string path)
        {
            foreach (var sceneAssetDataBinder in Elements)
            {
                if (!string.Equals(sceneAssetDataBinder.Data.Path, path, StringComparison.InvariantCulture)) continue;
                return sceneAssetDataBinder.Data;
            }

            return null;
        }

        public bool TryGetDataFromPath(string path, out SceneAssetData data)
        {
            foreach (var sceneAssetDataBinder in Elements)
            {
                if (!string.Equals(sceneAssetDataBinder.Data.Path, path, StringComparison.InvariantCulture)) continue;
                data = sceneAssetDataBinder.Data;
                return true;
            }

            data = null;
            return false;
        }

        /// <param name="tag">Parse <see cref="SceneAssetData"/> list for the specified tag. Case sensitive.</param>
        /// <returns>Will return null if no <see cref="SceneAssetData"/> was found with specified <see cref="Tag"/>.</returns>
        public IEnumerable<SceneAssetData> GetDataWhereTag(string tag)
        {
            foreach (var sceneAssetDataBinder in Elements)
            {
                if (!sceneAssetDataBinder.Tags.Exists(x => x.ID == tag)) continue;
                yield return sceneAssetDataBinder.Data;
            }

            yield return null;
        }
    }
}