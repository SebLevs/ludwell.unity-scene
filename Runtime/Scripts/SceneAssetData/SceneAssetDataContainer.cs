using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ludwell.Scene
{
    public class SceneAssetDataContainer : ScriptableObject
    {
        private static SceneAssetDataContainer _instance;

        [SerializeField] private List<SceneAssetDataBinder> _dataBinders = new();

        public static SceneAssetDataContainer Instance
        {
            get
            {
                if (_instance != null) return _instance;
                _instance = Resources.Load<SceneAssetDataContainer>(nameof(SceneAssetDataContainer));
                return _instance;
            }
        }

        public bool Contains(string key)
        {
            foreach (var binder in _dataBinders)
            {
                if (string.Equals(key, binder.Key, StringComparison.InvariantCulture)) return true;
            }

            return false;
        }

        public void Add(SceneAssetDataBinder newBinder)
        {
            foreach (var binder in _dataBinders)
            {
                if (!string.Equals(newBinder.Key, binder.Key, StringComparison.InvariantCulture)) continue;
                return;
            }

            _dataBinders.Add(newBinder);
            _dataBinders.Sort();
        }

        public void Remove(string key)
        {
            foreach (var binder in _dataBinders)
            {
                if (!string.Equals(key, binder.Key, StringComparison.InvariantCulture)) continue;
                _dataBinders.Remove(binder);
                return;
            }
        }

        public SceneAssetData GetData(string key)
        {
            foreach (var sceneAssetDataBinder in _dataBinders)
            {
                if (string.Equals(sceneAssetDataBinder.Key, key, StringComparison.InvariantCulture))
                {
                    return sceneAssetDataBinder.Data;
                }
            }

            return null;
        }

        public bool TryGetValue(string key, out SceneAssetData data)
        {
            foreach (var sceneAssetDataBinder in _dataBinders)
            {
                if (string.Equals(sceneAssetDataBinder.Key, key, StringComparison.InvariantCulture))
                {
                    data = sceneAssetDataBinder.Data;
                    return true;
                }
            }

            data = null;
            return false;
        }
    }
}