using System;
using System.Linq;
using Ludwell.UIToolkitUtilities;
using UnityEngine.UIElements;

namespace Ludwell.MoreInformation.Editor
{
    public class MoreInformationView : IDisposable
    {
        private const string ModalOverlayName = "modal-overlay";
        private const string MoreInformationPanelName = "panel__more-information";

        private const string AboutCompanyButtonName = "about-company";
        private const string DocumentationButtonName = "documentation";
        private const string DiscordServerButtonName = "discord-server";
        private const string BrowseProductsButtonName = "browse-products";
        private const string RateProductButtonName = "rate-product";

        private const string ScaleAnimationClassName = "scale-animation";
        private const string ScaleAnimationDownClassName = "scale-animation__down";

        private readonly VisualElement _root;

        private readonly VisualElement _modalOverlay;
        private readonly VisualElement _moreInformationPanel;

        public VisualElement AboutCompanyButton { get; }
        public VisualElement DocumentationButton { get; }
        public VisualElement DiscordServerButton { get; }
        public VisualElement BrowseProductsButton { get; }
        public VisualElement RateProductButton { get; }

        private readonly VisualElement[] _stars = new VisualElement[5];

        private float _baseStarScale = -1;

        private void Hide(ClickEvent _) => Hide();

        public MoreInformationView(VisualElement root)
        {
            _root = root.Q<VisualElement>(nameof(MoreInformationView));

            _modalOverlay = _root.Q<VisualElement>(ModalOverlayName);
            _modalOverlay.RegisterCallback<ClickEvent>(Hide);

            _moreInformationPanel = _root.Q<VisualElement>(MoreInformationPanelName);
            _moreInformationPanel.RegisterCallback<ClickEvent>(PreventPanelBubbleUp);

            AboutCompanyButton = _root.Q<VisualElement>(AboutCompanyButtonName);
            AboutCompanyButton.RegisterCallback<MouseDownEvent>(MouseDownEvent);
            AboutCompanyButton.RegisterCallback<MouseUpEvent>(MouseUpEvent);

            DocumentationButton = _root.Q<VisualElement>(DocumentationButtonName);
            DocumentationButton.RegisterCallback<MouseDownEvent>(MouseDownEvent);
            DocumentationButton.RegisterCallback<MouseUpEvent>(MouseUpEvent);

            DiscordServerButton = _root.Q<VisualElement>(DiscordServerButtonName);
            DiscordServerButton.RegisterCallback<MouseDownEvent>(MouseDownEvent);
            DiscordServerButton.RegisterCallback<MouseUpEvent>(MouseUpEvent);

            BrowseProductsButton = _root.Q<VisualElement>(BrowseProductsButtonName);
            BrowseProductsButton.RegisterCallback<MouseDownEvent>(MouseDownEvent);
            BrowseProductsButton.RegisterCallback<MouseUpEvent>(MouseUpEvent);

            RateProductButton = _root.Q<VisualElement>(RateProductButtonName);
            RateProductButton.RegisterCallback<MouseDownEvent>(MouseDownEvent);
            RateProductButton.RegisterCallback<MouseUpEvent>(MouseUpEvent);

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

            AboutCompanyButton.UnregisterCallback<MouseDownEvent>(MouseDownEvent);
            AboutCompanyButton.UnregisterCallback<MouseUpEvent>(MouseUpEvent);

            DocumentationButton.UnregisterCallback<MouseDownEvent>(MouseDownEvent);
            DocumentationButton.UnregisterCallback<MouseUpEvent>(MouseUpEvent);

            DiscordServerButton.UnregisterCallback<MouseDownEvent>(MouseDownEvent);
            DiscordServerButton.UnregisterCallback<MouseUpEvent>(MouseUpEvent);

            BrowseProductsButton.UnregisterCallback<MouseDownEvent>(MouseDownEvent);
            BrowseProductsButton.UnregisterCallback<MouseUpEvent>(MouseUpEvent);

            RateProductButton.UnregisterCallback<MouseDownEvent>(MouseDownEvent);
            RateProductButton.UnregisterCallback<MouseUpEvent>(MouseUpEvent);

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

        private void PreventPanelBubbleUp(ClickEvent evt)
        {
            evt.PreventDefault();
            evt.StopPropagation();
        }

        private void MouseDownEvent(MouseDownEvent evt)
        {
            var targetAsVisualElement = (evt.target as VisualElement).FirstAncestorWithClass(ScaleAnimationClassName);
            targetAsVisualElement.AddToClassList(ScaleAnimationDownClassName);
            targetAsVisualElement.RemoveFromClassList(ScaleAnimationClassName);
        }

        private void MouseUpEvent(MouseUpEvent evt)
        {
            var targetAsVisualElement =
                (evt.target as VisualElement).FirstAncestorWithClass(ScaleAnimationDownClassName);
            if (targetAsVisualElement == null) return;
            targetAsVisualElement.RemoveFromClassList(ScaleAnimationDownClassName);
            targetAsVisualElement.AddToClassList(ScaleAnimationClassName);
        }

        private void ScaleStarsUpTo(MouseEnterEvent evt)
        {
            if (_baseStarScale <= -1) _baseStarScale = _stars[0].Children().First().resolvedStyle.width;

            var target = evt.target as VisualElement;

            foreach (var star in _stars)
            {
                var icon = star.Children().First();
                icon.style.width = _baseStarScale * 2;
                icon.style.height = _baseStarScale * 2;
                if (star == target) break;
            }
        }

        private void ScaleDownAllStars(MouseLeaveEvent evt)
        {
            foreach (var star in _stars)
            {
                var icon = star.Children().First();
                icon.style.width = _baseStarScale;
                icon.style.height = _baseStarScale;
                if (star == evt.target) break;
            }
        }
    }
}