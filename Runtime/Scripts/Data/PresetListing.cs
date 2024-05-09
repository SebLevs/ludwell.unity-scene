using System;
using System.Collections.Generic;

namespace Ludwell.Scene
{
    [Serializable]
    public class PresetListing : IComparable
    {
        public string Label = "Not labeled";
        public List<JsonData> JsonDataListing = new();

        // todo: replace + button with this
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

        // todo: replace - button with this
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

            var otherAsType = obj as PresetListing;
            return string.Compare(Label, otherAsType.Label, StringComparison.Ordinal);
        }
    }
}
