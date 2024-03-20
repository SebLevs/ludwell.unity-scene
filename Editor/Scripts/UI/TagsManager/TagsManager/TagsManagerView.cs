using UnityEngine.UIElements;

namespace Ludwell.Scene.Editor
{
    public class TagsManagerView
    {
        private const string ReferenceName = "reference-name";

        private Label _referenceName;

        private VisualElement _root;

        public TagsManagerView(VisualElement root)
        {
            _root = root;
            SetReferences();
        }

        public void Show()
        {
            _root.style.display = DisplayStyle.Flex;
        }

        public void Hide()
        {
            _root.style.display = DisplayStyle.None;
        }

        public void SetReferenceText(string value)
        {
            _referenceName.text = value;
        }

        private void SetReferences()
        {
            _referenceName = _root.Q<Label>(ReferenceName);
        }
    }
}