using UnityEngine;
using UnityEngine.UIElements;

namespace Ludwell.Scene
{
    public class TagsController : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<TagsController, UxmlTraits>
        {
        }

        private const string UxmlPath = "Uxml/" + nameof(LoaderController) + "/" + nameof(TagsController);
        private const string UssPath = "Uss/" + nameof(LoaderController) + "/" + nameof(TagsController);

        private const string AddButtonName = "tags__button-add";
        private const string TagsContainerName = "tags-container";

        private LoaderListViewElement _loaderListViewElement;

        private Button _manageTagsButton;
        private VisualElement _tagsContainer;

        public TagsController()
        {
            this.AddHierarchyFromUxml(UxmlPath);
            this.AddStyleFromUss(UssPath);

            SetReferences();
            SetButtonEvents();
        }

        public void Remove(TagElement tagElement)
        {
            _loaderListViewElement.Cache.Tags.Remove(tagElement.Value);
            _tagsContainer.Remove(tagElement);
        }

        public void ShowElementsWithTag(TagElement tagElement)
        {
            Debug.LogError(nameof(ShowElementsWithTag));
            // todo: change what list the listview reference 
        }

        public void Refresh()
        {
            _tagsContainer.Clear();
            foreach (var tag in _loaderListViewElement.Cache.Tags)
            {
                _tagsContainer.Add(CreateTagElement(tag));
            }
        }

        private void SetReferences()
        {
            _manageTagsButton = this.Q<Button>(AddButtonName);
            _tagsContainer = this.Q<VisualElement>(TagsContainerName);

            RegisterCallback<AttachToPanelEvent>(_ =>
            {
                _loaderListViewElement = GetFirstAncestorOfType<LoaderListViewElement>();
            });
        }

        private void SetButtonEvents()
        {
            _manageTagsButton.RegisterCallback<ClickEvent>(_ =>
            {
                this.Root().Q<TagsManager>().Show(_loaderListViewElement.Cache.Tags);
            });
        }

        private TagElement CreateTagElement(string value)
        {
            TagElement tagElement = new();
            tagElement.SetTagName(value);
            return tagElement;
        }
    }
}