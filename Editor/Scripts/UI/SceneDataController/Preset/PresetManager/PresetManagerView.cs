using System;
using UnityEngine.UIElements;

namespace Ludwell.Scene.Editor
{
    public class PresetManagerView
    {
        private const string ReferenceName = "reference-name";
        private readonly Label _referenceName;

        private const string ReturnButtonName = "button__return";
        private readonly Button _returnButton;
        private readonly Action _onReturnClicked;

        private const string SelectedPresetContainerName = "selected-preset-container";
        private readonly VisualElement _selectedPresetContainer;
        private const string SelectedPresetContainerNullName = "selected-preset-container__null";
        private readonly VisualElement _selectedPresetContainerNull;

        private const string DeselectButtonName = "deselect";
        private readonly Button _deselectButton;
        private readonly Action _onDeselectClicked;
        
        private const string SelectButtonName = "select";
        private readonly Button _selectButton;
        private readonly Action _onSelectClicked;

        private const string AddButtonName = "add";
        private readonly Button _addButton;
        private readonly Action _onAddClicked;

        private const string RemoveButtonName = "remove";
        private readonly Button _removeButton;
        private readonly Action _onRemoveClicked;
        
        private const string PreviousButtonName = "previous";
        private readonly Button _previousButton;
        private readonly Action _onPreviousClicked;

        private const string NextButtonName = "next";
        private readonly Button _nextButton;
        private readonly Action _onNextClicked;
        
        private const string SelectedPresetLabelName = "selected-preset__label";
        private readonly Label _selectedPresetLabel;
        
        private const string OpenedPresetTextFieldName = "opened-listing__label";
        private readonly TextField _openedPresetTextField;
        private readonly EventCallback<ChangeEvent<string>> _onOpenedPresetTextFieldValueChanged;
        
        private const string ContentContainerName = "content-container";
        private readonly VisualElement _contentContainer;
        
        private const string ContentContainerEmptyName = "content-container__empty";
        private readonly VisualElement _contentContainerEmpty;
        
        private const string CurrentIndexName = "current-index";
        private readonly TextField _currentIndex;
        
        private const string CountName = "count";
        private readonly Label _count;
        
        private VisualElement _root;

        // todo: refactor to remove constructor parameters
        public PresetManagerView(VisualElement root, 
            Action onReturnClicked,
            Action onDeselectClicked,
            Action onSelectClicked,
            Action onAddClicked, 
            Action onRemoveClicked, 
            Action onPreviousClicked, 
            Action onNextClicked, 
            EventCallback<ChangeEvent<string>> onOpenedTextFieldValueChangedValueChanged)
        {
            _root = root;

            _referenceName = _root.Q<Label>(ReferenceName);

            _returnButton = _root.Q<Button>(ReturnButtonName);
            _onReturnClicked = onReturnClicked;
            _returnButton.clicked += ExecuteReturnButtonClicked;

            _selectedPresetContainer = _root.Q<VisualElement>(SelectedPresetContainerName);
            _selectedPresetContainerNull = _root.Q<VisualElement>(SelectedPresetContainerNullName);

            _deselectButton = _root.Q<Button>(DeselectButtonName);
            _onDeselectClicked += onDeselectClicked;
            _deselectButton.clicked += ExecuteDeselectButtonClicked;
            
            _selectButton = _root.Q<Button>(SelectButtonName);
            _onSelectClicked += onSelectClicked;
            _selectButton.clicked += ExecuteSelectButtonClicked;

            _addButton = _root.Q<Button>(AddButtonName);
            _onAddClicked += onAddClicked;
            _addButton.clicked += ExecuteAddButtonClicked;

            _removeButton = _root.Q<Button>(RemoveButtonName);
            _onRemoveClicked += onRemoveClicked;
            _removeButton.clicked += ExecuteRemoveButtonClicked;
            
            _previousButton = _root.Q<Button>(PreviousButtonName);
            _onPreviousClicked += onPreviousClicked;
            _previousButton.clicked += ExecutePreviousButtonClicked;

            _nextButton = _root.Q<Button>(NextButtonName);
            _onNextClicked += onNextClicked;
            _nextButton.clicked += ExecuteNextButtonClicked;
            
            _selectedPresetLabel = _root.Q<Label>(SelectedPresetLabelName);
            _openedPresetTextField = _root.Q<TextField>(OpenedPresetTextFieldName);
            _onOpenedPresetTextFieldValueChanged = onOpenedTextFieldValueChangedValueChanged;
            _openedPresetTextField.RegisterValueChangedCallback(ExecuteOpenedPresetValueChangedCallback);
            
            _contentContainer = _root.Q<VisualElement>(ContentContainerName);
            _contentContainerEmpty = _root.Q<VisualElement>(ContentContainerEmptyName);
            
            _currentIndex = _root.Q<TextField>(CurrentIndexName);
            _count = _root.Q<Label>(CountName);
        }

