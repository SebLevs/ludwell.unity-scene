using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ludwell.Scene
{
    public class DropdownListView : ListView
    {
        public new class UxmlFactory : UxmlFactory<DropdownListView, UxmlTraits> { }

        private const string UssPath = "Uss/" + nameof(DropdownSearchField) + "/" + nameof(DropdownListView);

        private ListViewInitializer<DropdownElement, DropdownData> _listViewInitializer;
        private readonly List<DropdownData> _data = new();
        
        private float _heightDifference = -1;

        public DropdownListView()
        {
            this.AddStyleFromUss(UssPath);
            AddToClassList("dropdown-list-view");
            SetStyle();

            _listViewInitializer = new(this, _data);
        }

        public int Count => itemsSource.Count;
        
        public bool IsHidden => style.display == DisplayStyle.None;

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

        public List<DropdownElement> GetElements()
        {
            return this.Query<DropdownElement>().ToList();
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
            if (_heightDifference <= -1)
            {
                var rootHeight = target.Root().resolvedStyle.height;
                var rootVisualElement = target.Root()
                    .FindFirstChildWhereNameContains(UiToolkitNames.RootVisualContainer);
                var rootVisualElementHeight = rootVisualElement.resolvedStyle.height;
                _heightDifference = rootHeight - rootVisualElementHeight;
            }
        }

        private void SetStyle()
        {
            style.width = Length.Percent(40);
            style.maxHeight = 200;
            style.position = Position.Absolute;
            this.Q<Scroller>().style.backgroundColor = new StyleColor(new Color(0.2627451f, 0.2627451f, 0.2627451f, 1f));
        }
    }
}