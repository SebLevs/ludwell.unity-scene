using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ludwell.Scene
{
    public class ButtonWithIcon : Button
    {
        public new class UxmlFactory : UxmlFactory<ButtonWithIcon>
        {
        }
        
        private const string IconName = "icon";
        private VisualElement _icon;

        public ButtonWithIcon()
        {
            InitializeIcon();
        }
        
        public void SetIcon(Sprite icon)
        {
            _icon.style.backgroundImage = new StyleBackground(icon);
        }
        
        private void InitializeIcon()
        {
            Add(new VisualElement());
            _icon = Children().First();
            _icon.name = IconName;
            _icon.pickingMode = PickingMode.Ignore;
            _icon.style.flexGrow = 1;
        }
    }
}
