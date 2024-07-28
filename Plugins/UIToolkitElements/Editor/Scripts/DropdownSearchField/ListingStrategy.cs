using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ludwell.UIToolkitElements.Editor
{
    public class ListingStrategy
    {
        public string Name { get; }
        public Texture2D Icon { get; }
        public bool IsSearchEmptyString { get; }

        private readonly Func<string, IList, List<IListable>> _strategy;

        public ListingStrategy(string name, Texture2D icon, Func<string, IList, List<IListable>> strategy,
            bool isSearchEmptyString = false)
        {
            Name = name;
            Icon = icon;
            IsSearchEmptyString = isSearchEmptyString;
            _strategy = strategy;
        }

        public List<IListable> Execute(string searchFieldValue, IList searchFrom)
        {
            return _strategy.Invoke(searchFieldValue, searchFrom);
        }
    }
}