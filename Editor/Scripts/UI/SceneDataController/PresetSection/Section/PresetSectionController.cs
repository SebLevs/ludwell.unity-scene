using System;
using UnityEngine.UIElements;

namespace Ludwell.Scene.Editor
{
    public class PresetSectionController
    {
        private QuickLoadElementData _model;
        private PresetSectionView _view;

        public PresetSectionController(QuickLoadElementData model, VisualElement root, Action onClicked)
        {
            _model = model;
            _view = new PresetSectionView(root, onClicked);
        }
        
        public void SetSectionSelectionLabel(string value)
        {
            // todo: set label from _model preset name
            _view.SetSectionSelectionLabel(value);
        }
    }
}
