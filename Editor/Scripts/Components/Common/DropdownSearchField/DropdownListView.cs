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

        public DropdownListView()
        {
            // this.AddStyleFromUss(UssPath);
            style.flexGrow = 0.4f;
            
            this.Q<ListView>();
            this.AddStyleFromUss(UssPath);
            RemoveFromClassList("unity-list-view");
            RemoveFromClassList("unity-collection-view");
            AddToClassList("dropdown-list-view");
            
            // style.position = Position.Absolute;
            // style.width = new(40, LengthUnit.Percent);
            // style.flexGrow = 1f;
            // style.maxHeight = 200;
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
        
        public void PlaceUnder(VisualElement sibling)
        {
            float rootTotalHeight = 0.0f;

            var rootHeight = sibling.Root().resolvedStyle.height;
            var rootVisualElementHeight = sibling.Root()
                .FindFirstChildWhereNameContains(UiToolkitNames.RootVisualContainer).resolvedStyle.height;
            var difference = rootHeight - rootVisualElementHeight;
            
            // WORKING SOLUTION
            var absolutePosition = sibling.LocalToWorld(Vector2.zero);
            style.left = absolutePosition.x;
            style.top = absolutePosition.y + sibling.resolvedStyle.height - difference;
        }
    }
}