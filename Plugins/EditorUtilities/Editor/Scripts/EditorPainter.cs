using UnityEditor;
using UnityEngine;

namespace Ludwell.EditorUtilities
{
    public static class EditorPainter
    {
        private static readonly Color _defaultColor = Color.grey;
        
        public static void DrawSeparatorLine(Color color = default, int thickness = 1, int verticalPadding = 8)
        {
            GUILayout.Space(verticalPadding);

            var rect = EditorGUILayout.GetControlRect(GUILayout.Height(thickness));
            EditorGUI.DrawRect(rect, color == default ? _defaultColor : color);

            GUILayout.Space(verticalPadding);
        }
    }
}
