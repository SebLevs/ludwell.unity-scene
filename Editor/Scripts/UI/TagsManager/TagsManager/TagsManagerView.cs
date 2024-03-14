using System;
using Ludwell.Scene.Editor;
using UnityEngine.UIElements;

namespace Ludwell.Scene
{
    public class TagsManagerView: IViewable
    {
        private const string ReferenceName = "reference-name";

        private Label _referenceName;

        private VisualElement _root;

        private Action OnShow;

        public TagsManagerView(VisualElement root, Action onShow)
        {
            _root = root;
            OnShow = onShow;
            SetReferences();
            
            ViewManager.Instance.Add(this);
        }

        ~TagsManagerView()
        {
            ViewManager.Instance.Remove(this);
        }

        public void Show()
        {
            _root.style.display = DisplayStyle.Flex;
            OnShow?.Invoke();
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