using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ludwell.Scene
{
    public class TagsController : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<TagsController, UxmlTraits> { }
        
        private const string UxmlPath = "Uxml/" + nameof(LoaderController) + "/" + nameof(TagsController);
        private const string UssPath = "Uss/" + nameof(LoaderController) + "/" + nameof(TagsController);

        private const string AddButtonName = "tags__button-add";
        private const string TagsContainerName = "tags-container";

        private LoaderListViewElement _loaderListViewElement;

        private Button _addButton;
        private VisualElement _tagsContainer;

        public TagsController()
        {
            this.AddHierarchyFromUxml(UxmlPath);
            this.AddStyleFromUss(UssPath);

            SetReferences();
            SetButtonEvents();
        }

        public IEnumerable<VisualElement> GetChildren => _tagsContainer.Children();
        
        public void Add(string value)
        {
            Debug.LogError(nameof(Add));
            
            var newTag = new TagElement();
            newTag.SetTagName(value);
            _tagsContainer.Add(newTag);
        }
        
        public void Remove(TagElement tagElement)
        {
            Debug.LogError(nameof(Remove));
            
            _loaderListViewElement.Cache.Tags.Remove(tagElement.GetTagName);
            _tagsContainer.Remove(tagElement);
        }

        public void ShowElementsWithTag(TagElement tagElement)
        {
            Debug.LogError(nameof(ShowElementsWithTag));
        }
        
        private void SetReferences()
        {
            _addButton = this.Q<Button>(AddButtonName);
            _tagsContainer = this.Q<VisualElement>(TagsContainerName);
            
            RegisterCallback<AttachToPanelEvent>(_ =>
            {
                _loaderListViewElement = GetFirstAncestorOfType<LoaderListViewElement>();
            });
        }
        
        private void SetButtonEvents()
        {
            _addButton.RegisterCallback<ClickEvent>(_ =>
            {
                this.Root().Q<TagsManager>().Show(this);
            });
        }
    }
}