        ~PresetManagerView()
        {
            _returnButton.clicked -= ExecuteReturnButtonClicked;

            _deselectButton.clicked -= ExecuteDeselectButtonClicked;
            _selectButton.clicked -= ExecuteSelectButtonClicked;
            
            _previousButton.clicked -= ExecutePreviousButtonClicked;
            _nextButton.clicked -= ExecuteNextButtonClicked;

            _openedPresetTextField.UnregisterValueChangedCallback(ExecuteOpenedPresetValueChangedCallback);
        }

        public void Show()
        {
            _root.style.display = DisplayStyle.Flex;
        }

        public void Hide()
        {
            _root.style.display = DisplayStyle.None;
        }
        
        public void SetSelectedPresetVisualState(bool state)
        {
            if (state)
            {
                ShowSelectionContainer();
                return;
            }
            ShowNullSelectionContainer();
        }
        
        public void SetPresetListingVisualState(bool state)
        {
            if (state)
            {
                ShowContentContainer();
                return;
            }
            ShowNullContentContainer();
        }
        
        public void SetRemoveButtonEnabled(bool state)
        {
            _removeButton.SetEnabled(state);
        }

        public void SetQuickLoadElementReferenceText(string value)
        {
            _referenceName.text = value;
        }
        
        public void SetSelectedPresetText(string value)
        {
            _selectedPresetLabel.text = value;
        }

        public void SetCurrentIndex(string value)
        {
            _currentIndex.value = value;
        }

        public void SetCount(string value)
        {
            _count.text = value;
        }
        
        public void SetOpenedPresetText(string value)
        {
            _openedPresetTextField.value = value;
        }
        
        private void ExecuteReturnButtonClicked()
        {
            _onReturnClicked?.Invoke();
        }
        
        private void ExecuteDeselectButtonClicked()
        {
            _onDeselectClicked?.Invoke();
            ShowNullSelectionContainer();
        }

        private void ExecuteSelectButtonClicked()
        {
            _onSelectClicked?.Invoke();
            ShowSelectionContainer();
        }
        
        private void ShowContentContainer()
        {
            _contentContainerEmpty.style.display = DisplayStyle.None;
            _contentContainer.style.display = DisplayStyle.Flex;
        }

        private void ShowNullContentContainer()
        {
            _contentContainer.style.display = DisplayStyle.None;
            _contentContainerEmpty.style.display = DisplayStyle.Flex;
        }
        
        private void ShowSelectionContainer()
        {
            _selectedPresetContainerNull.style.display = DisplayStyle.None;
            _selectedPresetContainer.style.display = DisplayStyle.Flex;
        }

        private void ShowNullSelectionContainer()
        {
            _selectedPresetContainer.style.display = DisplayStyle.None;
            _selectedPresetContainerNull.style.display = DisplayStyle.Flex;
        }

        private void ExecuteAddButtonClicked()
        {
            _onAddClicked?.Invoke();
        }

        private void ExecuteRemoveButtonClicked()
        {
            _onRemoveClicked?.Invoke();
        }
        
        private void ExecutePreviousButtonClicked()
        {
            _onPreviousClicked?.Invoke();
        }
        
        private void ExecuteNextButtonClicked()
        {
            _onNextClicked?.Invoke();
        }

        private void ExecuteOpenedPresetValueChangedCallback(ChangeEvent<string> evt)
        {
            _onOpenedPresetTextFieldValueChanged?.Invoke(evt);
        }
    }
}