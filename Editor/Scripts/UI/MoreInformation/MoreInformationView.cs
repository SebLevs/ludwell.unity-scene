using UnityEngine.UIElements;

namespace Ludwell.Scene.Editor
{
    public class MoreInformationView
    {
        private readonly VisualElement _root;

        public MoreInformationView(VisualElement root)
        {
            _root = root.Q<VisualElement>(nameof(MoreInformationView));
        }
    }
}
