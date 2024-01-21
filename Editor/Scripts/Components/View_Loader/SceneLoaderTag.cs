using UnityEngine.UIElements;

namespace Ludwell.Scene
{
    public class SceneLoaderTag : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<SceneLoaderTag, UxmlTraits> { }
        
        private const string UxmlPath = "Uxml/" + nameof(LoaderController) + "/" + nameof(SceneLoaderTag);
        private const string UssPath = "Uss/" + nameof(LoaderController) + "/" + nameof(SceneLoaderTag);

        private const string RemoveButtonName = "button-remove";
        private const string MainButtonName = "button-main";
        private const string SearchButtonName = "button-search";

        private static SceneLoaderTag _currentSelection;
        
        private Button _removeButton;
        private Button _mainButton;
        private Button _searchButton;

        private SceneLoaderTagController _tagController;

        public SceneLoaderTag()
        {
            this.AddHierarchyFromUxml(UxmlPath);
            this.AddStyleFromUss(UssPath);
            
            SetReferences();
            SetButtonEvents();
            
            ToggleBehaviourButtons(DisplayStyle.None);
        }

        public string GetTagName => _mainButton.text;
        public void SetTagName(string value)
        {
            _mainButton.text = value;
        }

        private void SetReferences()
        {
            _removeButton = this.Q<Button>(RemoveButtonName);
            _mainButton = this.Q<Button>(MainButtonName);
            _searchButton = this.Q<Button>(SearchButtonName);

            RegisterCallback<AttachToPanelEvent>(_ =>
            {
                _tagController = GetFirstAncestorOfType<SceneLoaderTagController>();
            });
        }

        private void SetButtonEvents()
        {
            _mainButton.RegisterCallback<ClickEvent>(_ =>
            {
                SelectTag(this);
            });
            
            _removeButton.RegisterCallback<ClickEvent>(_ =>
            {
                _tagController.Remove(this);
            });
            
            _searchButton.RegisterCallback<ClickEvent>(_ =>
            {
                _tagController.ShowElementsWithTag(this);
            });
        }

        private static void SelectTag(SceneLoaderTag tag)
        {
            _currentSelection?.ToggleBehaviourButtons(DisplayStyle.None);

            if (_currentSelection == tag) return;
            _currentSelection = tag;
            _currentSelection?.ToggleBehaviourButtons(DisplayStyle.Flex);
        }

        private void ToggleBehaviourButtons(DisplayStyle displayStyle)
        {
            _removeButton.style.display = displayStyle;
            _searchButton.style.display = displayStyle;
        }
    }
}
