using UnityEngine.UIElements;

namespace Ludwell.Scene.Editor
{
    public class QuickLoadView
    {
        private const string CloseAllButtonName = "button__close-all";
        private const string AddButtonName = "add";
        private const string RemoveButtonName = "remove";

        private readonly VisualElement _root;

        public Button CloseAllButton { get; }
        public Button AddButton { get; }
        public Button RemoveButton { get; }

        public QuickLoadView(VisualElement root)
        {
            _root = root;

            CloseAllButton = _root.Q<Button>(CloseAllButtonName);
            AddButton = _root.Q<Button>(AddButtonName);
            RemoveButton = _root.Q<Button>(RemoveButtonName);
        }
    }
}