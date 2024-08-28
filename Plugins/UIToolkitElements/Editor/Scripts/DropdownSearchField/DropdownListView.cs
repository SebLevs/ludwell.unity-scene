using System;
using System.Collections.Generic;
using System.IO;
using Ludwell.UIToolkitUtilities;
using Ludwell.UIToolkitUtilities.Editor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ludwell.UIToolkitElements.Editor
{
    public class DropdownListView : ListView
    {
        public new class UxmlFactory : UxmlFactory<DropdownListView, UxmlTraits>
        {
        }

        private static readonly string UssPath =
            Path.Combine("UI", nameof(DropdownSearchField), "Uss_" + nameof(DropdownListView));

        private ListViewHandler<DropdownVisualElement, DropdownData> _listViewHandler;
        private readonly List<DropdownData> _data = new();

        private float _heightDifference = -1;

        private const float _maxHeight = 200;

        private DropdownSearchField _owner;

        public int Count => itemsSource.Count;

        public bool IsHidden => style.display == DisplayStyle.None;

        public DropdownListView()
        {
            this.AddStyleFromUss(UssPath);
            AddToClassList("dropdown-list-view");
            SetStyle();

            _listViewHandler = new ListViewHandler<DropdownVisualElement, DropdownData>(this, _data);
        }

        public void SetOwner(DropdownSearchField dropdownSearchField)
        {
            _owner = dropdownSearchField;
        }

        public void Show()
        {
            style.display = DisplayStyle.Flex;
        }

        public void Hide()
        {
            style.display = DisplayStyle.None;
        }

        public void Add(string actionName, Action action)
        {
            _data.Add(new DropdownData()
            {
                Name = actionName,
                Action = action
            });
            Rebuild();
        }

        public void ClearData()
        {
            _data.Clear();
            Rebuild();
        }

        public List<DropdownVisualElement> GetElements()
        {
            return this.Query<DropdownVisualElement>().ToList();
        }

        public void PlaceUnder(VisualElement target)
        {
            TryCacheHeightDifference(target);

            var absolutePosition = target.LocalToWorld(Vector2.zero);
            style.left = absolutePosition.x;
            style.top = absolutePosition.y + target.resolvedStyle.height - _heightDifference;
        }

        private void TryCacheHeightDifference(VisualElement target)
        {
            if (_heightDifference > -1) return;
            var rootHeight = target.Root().resolvedStyle.height;
            var rootVisualElement = target.Root().FindFirstChildWhereNameContains(UiToolkitNames.RootVisualContainer);
            var rootVisualElementHeight = rootVisualElement.resolvedStyle.height;
            _heightDifference = rootHeight - rootVisualElementHeight;
        }

        private void SetStyle()
        {
            style.maxHeight = _maxHeight;
            style.position = Position.Absolute;
            this.Q<Scroller>().style.backgroundColor =
                new StyleColor(new Color(0.2627451f, 0.2627451f, 0.2627451f, 1f));

            RegisterCallback<GeometryChangedEvent>(_ => { style.width = _owner.resolvedStyle.width; });
        }
    }
}
