using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ludwell.Scene
{
    public class DropdownListView : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<DropdownListView, UxmlTraits> { }

        private const string UxmlPath = "Uxml/" + nameof(DropdownSearchField) + "/" + nameof(DropdownListView);
        private const string UssPath = "Uss/" + nameof(DropdownSearchField) + "/" + nameof(DropdownListView);

        private ListViewInitializer<DropdownElement, DropdownData> _listViewInitializer;
        private readonly List<DropdownData> _data = new();

        private readonly ListView _listView;

        public DropdownListView()
        {
            this.SetHierarchyFromUxml(UxmlPath);
            this.AddStyleFromUss(UssPath);

            _listView = this.Q<ListView>();
            _listViewInitializer = new(_listView, _data);
        }

        public int Count => _listView.itemsSource.Count;
        
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
            _listView.Rebuild();
        }

        public void ClearData()
        {
            _data.Clear();
            _listView.Rebuild();
        }

        public List<DropdownElement> GetElements()
        {
            return this.Query<DropdownElement>().ToList();
        }
        
        public void PlaceUnder(VisualElement sibling)
        {
            var absolutePosition = sibling.LocalToWorld(Vector2.zero);
            style.left = absolutePosition.x;
            style.top = absolutePosition.y + sibling.resolvedStyle.height;
        }
    }
}