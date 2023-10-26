using UnityEngine;
using UnityEngine.UIElements;

namespace Ludwell.Scene
{
    public static class VisualElementExtensions
    {
        public static void SetHierarchyFromUxml(this VisualElement element, string uxmlPath)
        {
            var visualTreeAsset = Resources.Load<VisualTreeAsset>(uxmlPath);
            var clone = visualTreeAsset.CloneTree();
            var rootElement = clone.ElementAt(0);
            element.hierarchy.Add(rootElement);
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
        
        public static VisualElement GetParentByName(this VisualElement element, string name)
        {
            if (element.parent == null) return null;
            if (element.parent.name == name) return element.parent;
            return element.parent.Root();
        }
    }
}
