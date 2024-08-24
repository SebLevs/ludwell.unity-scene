using System;
using UnityEditor;
using UnityEngine.UIElements;

namespace Ludwell.UIToolkitUtilities.Editor
{
    public class ThemeManagerUIToolkitEditor : IDisposable
    {
        private VisualElement _root;
        private StyleSheet _darkTheme;
        private StyleSheet _lightTheme;

        public ThemeManagerUIToolkitEditor(VisualElement element, StyleSheet darkTheme, StyleSheet lightTheme)
        {
            _root = element;
            _root.styleSheets.Add(DefaultThemes.GetThemedClass());

            _darkTheme = darkTheme;
            _lightTheme = lightTheme;

            _root.styleSheets.Remove(_darkTheme);
            _root.styleSheets.Remove(_lightTheme);

            SolveTheme();
            EditorApplication.delayCall += SolveTheme;
        }

        public void Dispose()
        {
            EditorApplication.delayCall -= SolveTheme;
        }

        private void SolveTheme()
        {
            if (EditorGUIUtility.isProSkin)
            {
                _root.styleSheets.Remove(_lightTheme);
                _root.styleSheets.Add(_darkTheme);
            }
            else
            {
                _root.styleSheets.Remove(_darkTheme);
                _root.styleSheets.Add(_lightTheme);
            }
        }
    }
}