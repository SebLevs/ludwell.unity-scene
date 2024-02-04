using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ludwell.Scene.Editor
{
    public class ListingStrategy
    {
        public Texture2D Icon { get; }
        private readonly Func<string, IList, List<ISearchFieldListable>> _strategy;

        public ListingStrategy(Texture2D icon, Func<string, IList, List<ISearchFieldListable>> strategy)
        {
            Icon = icon;
            _strategy = strategy;
        }

        public List<ISearchFieldListable> Execute(string searchFieldValue, IList searchFrom)
        {
            return _strategy.Invoke(searchFieldValue, searchFrom);
        }
    }
}
