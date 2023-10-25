using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Ludwell.Scene
{
    public class Dropdown : ListView
    {
        public new class UxmlFactory : UxmlFactory<Dropdown, UxmlTraits> { }

        public new class UxmlTraits : VisualElement.UxmlTraits { }

        private const string DropdownListViewName = "dropdown-list-view";
        private const string UssPath = "Uss/" + DropdownListViewName;

        private ListViewInitializer<DropdownElement, DropdownData> _listViewInitializer;
        public List<DropdownData> _data = new();

        public Dropdown()
        {
            virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;
            this.AddStyleFromUss(UssPath);
            AddToClassList(DropdownListViewName);

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
    }
}