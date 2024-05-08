using System.Linq;
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
        
        public int IndexOf(PresetListing presetListing) => Preset.PresetListings.IndexOf(presetListing);
        public int IndexOfPlusOne(PresetListing presetListing) => Preset.PresetListings.IndexOf(presetListing) + 1;

        public int PresetListingCount => Preset.PresetListings.Count;

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

            _view.SetSelectedPresetVisualState(_model.Preset.SelectedPresetListing != null);

            var validPreset = _model.Preset.GetValidDataPreset();
            if (validPreset == null) 
            {
                _view.SetRemoveButtonEnabled(false);
                _view.SetPresetListingVisualState(false);
                _view.SetEmptyCount();
                return;
            }
            
            _view.SetRemoveButtonEnabled(true);
            _view.SetPresetListingVisualState(true);
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

        private void OpenPresetListing(PresetListing presetListing)
        {
            _openedPreset = presetListing;
            _view.SetOpenedPresetText(_openedPreset.Label);
            _selectedPresetlistViewHandler.ListView.itemsSource = presetListing.JsonDataListing;
            
            var index = _model.IndexOfPlusOne(presetListing);
            _view.SetCurrentIndex(index.ToString());
        }

        private void SelectPresetListing()
        {
            _model.SetSelectedPresetListing(_openedPreset);
            _view.SetSelectedPresetText(_model.GetSelectedPresetListing().Label);
        }
        
        private void AddPresetListing()
        {
            var presetListings = _model.Preset.PresetListings;
            presetListings.Add(new PresetListing());
            
            OpenPresetListing(_model.Preset.PresetListings[^1]);
            _view.SetCount(_model.PresetListingCount.ToString());
            
            if (presetListings.Count != 1) return;
            _view.SetRemoveButtonEnabled(true);
            _view.SetPresetListingVisualState(true);
        }

        private void RemovePresetListing()
        {
            if (_openedPreset == _model.GetSelectedPresetListing())
            {
                _model.Preset.ClearSelection();
                _view.SetSelectedPresetVisualState(false);
            }
            
            var presetListings = _model.Preset.PresetListings;
            var index = presetListings.IndexOf(_openedPreset);
            presetListings.RemoveAt(index);

            if (presetListings.Count > 0)
            {
                OpenPresetListing(presetListings[^1]);
                _view.SetCount(presetListings.Count.ToString());
            }
            else
            {
                _view.SetRemoveButtonEnabled(false);
                _view.SetPresetListingVisualState(false);
                _view.SetEmptyCount();
            }
        }

        private void ShowPreviousPresetListing()
        {
            var index = _model.IndexOf(_openedPreset);
            OpenPresetListing(_model.Preset.PresetListings[index]);
            
            if (index + 1 > 1) return;
            // todo: prevent click on previous arrow
        }

        private void ShowNextPresetListing()
        {
            var index = _model.IndexOf(_openedPreset);
            OpenPresetListing(_model.Preset.PresetListings[index]);
            
            if (index + 1 < _model.PresetListingCount) return;
            // todo: prevent click on next arrow
        }

        private void JumpToPresetListing(int index)
        {
            if (_model.PresetListingCount == 0)
            {
                _view.SetEmptyCount();
                return;
            }
            
            if (index > _model.PresetListingCount || index < 1)
            {
                var openedPresetIndex = _model.IndexOf(_openedPreset) + 1;
                _view.SetCurrentIndex(openedPresetIndex.ToString());
                return;
            }
            
            OpenPresetListing(_model.Preset.PresetListings[index - 1]);
        }

        private void DeselectPresetListing()
        {
            _model.SetSelectedPresetListing(null);
            _view.SetSelectedPresetText(string.Empty);
        }
    }
}