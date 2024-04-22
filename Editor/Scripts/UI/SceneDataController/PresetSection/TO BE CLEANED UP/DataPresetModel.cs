using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ludwell.Scene.Editor
{
    public class DataPresetModel
    {
        private Dictionary<Object, string> _presets = new();

        public void AddPreset(Object obj)
        {
            _presets.TryAdd(obj, JsonUtility.ToJson(obj));
        }

        public void RemovePreset(Object preset)
        {
            _presets.Remove(preset);
        }

        public void ClearMissingReferences()
        {
            _presets = _presets
                .Where(x => x.Key != null)
                .ToDictionary(x => x.Key, x => x.Value);
        }
    }
}