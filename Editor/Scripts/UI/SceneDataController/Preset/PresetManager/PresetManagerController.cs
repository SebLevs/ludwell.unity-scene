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

        public void SetSelectedPresetListing(PresetListing presetListing)
        {
            Preset.SelectedPreset = presetListing;
        }

        public PresetListing GetSelectedPresetListing()
        {
            return Preset.SelectedPreset;
        }
    }

    public class PresetManagerController : IViewable
    {
        private ViewManager _viewManager;

        private PresetManagerViewArgs _model;
        private PresetManagerView _view;
        private VisualElement _root;

        private ListViewHandler<DataPresetElementController, JsonData> _selectedPresetlistViewHandler;

        private PresetListing _openedPreset;

        public PresetManagerController(VisualElement parent)
        {
            _root = parent.Q(nameof(PresetManagerView));
            _view = new PresetManagerView(_root, 
                ReturnToPreviousView, 
                DeselectPresetListing, 
                SelectPresetListing, 
                OnCurrentSelectionLabelChanged);

            _viewManager = _root.Root().Q<ViewManager>();
            _viewManager.Add(this);

            _root.Root().RegisterCallback<KeyUpEvent>(OnKeyUpReturn);
        }

        private void OnCurrentSelectionLabelChanged(ChangeEvent<string> evt)
        {
            _openedPreset.Label = evt.newValue;

            if (_openedPreset == _model.Preset.SelectedPreset)
            {
                _view.SetSelectedPresetText(_openedPreset.Label);
            }

            DataFetcher.SaveQuickLoadElementsDelayed();
        }

        public void Show(ViewArgs args)
        {
            _view.Show();

            _model = (PresetManagerViewArgs)args;
            _view.SetQuickLoadElementReferenceText(_model.QuickLoadElementData.Name);

            if (_model.Preset.SelectedPreset == null)
            {
                _view.ShowNullSelectionContainer();
            }
            else
            {
                _view.ShowSelectionContainer();
            }

            var validPreset = _model.Preset.GetValidDataPreset();
            if (validPreset == null) return;
            OpenPresetListing(validPreset);
        }

        public void Hide()
        {
            _view.Hide();
        }

        private void ReturnToPreviousView()
        {
            _viewManager.TransitionToPreviousView();
        }

        private void OnKeyUpReturn(KeyUpEvent evt)
        {
            if (_root.style.display == DisplayStyle.None) return;

            if (evt.keyCode == KeyCode.Escape)
            {
                ReturnToPreviousView();
            }
        }

        private void OpenPresetListing(PresetListing preset)
        {
            _openedPreset = preset;

            _view.SetOpenedPresetText(_openedPreset.Label);

            _selectedPresetlistViewHandler = new ListViewHandler<DataPresetElementController, JsonData>(
                _root.Q<ListView>(),
                _openedPreset.JsonDataListing);
        }

        private void SelectPresetListing()
        {
            _model.SetSelectedPresetListing(_openedPreset);
            _view.SetSelectedPresetText(_model.GetSelectedPresetListing().Label);
        }

        private void DeselectPresetListing()
        {
            _model.SetSelectedPresetListing(null);
            _view.SetSelectedPresetText(string.Empty);
        }
    }
}