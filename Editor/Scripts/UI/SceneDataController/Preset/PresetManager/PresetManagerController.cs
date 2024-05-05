using UnityEngine;
using UnityEngine.UIElements;

namespace Ludwell.Scene.Editor
{
    public class PresetManagerViewArgs : ViewArgs
    {
        public PresetManagerViewArgs(QuickLoadElementData quickLoadElementData, Preset preset)
        {
            QuickLoadElementData = quickLoadElementData;
            Preset = preset;
        }

        public QuickLoadElementData QuickLoadElementData { get; }
        public Preset Preset { get; }
    }

    public class PresetManagerController : IViewable
    {
        private const string CurrentSelectionLabelName = "current-selection-label";
        
        private ViewManager _viewManager;

        private Preset _model;
        private PresetManagerView _view;
        private VisualElement _root;
        
        private ListViewHandler<DataPresetElementController, JsonData> _selectedPresetlistViewHandler;

        private TextField _currentSelectionLabel;
        private PresetJsonDataListing _openedPreset;

        public PresetManagerController(VisualElement parent)
        {
            _root = parent.Q(nameof(PresetManagerView));
            _view = new PresetManagerView(_root, ReturnToPreviousView);

            _viewManager = _root.Root().Q<ViewManager>();
            _viewManager.Add(this);
            
            _currentSelectionLabel = _root.Q<TextField>(CurrentSelectionLabelName);
            _currentSelectionLabel.RegisterValueChangedCallback(OnCurrentSelectionLabelChanged);
            
            InitializeReturnEvent();
        }

        private void OnCurrentSelectionLabelChanged(ChangeEvent<string> evt)
        {
            _openedPreset.Label = evt.newValue;
            // todo: delayed save
        }

        public void Show(ViewArgs args)
        {
            _view.Show();

            var presetManagerViewArgs = (PresetManagerViewArgs)args;
            _view.SetReferenceText(presetManagerViewArgs.QuickLoadElementData.Name);
            _model = presetManagerViewArgs.Preset;
            _openedPreset = _model.GetValidDataPreset();

            if (_model != null)
            {
                _currentSelectionLabel.value = _model.GetSelectedPresetLabel;
            }

            _selectedPresetlistViewHandler = new ListViewHandler<DataPresetElementController, JsonData>(
                _root.Q<ListView>(),
                presetManagerViewArgs.QuickLoadElementData.DataPreset.GetValidDataPreset().JsonDataListing);
        }

        public void Hide()
        {
            _view.Hide();
        }

        private void InitializeReturnEvent()
        {
            _root.Root().RegisterCallback<KeyUpEvent>(OnKeyUpReturn);
        }

        private void OnKeyUpReturn(KeyUpEvent evt)
        {
            if (_root.style.display == DisplayStyle.None) return;

            if (evt.keyCode == KeyCode.Escape)
            {
                ReturnToPreviousView();
            }
        }

        private void ReturnToPreviousView()
        {
            _viewManager.TransitionToPreviousView();
        }
    }
}