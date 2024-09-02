using System;
using System.Linq;
using Ludwell.UIToolkitUtilities;
using UnityEngine.UIElements;

namespace Ludwell.MoreInformation.Editor
{
    internal class MoreInformationView : IDisposable
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

        private VisualElement _mouseDownCache;

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

            DocumentationButton = _root.Q<VisualElement>(DocumentationButtonName);
            DocumentationButton.RegisterCallback<MouseDownEvent>(MouseDownEvent);

            DiscordServerButton = _root.Q<VisualElement>(DiscordServerButtonName);
            DiscordServerButton.RegisterCallback<MouseDownEvent>(MouseDownEvent);

            BrowseProductsButton = _root.Q<VisualElement>(BrowseProductsButtonName);
            BrowseProductsButton.RegisterCallback<MouseDownEvent>(MouseDownEvent);

            RateProductButton = _root.Q<VisualElement>(RateProductButtonName);
            RateProductButton.RegisterCallback<MouseDownEvent>(MouseDownEvent);

            _root.RegisterCallback<MouseUpEvent>(MouseUpEvent);

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
            DocumentationButton.UnregisterCallback<MouseDownEvent>(MouseDownEvent);
            DiscordServerButton.UnregisterCallback<MouseDownEvent>(MouseDownEvent);
            BrowseProductsButton.UnregisterCallback<MouseDownEvent>(MouseDownEvent);
            RateProductButton.UnregisterCallback<MouseDownEvent>(MouseDownEvent);

            _root.UnregisterCallback<MouseUpEvent>(MouseUpEvent);

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
            _mouseDownCache = (evt.target as VisualElement).FirstAncestorWithClass(ScaleAnimationClassName);
            _mouseDownCache.AddToClassList(ScaleAnimationDownClassName);
            _mouseDownCache.RemoveFromClassList(ScaleAnimationClassName);
        }

        private void MouseUpEvent(MouseUpEvent evt)
        {
            if (_mouseDownCache == null) return;
            _mouseDownCache.RemoveFromClassList(ScaleAnimationDownClassName);
            _mouseDownCache.AddToClassList(ScaleAnimationClassName);
            _mouseDownCache = null;
        }

        private void ScaleStarsUpTo(MouseEnterEvent evt)
        {
            if (_baseStarScale <= -1) _baseStarScale = _stars[0].Children().First().resolvedStyle.width;

            var target = evt.target as VisualElement;

            foreach (var star in _stars)
            {
                var icon = star.Children().First();
                icon.style.width = _baseStarScale * 1.5f;
                icon.style.height = _baseStarScale * 1.5f;
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
