using System;
using UnityEngine.UIElements;

namespace Ludwell.Scene.Editor
{
    public class MoreInformationView : IDisposable
    {
        private const string ModalOverlayName = "modal-overlay";
        private const string MoreInformationPanelName = "panel__more-information";

        private readonly VisualElement _root;

        private readonly VisualElement _modalOverlay;
        private readonly VisualElement _moreInformationPanel;

        private readonly VisualElement[] _stars = new VisualElement[5];

        private void Hide(ClickEvent _) => Hide();

        public MoreInformationView(VisualElement root)
        {
            _root = root.Q<VisualElement>(nameof(MoreInformationView));

            _modalOverlay = _root.Q<VisualElement>(ModalOverlayName);
            _modalOverlay.RegisterCallback<ClickEvent>(Hide);

            _moreInformationPanel = _root.Q<VisualElement>(MoreInformationPanelName);
            _moreInformationPanel.RegisterCallback<ClickEvent>(PreventPanelBubbleUp);

            for (var index = 1; index < _stars.Length + 1; index++)
            {
                _stars[index - 1] = _root.Q<VisualElement>(index.ToString());
                _stars[index - 1].RegisterCallback<MouseEnterEvent>(ScaleStarsUpTo);
                _stars[index - 1].RegisterCallback<MouseLeaveEvent>(ScaleDownAllStars);
            }
        }

        public void Dispose()
        {
            _modalOverlay.UnregisterCallback<ClickEvent>(Hide);

            _moreInformationPanel.UnregisterCallback<ClickEvent>(PreventPanelBubbleUp);

            for (var index = 1; index < _stars.Length + 1; index++)
            {
                _stars[index - 1].UnregisterCallback<MouseEnterEvent>(ScaleStarsUpTo);
                _stars[index - 1].UnregisterCallback<MouseLeaveEvent>(ScaleDownAllStars);
            }
        }

        public void Show()
        {
            _root.style.display = DisplayStyle.Flex;
        }

        public void Hide()
        {
            _root.style.display = DisplayStyle.None;
        }

        private void PreventPanelBubbleUp(ClickEvent _)
        {
            _.PreventDefault();
            _.StopPropagation();
        }

        private void ScaleStarsUpTo(MouseEnterEvent evt)
        {
            ScaleDownAllStars();

            var target = evt.target as Button;
            foreach (var star in _stars)
            {
                star.style.width = new StyleLength(52);
                star.style.height = new StyleLength(52);
                if (star == target) break;
            }
        }

        private void ScaleDownAllStars(MouseLeaveEvent evt = null)
        {
            foreach (var star in _stars)
            {
                star.style.width = new StyleLength(40);
                star.style.height = new StyleLength(40);
            }
        }
    }
}