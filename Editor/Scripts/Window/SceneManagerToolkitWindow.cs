using Ludwell.Architecture;
using Ludwell.UIToolkitUtilities.Editor;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ludwell.Scene.Editor
{
    public class SceneManagerToolkitWindow : EditorWindow
    {
        [SerializeField] private VisualTreeAsset _visualTreeAsset;

        [SerializeField] private StyleSheet _darkTheme;
        [SerializeField] private StyleSheet _lightTheme;

        public TagsManagerController TagsManagerController { get; private set; }
        public SceneElementsController SceneElementsController { get; private set; }

        private ThemeManagerEditor _themeManagerEditor;

        private Disposer _disposer;

        [MenuItem("Tools/Ludwell Studio/Scene Manager Toolkit")]
        public static void OpenWindow()
        {
            GetWindow<SceneManagerToolkitWindow>(title: "Scene Manager Toolkit");
        }

        public void CreateGUI()
        {
            _visualTreeAsset.CloneTree(rootVisualElement);

            _themeManagerEditor = new ThemeManagerEditor(rootVisualElement, _darkTheme, _lightTheme);

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
            _themeManagerEditor = null;
            TagsManagerController = null;
            SceneElementsController = null;

            Signals.Clear<UISignals.RefreshView>();
        }

        private void Dispose()
        {
            _themeManagerEditor?.Dispose();
            TagsManagerController?.Dispose();
            SceneElementsController?.Dispose();
            _disposer.Dispose();
        }
    }
}