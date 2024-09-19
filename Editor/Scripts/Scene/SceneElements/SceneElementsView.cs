using UnityEngine.UIElements;

namespace Ludwell.SceneManagerToolkit.Editor
{
    internal class SceneElementsView
    {
        private const string CloseAllButtonName = "button__close-all";
        private const string MoreInformationButtonName = "button__more-information";

        private readonly VisualElement _root;

        public Button CloseAllButton { get; }
        public Button MoreInformationButton { get; }

        public SceneElementsView(VisualElement root)
        {
            _root = root;

            CloseAllButton = _root.Q<Button>(CloseAllButtonName);
            MoreInformationButton = _root.Q<Button>(MoreInformationButtonName);
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
