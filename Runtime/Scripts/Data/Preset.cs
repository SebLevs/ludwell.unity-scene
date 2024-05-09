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
            if (PresetListings.Count > 0) return PresetListings[0];
            return null;
        }

        public void ClearSelection()
        {
            SelectedPresetListing = null;
        }
    }
}
