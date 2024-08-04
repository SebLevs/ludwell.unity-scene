using UnityEditor;
using UnityEngine.UIElements;

namespace Ludwell.UIToolkitUtilities
{
    public static class EditorVisualElementExtensions
    {
        public static void FocusOnNextEditorFrame(this TextField element)
        {
            EditorApplication.CallbackFunction updateDelegate = null;

            updateDelegate = () =>
            {
                EditorApplication.delayCall -= updateDelegate;
                element.Focus();
                var textLength = element.text.Length;
                element.SelectRange(textLength, textLength);
            };

            EditorApplication.delayCall += updateDelegate;
        }
    }
}