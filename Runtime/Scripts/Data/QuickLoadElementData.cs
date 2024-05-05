using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Ludwell.Scene
{
    [Serializable]
    public class QuickLoadElementData : TagSubscriberWithTags, IComparable
    {
        [HideInInspector] public SceneData SceneData;
        [HideInInspector] public bool IsOpen = true;
        [HideInInspector] public bool IsOutsideAssetsFolder;

        [HideInInspector] public Preset DataPreset;

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;

            var otherAsType = obj as QuickLoadElementData;
            return string.Compare(Name, otherAsType.Name, StringComparison.Ordinal);
        }
    }

    [Serializable]
    public class Preset
    {
        [HideInInspector] public PresetJsonDataListing SelectedPreset;
        [HideInInspector] public List<PresetJsonDataListing> Presets = new();

        public string GetSelectedPresetLabel => SelectedPreset.Label;
        
        public PresetJsonDataListing GetValidDataPreset()
        {
            if (SelectedPreset != null) return SelectedPreset;
            return Presets.Any() ? Presets[0] : null;
        }
    }

    [Serializable]
    public class PresetJsonDataListing : IComparable
    {
        public string Label = "Not labeled";
        public List<JsonData> JsonDataListing = new();

        public void AddToSelectedPreset(string json)
        {
            // var data = JsonConvert.DeserializeObject<dynamic>(json);
            //
            // foreach (var jsonData in SelectedPreset)
            // {
            //     if (jsonData.Original.GetType() != data.GetType()) continue;
            //     Debug.LogWarning("The preset already contains the element.");
            //     return;
            // }
            //
            JsonDataListing.Add(new JsonData { Json = json });
        }

        public void RemoveFromSelectedPreset(string json)
        {
            foreach (var dataPreset in JsonDataListing)
            {
                if (dataPreset.Json != json) continue;
                JsonDataListing.Remove(dataPreset);
                return;
            }
        }
        
        public int CompareTo(object obj)
        {
            if (obj == null) return 1;

            var otherAsType = obj as PresetJsonDataListing;
            return string.Compare(Label, otherAsType.Label, StringComparison.Ordinal);
        }
    }

    [Serializable]
    public class JsonData
    {
        public Object Original;
        public string Json;
        
        private void CopyDataToOriginal(Object source)
        {
            // todo: cache the original values
            // todo: reapply original values when play mode has exited
            EditorUtility.CopySerializedIfDifferent(source, Original);
        }
        
        
        // CACHE
        private void CopyData(Object source, Object target)
        {
            var originalName = target.name;
            EditorUtility.CopySerializedIfDifferent(source, target);
            target.name = originalName;
        }
    }
}