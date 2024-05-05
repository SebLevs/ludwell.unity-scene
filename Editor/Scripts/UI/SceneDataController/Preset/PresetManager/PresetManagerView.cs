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

        private const string RemoveButtonName = "remove";
        private readonly Button _removeButton;
        
        private const string SelectedPresetLabelName = "selected-preset__label";
        private readonly Label _selectedPresetLabel;
        
        private const string OpenedPresetTextFieldName = "opened-listing__label";
        private readonly TextField _openedPresetTextField;
        private readonly EventCallback<ChangeEvent<string>> _onOpenedPresetTextFieldValueChanged;
        
        private VisualElement _root;

        public PresetManagerView(VisualElement root, 
            Action onReturnClicked,
            Action onDeselectClicked,
            Action onSelectClicked,
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
            _addButton.clicked += ExecuteAddButtonClicked;

            _removeButton = _root.Q<Button>(RemoveButtonName);
            _removeButton.clicked += ExecuteRemoveButtonClicked;
            
            _selectedPresetLabel = _root.Q<Label>(SelectedPresetLabelName);
            _openedPresetTextField = _root.Q<TextField>(OpenedPresetTextFieldName);
            _onOpenedPresetTextFieldValueChanged = onOpenedTextFieldValueChangedValueChanged;
            _openedPresetTextField.RegisterValueChangedCallback(ExecuteOpenedPresetValueChangedCallback);
        }

        ~PresetManagerView()
        {
            _returnButton.clicked -= ExecuteReturnButtonClicked;

            _deselectButton.clicked -= ExecuteDeselectButtonClicked;
            _selectButton.clicked -= ExecuteSelectButtonClicked;

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
        
        public void ShowSelectionContainer()
        {
            _selectedPresetContainerNull.style.display = DisplayStyle.None;
            _selectedPresetContainer.style.display = DisplayStyle.Flex;
        }

        public void ShowNullSelectionContainer()
        {
            _selectedPresetContainer.style.display = DisplayStyle.None;
            _selectedPresetContainerNull.style.display = DisplayStyle.Flex;
        }

        public void SetQuickLoadElementReferenceText(string value)
        {
            _referenceName.text = value;
        }
        
        public void SetSelectedPresetText(string value)
        {
            _selectedPresetLabel.text = value;
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

        private void ExecuteAddButtonClicked()
        {
            // _listViewHandler.ListView.itemsSource.Add();
        }

        private void ExecuteRemoveButtonClicked()
        {
            //_listViewHandler.ListView.itemsSource.removeAt();
        }

        private void ExecuteOpenedPresetValueChangedCallback(ChangeEvent<string> evt)
        {
            _onOpenedPresetTextFieldValueChanged?.Invoke(evt);
        }
    }
}