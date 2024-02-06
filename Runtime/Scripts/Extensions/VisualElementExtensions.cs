using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ludwell.Scene
{
    public static class VisualElementExtensions
    {
        public static VisualElement AddHierarchyFromUxml(this VisualElement element, string uxmlPath)
        {
            var visualTreeAsset = Resources.Load<VisualTreeAsset>(uxmlPath);
            var clone = visualTreeAsset.CloneTree();
            var rootElement = clone.ElementAt(0);
            element.hierarchy.Add(rootElement);
            return rootElement;
        }

        public static void AddStyleFromUss(this VisualElement element, string ussPath)
        {
            var styleSheet = Resources.Load<StyleSheet>(ussPath);
            element.styleSheets.Add(styleSheet);
        }

        public static VisualElement Root(this VisualElement element)
        {
            if (element.parent == null) return element;
            return element.parent.Root();
        }

        public static VisualElement FindFirstParentWithName(this VisualElement element, string name)
        {
            if (element.parent == null)
            {
                throw new MissingReferenceException($"No parent containing the name \"{name}\" was found");
            }

            if (element.parent.name == name) return element.parent;
            return element.parent.FindFirstParentWithName(name);
        }

        public static VisualElement FindFirstChildWhereNameContains(this VisualElement element, string name)
        {
            foreach (VisualElement child in element.Children())
            {
                if (child.name.Contains(name))
                {
                    return child;
                }
            }

            throw new MissingReferenceException($"No child containing the name \"{name}\" was found");
        }

        /// <summary>
        /// Bubble up recursive algorithm.<br/>
        /// Search children in parent of current element for a type T.<br/>
        /// Memoization is used to prevent high complexity. Prevent searching already searched elements.
        /// </summary>
        /// <param name="element">Current element</param>
        /// <param name="memoization">Will be initialized automatically if null</param>
        public static T FindInAncestors<T>(this VisualElement element, List<VisualElement> memoization = null)
            where T : VisualElement
        {
            if (element.parent == null)
            {
                throw new MissingReferenceException($"No element of type \"{typeof(T)}\" was found in parents");
            }

            memoization ??= new List<VisualElement>();

            memoization.Add(element);
            foreach (var child in element.parent.Children())
            {
                if (memoization.Contains(child)) continue;
                Debug.LogError(child.name);
                var query = child.Q<T>();
                if (query != null) return query;
            }

            return element.parent.FindInAncestors<T>(memoization);
        }
    }
}