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

        private void SetReferences()
        {
            _referenceName = this.Q<Label>(ReferenceName);
            _presentor = new TagsManagerPresentor(this);
        }
    }
}