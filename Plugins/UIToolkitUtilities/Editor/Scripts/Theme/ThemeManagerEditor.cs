using System;
using UnityEditor;
using UnityEngine.UIElements;

namespace Ludwell.Theme.Editor
{
    public class ThemeManagerEditor : IDisposable
    {
        private VisualElement _root;
        private StyleSheet _darkTheme;
        private StyleSheet _lightTheme;

        public ThemeManagerEditor(VisualElement element, StyleSheet darkTheme, StyleSheet lightTheme)
        {
            _root = element;
            _darkTheme = darkTheme;
            _lightTheme = lightTheme;
            
            _root.styleSheets.Remove(_lightTheme);
            _root.styleSheets.Remove(_darkTheme);
            
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
