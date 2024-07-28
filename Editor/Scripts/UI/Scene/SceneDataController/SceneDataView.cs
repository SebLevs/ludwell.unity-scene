using UnityEngine.UIElements;

namespace Ludwell.Scene.Editor
{
    public class SceneDataView
    {
        private VisualElement _root;

        public SceneDataView(VisualElement parent)
        {
            _root = parent.Q(nameof(SceneDataView));
        }

        public void Show()
        {
            _root.style.display = DisplayStyle.Flex;
        }

        public void Hide()
        {
            _root.style.display = DisplayStyle.None;
        }
    }
}