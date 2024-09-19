using UnityEngine.UIElements;

namespace Ludwell.SceneManagerToolkit.Editor
{
    internal class ListFooterView
    {
        private const string RootName = "buttons-row";
        private const string AddButtonName = "add";
        private const string RemoveButtonName = "remove";
        
        private readonly VisualElement _root;
        
        public Button AddButton { get; }
        public Button RemoveButton { get; }

        public ListFooterView(VisualElement root)
        {
            _root = root.Q<VisualElement>(RootName);
            
            AddButton = _root.Q<Button>(AddButtonName);
            RemoveButton = _root.Q<Button>(RemoveButtonName);
        }
    }
}
