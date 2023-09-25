using UnityEngine;
using UnityEngine.UIElements;

namespace Ludwell.Scene
{
    public class SplitView: TwoPaneSplitView  
    {
        public new class UxmlFactory : UxmlFactory<SplitView, UxmlTraits> { }

        private const string draglineAnchorName = "unity-dragline-anchor";
        
        private VisualElement leftPane;
        private VisualElement rightPane;
        public SplitView()
        {
            CustomizeDragLineAnchor();
        }

        private void CustomizeDragLineAnchor()
        {
            var dragLineAnchor = this.Q<VisualElement>(draglineAnchorName);
            //dragLineAnchor.style.backgroundColor = new StyleColor(new Color(0.1372549f, 0.1372549f, 0.1372549f, 1));
            //dragLineAnchor.style.width = 0.75f;
        }
    }
}
