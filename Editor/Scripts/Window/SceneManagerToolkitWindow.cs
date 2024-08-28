using Ludwell.Architecture;
using Ludwell.UIToolkitUtilities.Editor;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ludwell.SceneManagerToolkit.Editor
{
    internal class SceneManagerToolkitWindow : EditorWindow
    {
        [SerializeField] private VisualTreeAsset _visualTreeAsset;

        [SerializeField] private StyleSheet _darkTheme;
        [SerializeField] private StyleSheet _lightTheme;

        private const string WindowTitle = "Scene Manager Toolkit";
        
        private ThemeManagerUIToolkitEditor _themeManagerUIToolkitEditor;
        
        private Disposer _disposer;

        public SceneElementsController SceneElementsController { get; private set; }
        private TagsManagerController TagsManagerController { get; set; }


        [MenuItem("Tools/Ludwell Studio/Scene Manager Toolkit")]
        public static void OpenWindow()
        {
            GetWindow<SceneManagerToolkitWindow>(title: WindowTitle);
        }

        public void CreateGUI()
        {
            titleContent = new GUIContent(WindowTitle);

            _visualTreeAsset.CloneTree(rootVisualElement);

            _themeManagerUIToolkitEditor = new ThemeManagerUIToolkitEditor(rootVisualElement, _darkTheme, _lightTheme);

            TagsManagerController = new TagsManagerController(rootVisualElement);

            SceneElementsController = new SceneElementsController(rootVisualElement);

            rootVisualElement.Q<ViewManager>().TransitionToFirstViewOfType<SceneElementsController>();

            _disposer = new();
            Services.Add<Disposer>(_disposer);
        }

        [Shortcut("SceneManagerToolkit", KeyCode.S,
            ShortcutModifiers.Control | ShortcutModifiers.Shift | ShortcutModifiers.Alt)]
        private static void ShortcutToggleWindow()
        {
            if (HasOpenInstances<SceneManagerToolkitWindow>())
            {
                GetWindow<SceneManagerToolkitWindow>().Close();
            }
            else
            {
                OpenWindow();
            }
        }

        private void OnDestroy()
        {
            rootVisualElement.Q<ViewManager>().Reset();

            Dispose();

            _disposer.Clear();
            _disposer = null;
            _themeManagerUIToolkitEditor = null;
            TagsManagerController = null;
            SceneElementsController = null;

            Signals.Clear<UISignals.RefreshView>();
        }

        private void Dispose()
        {
            _themeManagerUIToolkitEditor?.Dispose();
            TagsManagerController?.Dispose();
            SceneElementsController?.Dispose();
            _disposer.Dispose();
        }
    }
}
