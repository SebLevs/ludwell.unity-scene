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
    }
}