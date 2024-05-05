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
        private readonly Action _returnButtonBehaviour;

        private const string SelectedPresetContainerName = "selected-preset-container";
        private readonly VisualElement _selectedPresetContainer;
        private const string SelectedPresetContainerNullName = "selected-preset-container__null";
        private readonly VisualElement _selectedPresetContainerNull;

        private const string DeselectButtonName = "deselect";
        private readonly Button _deselectButton;
        private readonly Action _deselectButtonBehaviour;
        
        private const string SelectButtonName = "select";
        private readonly Button _selectButton;
        private readonly Action _selectButtonBehaviour;

        private const string AddButtonName = "add";
        private readonly Button _addButton;

        private const string RemoveButtonName = "remove";
        private readonly Button _removeButton;

        private VisualElement _root;

        public PresetManagerView(VisualElement root, 
            Action returnButtonBehaviour,
            Action deselectButtonButtonBehaviour,
            Action selectButtonButtonBehaviour)
        {
            _root = root;

            _referenceName = _root.Q<Label>(ReferenceName);

            _returnButton = _root.Q<Button>(ReturnButtonName);
            _returnButtonBehaviour = returnButtonBehaviour;
            _returnButton.clicked += ExecuteReturnButtonClicked;

            _selectedPresetContainer = _root.Q<VisualElement>(SelectedPresetContainerName);
            _selectedPresetContainerNull = _root.Q<VisualElement>(SelectedPresetContainerNullName);

            _deselectButton = _root.Q<Button>(DeselectButtonName);
            _deselectButtonBehaviour += deselectButtonButtonBehaviour;
            _deselectButton.clicked += ExecuteDeselectButtonClicked;
            
            _selectButton = _root.Q<Button>(SelectButtonName);
            _selectButtonBehaviour += selectButtonButtonBehaviour;
            _selectButton.clicked += ExecuteSelectButtonClicked;

            _addButton = _root.Q<Button>(AddButtonName);
            _addButton.clicked += ExecuteAddButtonClicked;

            _removeButton = _root.Q<Button>(RemoveButtonName);
            _removeButton.clicked += ExecuteRemoveButtonClicked;
        }

        ~PresetManagerView()
        {
            _returnButton.clicked -= ExecuteReturnButtonClicked;

            _deselectButton.clicked -= ExecuteDeselectButtonClicked;
            _selectButton.clicked -= ExecuteSelectButtonClicked;
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

        public void SetReferenceText(string value)
        {
            _referenceName.text = value;
        }

        private void ExecuteReturnButtonClicked()
        {
            _returnButtonBehaviour?.Invoke();
        }
        
        private void ExecuteDeselectButtonClicked()
        {
            _deselectButtonBehaviour?.Invoke();
            ShowNullSelectionContainer();
        }

        private void ExecuteSelectButtonClicked()
        {
            _selectButtonBehaviour?.Invoke();
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
    }
}