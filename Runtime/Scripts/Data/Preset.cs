using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ludwell.Scene
{
    [Serializable]
    public class Preset
    {
        [field: HideInInspector] public PresetListing SelectedPresetListing { get; private set; } = null;
        [HideInInspector] public List<PresetListing> PresetListings = new();

        public void SetSelectedPresetListing(PresetListing presetListing)
        {
            SelectedPresetListing = presetListing;
        }

        public PresetListing GetValidDataPresetListings()
        {
            if (SelectedPresetListing != null) return SelectedPresetListing;
            return PresetListings.Count > 0 ? PresetListings[0] : null;
        }

        public void ClearSelection()
        {
            SelectedPresetListing = null;
        }

        public void SetPresetListings()
        {
            foreach (var presetListing in SelectedPresetListing.JsonDataListing)
            {
                Debug.LogError("Copy to original: " + presetListing.Original.name);
                presetListing.CopyDataToOriginal();
            }
        }

        public void RevertPresetListings()
        {
            foreach (var presetListing in SelectedPresetListing.JsonDataListing)
            {
                Debug.LogError("Revert: " +presetListing.Original.name);
            }
        }
    }
}
