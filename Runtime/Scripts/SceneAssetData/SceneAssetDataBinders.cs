using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ludwell.SceneManagerToolkit
{
    public class SceneAssetDataBinders : ScriptableObject
    {
        private static SceneAssetDataBinders _instance;

        public const string NotAddressableName = "NA";

        public List<SceneAssetDataBinder> Elements = new();

        public static SceneAssetDataBinders Instance
        {
            get
            {
                if (_instance) return _instance;
                _instance = Resources.Load<SceneAssetDataBinders>(nameof(SceneAssetDataBinders));
                return _instance;
            }
        }

        public void ResetAddresses()
        {
            foreach (var binder in Elements)
            {
                binder.Data.AddressableID = NotAddressableName;
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

        public int IndexOf(SceneAssetData data)
        {
            for (var index = 0; index < Elements.Count; index++)
            {
                var binder = Elements[index];
                if (data == binder.Data) return index;
            }

            return -1;
        }

        public int IndexOfGuid(string guid)
        {
            for (var index = 0; index < Elements.Count; index++)
            {
                var binder = Elements[index];
                if (string.Equals(guid, binder.Data.GUID)) return index;
            }

            return -1;
        }

        public bool ContainsWithId(string id)
        {
            foreach (var binder in Elements)
            {
                if (string.Equals(id, binder.Data.GUID, StringComparison.InvariantCulture)) return true;
            }

            return false;
        }

        public bool Add(string guid, string assetName, string path, string addressableID = NotAddressableName)
        {
            if (ContainsWithId(guid)) return false;
            var newBinder = new SceneAssetDataBinder
            {
                Data = new SceneAssetData
                {
                    GUID = guid,
                    Name = assetName,
                    Path = path,
                    AddressableID = addressableID
                }
            };

            Elements.Add(newBinder);
            Elements.Sort();
            return true;
        }

        public void Remove(string id)
        {
            foreach (var binder in Elements)
            {
                if (!string.Equals(id, binder.Data.GUID, StringComparison.InvariantCulture)) continue;
                Elements.Remove(binder);
                return;
            }
        }

        public SceneAssetDataBinder GetBinderFromId(string id)
        {
            foreach (var sceneAssetDataBinder in Elements)
            {
                if (!string.Equals(sceneAssetDataBinder.Data.GUID, id, StringComparison.InvariantCulture)) continue;
                return sceneAssetDataBinder;
            }

            return null;
        }

        public bool TryGetBinderFromId(string id, out SceneAssetDataBinder binder)
        {
            foreach (var sceneAssetDataBinder in Elements)
            {
                if (!string.Equals(sceneAssetDataBinder.Data.GUID, id, StringComparison.InvariantCulture)) continue;
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
                if (!string.Equals(sceneAssetDataBinder.Data.GUID, id, StringComparison.InvariantCulture)) continue;
                return sceneAssetDataBinder.Data;
            }

            return null;
        }

        public bool TryGetDataFromId(string id, out SceneAssetData data)
        {
            foreach (var sceneAssetDataBinder in Elements)
            {
                if (!string.Equals(sceneAssetDataBinder.Data.GUID, id, StringComparison.InvariantCulture)) continue;
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

        /// <param name="tag">
        /// Parse <see cref="SceneAssetData"/>s for the specified tag.<br/>
        /// Not case-sensitive.
        /// </param>
        /// <returns>Will return null if no <see cref="SceneAssetData"/> is found with the specified <see cref="Tag"/>.</returns>
        public IEnumerable<SceneAssetData> GetDataWhereTag(string tag)
        {
            foreach (var sceneAssetDataBinder in Elements)
            {
                if (!sceneAssetDataBinder.Tags.Exists(x => string.Equals(x.ID, tag, StringComparison.InvariantCultureIgnoreCase))) continue;
                yield return sceneAssetDataBinder.Data;
            }

            yield return null;
        }
    }
}
