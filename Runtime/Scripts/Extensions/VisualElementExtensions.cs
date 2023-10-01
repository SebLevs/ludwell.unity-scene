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
    }
}
