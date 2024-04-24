using System;
using UnityEngine.UIElements;

namespace Ludwell.Scene.Editor
{
    public class PresetSectionView
    {
        private const string SectionSelectionName = "section-selection";
        
        private readonly VisualElement _root;
        
        private readonly Button _settingsButton;
        private readonly Label _selectionLabel;

        private readonly Action _onSettingsClicked;
        
        public PresetSectionView(VisualElement root, Action onOptionClicked)
        {
            _root = root;
            
            _settingsButton = _root.Q<Button>();
            _onSettingsClicked = onOptionClicked;
            _settingsButton.clicked += OnClicked;
            
            _selectionLabel = _root.Q<Label>(SectionSelectionName);
        }

        ~PresetSectionView()
        {
            _settingsButton.clicked -= OnClicked;
        }
        
        public void SetSectionSelectionLabel(string value)
        {
            _selectionLabel.text = value;
        }

        private void OnClicked()
        {
            _onSettingsClicked?.Invoke();
        }
    }
}
