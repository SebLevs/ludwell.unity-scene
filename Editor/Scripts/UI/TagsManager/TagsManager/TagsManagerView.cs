using UnityEngine.UIElements;

namespace Ludwell.Scene.Editor
{
    public class TagsManagerView
    {
        private const string ReferenceName = "reference-name";

        private VisualElement _root;
        
        private Label _referenceName;

        public TagsManagerView(VisualElement root)
        {
            _root = root;
            _referenceName = _root.Q<Label>(ReferenceName);
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
    }
}