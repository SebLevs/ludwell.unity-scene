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
            Preset.SelectedPresetListing = presetListing;
        }

        public PresetListing GetSelectedPresetListing()
        {
            return Preset.SelectedPresetListing;
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
                AddPresetListing, 
                RemovePresetListing, 
                ShowPreviousPresetListing, 
                ShowNextPresetListing, 
                OnCurrentSelectionLabelChanged);
            
            _viewManager = _root.Root().Q<ViewManager>();
            _viewManager.Add(this);
            
            _selectedPresetlistViewHandler = new ListViewHandler<DataPresetElementController, JsonData>(
                _root.Q<ListView>(),
                null);

            _root.Root().RegisterCallback<KeyUpEvent>(OnKeyUpReturn);
        }

        private void OnCurrentSelectionLabelChanged(ChangeEvent<string> evt)
        {
            _openedPreset.Label = evt.newValue;

            if (_openedPreset == _model.Preset.SelectedPresetListing)
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

            _view.SetSelectedPresetEnabled(_model.Preset.SelectedPresetListing != null);

            var validPreset = _model.Preset.GetValidDataPreset();
            if (validPreset == null) 
            {
                // todo: Show an empty feedback like for list views
                _view.SetRemoveButtonEnabled(false);
                return; 
            }
            
            _view.SetRemoveButtonEnabled(true);
            _view.SetPresetListingEnabled(true);
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

            // _selectedPresetlistViewHandler = new ListViewHandler<DataPresetElementController, JsonData>(
            //     _root.Q<ListView>(),
            //     _openedPreset.JsonDataListing);

            _selectedPresetlistViewHandler.ListView.itemsSource = preset.JsonDataListing;
        }

        private void SelectPresetListing()
        {
            _model.SetSelectedPresetListing(_openedPreset);
            _view.SetSelectedPresetText(_model.GetSelectedPresetListing().Label);
        }
        
        private void AddPresetListing()
        {
            _model.Preset.PresetListings.Add(new PresetListing());
            OpenPresetListing(_model.Preset.PresetListings[^1]);
            
            // todo: update 00/00 in view
            _view.SetCurrentIndex(_model.Preset.PresetListings.Count.ToString());
            _view.SetCount(_model.Preset.PresetListings.Count.ToString());
            
            // if (_model.Preset.PresetListings.Count != 1) return;
            _view.SetRemoveButtonEnabled(true);
            _view.SetPresetListingEnabled(true);
        }

        private void RemovePresetListing()
        {
            if (_openedPreset == _model.GetSelectedPresetListing())
            {
                _model.Preset.SelectedPresetListing = null;
                _view.SetSelectedPresetEnabled(false);
            }
            
            var index = _model.Preset.PresetListings.IndexOf(_openedPreset);
            _model.Preset.PresetListings.RemoveAt(index);

            if (_model.Preset.PresetListings.Count > 0)
            {
                OpenPresetListing(_model.Preset.PresetListings[^1]);
                _view.SetCurrentIndex(_model.Preset.PresetListings.Count.ToString());
            }
            else
            {
                _view.SetRemoveButtonEnabled(false);
                _view.SetPresetListingEnabled(false);
            }
            
            // todo: update 00/00 in view
            _view.SetCount(_model.Preset.PresetListings.Count.ToString());
        }

        private void ShowPreviousPresetListing()
        {
            
        }

        private void ShowNextPresetListing()
        {
            
        }

        private void JumpToPresetListing(int index)
        {
            // todo: set logic for 00/00 item
        }

        private void DeselectPresetListing()
        {
            _model.SetSelectedPresetListing(null);
            _view.SetSelectedPresetText(string.Empty);
        }
    }
}