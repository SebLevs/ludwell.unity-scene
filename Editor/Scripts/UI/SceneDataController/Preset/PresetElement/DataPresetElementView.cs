using UnityEngine.UIElements;

namespace Ludwell.Scene.Editor
{
    public class DataPresetElementView
    {
        private VisualElement _view;
        
        private Toggle _toggle;

        public DataPresetElementView(VisualElement parent)
        {
            _view = parent.Q(nameof(DataPresetElementView));
            _toggle = _view.Q<Toggle>();
        }

        public void UpdateToggleLabel(string value)
        {
            _toggle.label = value;
        }
    }
}
