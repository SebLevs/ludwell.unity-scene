using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ludwell.UIToolkitUtilities.Editor
{
    public class DefaultThemes
    {
        private static readonly string UssThemedClass = Path.Combine("UI", "Theme", "themed-classes");
        private static readonly string UssPathDark = Path.Combine("UI", "Theme", "ludwell_theme-dark");
        private static readonly string UssPathLight = Path.Combine("UI", "Theme", "ludwell_theme-light");

        public static StyleSheet GetThemedClass()
        {
            return Resources.Load<StyleSheet>(UssThemedClass);
        }
        
        public static StyleSheet GetDarkThemeStyleSheet()
        {
            return Resources.Load<StyleSheet>(UssPathDark);
        }
        
        public static StyleSheet GetLightThemeStyleSheet()
        {
            return Resources.Load<StyleSheet>(UssPathLight);
        }
    }
}