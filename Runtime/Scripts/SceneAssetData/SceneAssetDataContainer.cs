using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ludwell.Scene
{
    public class SceneAssetDataContainer : ScriptableObject
    {
        private static SceneAssetDataContainer _instance;

        public List<SceneAssetDataBinder> DataBinders = new();

        public static SceneAssetDataContainer Instance
        {
            get
            {
                if (_instance != null) return _instance;
                _instance = Resources.Load<SceneAssetDataContainer>(nameof(SceneAssetDataContainer));
                return _instance;
            }
        }

        public int IndexOf(SceneAssetDataBinder sceneAssetDataBinder)
        {
            for (var index = 0; index < DataBinders.Count; index++)
            {
                var binder = DataBinders[index];
                if (sceneAssetDataBinder.Equals(binder)) return index;
            }

            return -1;
        }

        public bool Contains(string id)
        {
            foreach (var binder in DataBinders)
            {
                if (string.Equals(id, binder.ID, StringComparison.InvariantCulture)) return true;
            }

            return false;
        }

        public void Add(SceneAssetDataBinder newBinder)
        {
            foreach (var binder in DataBinders)
            {
                if (!string.Equals(newBinder.ID, binder.ID, StringComparison.InvariantCulture)) continue;
                return;
            }

            DataBinders.Add(newBinder);
            DataBinders.Sort();
        }

        public void Remove(string id)
        {
            foreach (var binder in DataBinders)
            {
                if (!string.Equals(id, binder.ID, StringComparison.InvariantCulture)) continue;
                DataBinders.Remove(binder);
                return;
            }
        }

        public SceneAssetDataBinder GetBinder(string id)
        {
            if (string.IsNullOrEmpty(id)) throw new Exception("Suspicious action | Binder id parameter is invalid");
            
            foreach (var sceneAssetDataBinder in DataBinders)
            {
                if (!string.Equals(sceneAssetDataBinder.ID, id, StringComparison.InvariantCulture)) continue;
                return sceneAssetDataBinder;
            }

            return null;
        }

        public SceneAssetDataBinder GetBinderFromPath(string path)
        {
            foreach (var sceneAssetDataBinder in DataBinders)
            {
                if (!string.Equals(sceneAssetDataBinder.Data.Path, path, StringComparison.InvariantCulture)) continue;
                return sceneAssetDataBinder;
            }

            return null;
        }
        
        public bool TryGetBinder(string id, out SceneAssetDataBinder binder)
        {
            foreach (var sceneAssetDataBinder in DataBinders)
            {
                if (!string.Equals(sceneAssetDataBinder.ID, id, StringComparison.InvariantCulture)) continue;
                binder = sceneAssetDataBinder;
                return true;
            }

            binder = null;
            return false;
        }

        public SceneAssetData GetData(string id)
        {
            foreach (var sceneAssetDataBinder in DataBinders)
            {
                if (!string.Equals(sceneAssetDataBinder.ID, id, StringComparison.InvariantCulture)) continue;
                return sceneAssetDataBinder.Data;
            }

            return null;
        }

        public bool TryGetData(string key, out SceneAssetData data)
        {
            foreach (var sceneAssetDataBinder in DataBinders)
            {
                if (!string.Equals(sceneAssetDataBinder.ID, key, StringComparison.InvariantCulture)) continue;
                data = sceneAssetDataBinder.Data;
                return true;
            }

            data = null;
            return false;
        }
    }
}