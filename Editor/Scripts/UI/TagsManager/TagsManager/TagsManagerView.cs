using System.IO;
using Ludwell.Scene.Editor;
using UnityEngine.UIElements;

namespace Ludwell.Scene
{
    public class TagsManagerView : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<TagsManagerView, UxmlTraits>
        {
        }

        private static readonly string
            UxmlPath = Path.Combine("Uxml", nameof(TagsManagerView), nameof(TagsManagerView));

        private static readonly string UssPath = Path.Combine("Uss", nameof(TagsManagerView), nameof(TagsManagerView));

        private const string ReferenceName = "reference-name";

        private Label _referenceName;

        private TagsManagerPresentor _presentor;

        public TagsManagerView()
        {
            this.AddHierarchyFromUxml(UxmlPath);
            this.AddStyleFromUss(UssPath);

            SetReferences();

            HandleTagController();
        }

        public void ShowDelegated(TagSubscriberWithTags tagSubscriber, VisualElement previousView)
        {
            _presentor.Show(tagSubscriber, previousView);
        }

        public void Show()
        {
            style.display = DisplayStyle.Flex;
        }

        public void SetReferenceText(string value)
        {
            _referenceName.text = value;
        }

        public void SetPreviousTargetedElementDelegated(TagsManagerElementView target)
        {
            _presentor.SetPreviousTargetedElement(target);
        }

        public void RemoveInvalidTagElementDelegated(TagWithSubscribers tag)
        {
            _presentor.RemoveInvalidTagElement(tag);
        }

        private void SetReferences()
        {
            _referenceName = this.Q<Label>(ReferenceName);
            _presentor = new TagsManagerPresentor(this);
        }

        private void HandleTagController()
        {
            _presentor.HandleTagController();
        }
    }
}