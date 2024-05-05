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
        private const string OpenedPresetListingName = "opened-listing__label";
        private const string SelectedPresetLabelName = "selected-preset__label";

        private ViewManager _viewManager;

        private PresetManagerViewArgs _model;
        private PresetManagerView _view;
        private VisualElement _root;

        private ListViewHandler<DataPresetElementController, JsonData> _selectedPresetlistViewHandler;

        private Label _selectedPresetLabel;

        private TextField _openedSelectionLabel;
        private PresetListing _openedPreset;

        public PresetManagerController(VisualElement parent)
        {
            _root = parent.Q(nameof(PresetManagerView));
            _view = new PresetManagerView(_root, ReturnToPreviousView, DeselectPresetListing, SelectPresetListing);

            _viewManager = _root.Root().Q<ViewManager>();
            _viewManager.Add(this);

            _selectedPresetLabel = _root.Q<Label>(SelectedPresetLabelName);


            _openedSelectionLabel = _root.Q<TextField>(OpenedPresetListingName);
            _openedSelectionLabel.RegisterValueChangedCallback(OnCurrentSelectionLabelChanged);

            _root.Root().RegisterCallback<KeyUpEvent>(OnKeyUpReturn);
        }

        private void OnCurrentSelectionLabelChanged(ChangeEvent<string> evt)
        {
            _openedPreset.Label = evt.newValue; // todo: place into view

            if (_openedPreset == _model.Preset.SelectedPreset)
            {
                // todo: change selection label from view here
            }
            // todo: delayed save
        }

        public void Show(ViewArgs args)
        {
            _view.Show();

            _model = (PresetManagerViewArgs)args;
            _view.SetReferenceText(_model.QuickLoadElementData.Name);

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

            _openedSelectionLabel.value = _openedPreset.Label; // todo: place into view

            _selectedPresetlistViewHandler = new ListViewHandler<DataPresetElementController, JsonData>(
                _root.Q<ListView>(),
                _openedPreset.JsonDataListing);
        }

        private void SelectPresetListing()
        {
            _model.SetSelectedPresetListing(_openedPreset);
            _selectedPresetLabel.text = _model.GetSelectedPresetListing().Label;
        }

        private void DeselectPresetListing()
        {
            _model.SetSelectedPresetListing(null);
            _selectedPresetLabel.text = "";
        }
    }
}