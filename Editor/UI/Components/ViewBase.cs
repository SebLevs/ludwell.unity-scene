using UnityEngine.UIElements;

namespace Ludwell.Scene
{
    public abstract class ViewBase
    {
        protected VisualElement VisualElement;
        
        public ViewBase(VisualElement visualElement)
        {
            VisualElement = visualElement;
        }

        public virtual void Reset() { }
    }
}
