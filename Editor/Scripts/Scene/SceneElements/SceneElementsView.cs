using UnityEngine.UIElements;

namespace Ludwell.Scene.Editor
{
    public class SceneElementsView
    {
        private const string CloseAllButtonName = "button__close-all";
        private const string AddButtonName = "add";
        private const string RemoveButtonName = "remove";
        private const string MoreInformationButtonName = "button__more-information";

        private readonly VisualElement _root;

        public Button CloseAllButton { get; }
        public Button AddButton { get; }
        public Button RemoveButton { get; }
        public Button MoreInformationButton { get; }

        public SceneElementsView(VisualElement root)
        {
            _root = root;

            CloseAllButton = _root.Q<Button>(CloseAllButtonName);
            AddButton = _root.Q<Button>(AddButtonName);
            RemoveButton = _root.Q<Button>(RemoveButtonName);
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