using UnityEngine;
using UnityEngine.UIElements;

namespace Ludwell.Scene
{
    public class SceneLoaderTagController : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<SceneLoaderTagController, UxmlTraits> { }
        
        private const string UxmlPath = "Uxml/" + nameof(LoaderController) + "/" + nameof(SceneLoaderTagController);
        private const string UssPath = "Uss/" + nameof(LoaderController) + "/" + nameof(SceneLoaderTagController);

        private const string AddButtonName = "tags__button-add";
        private const string TagsContainerName = "tags-container";

        private LoaderListViewElement _loaderListViewElement;

        private Button _addButton;
        private VisualElement _tagContainer;

        public SceneLoaderTagController()
        {
            this.AddHierarchyFromUxml(UxmlPath);
            this.AddStyleFromUss(UssPath);

            SetReferences();
            SetButtonEvents();
        }

        public void Add(string value)
        {
            Debug.LogError(nameof(Add));
            
            var newTag = new SceneLoaderTag();
            newTag.SetTagName(value);
            _tagContainer.Add(newTag);
            _loaderListViewElement.Cache.Tags.Add(value);
        }
        
        public void Remove(SceneLoaderTag tag)
        {
            Debug.LogError(nameof(Remove));
            
            _loaderListViewElement.Cache.Tags.Remove(tag.GetTagName);
            _tagContainer.Remove(tag);
        }

        public void ShowElementsWithTag(SceneLoaderTag tag)
        {
            Debug.LogError(nameof(ShowElementsWithTag));
        }
        
        private void SetReferences()
        {
            _addButton = this.Q<Button>(AddButtonName);
            _tagContainer = this.Q<VisualElement>(TagsContainerName);
            
            RegisterCallback<AttachToPanelEvent>(_ =>
            {
                _loaderListViewElement = GetFirstAncestorOfType<LoaderListViewElement>();
            });
        }
        
        private void SetButtonEvents()
        {
            _addButton.RegisterCallback<ClickEvent>(_ =>
            {
                Add("Button TEST");
            });
        }
    }
}
