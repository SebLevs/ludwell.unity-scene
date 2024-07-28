using Ludwell.Architecture;
using Ludwell.Theme.Editor;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ludwell.Scene.Editor
{
    public class SceneDataManagerWindow : EditorWindow
    {
        [SerializeField] private VisualTreeAsset _visualTreeAsset;

        [SerializeField] private StyleSheet _darkTheme;
        [SerializeField] private StyleSheet _lightTheme;

        private TagsManagerController _tagsManagerController;
        private SceneDataController _sceneDataController;

        private ThemeManagerEditor _themeManagerEditor;

        [MenuItem("Tools/Scene Data Manager")]
        public static void OpenWindow()
        {
            GetWindow<SceneDataManagerWindow>(title: "Scene Data Manager");
        }

        public void CreateGUI()
        {
            _visualTreeAsset.CloneTree(rootVisualElement);

            _themeManagerEditor = new ThemeManagerEditor(rootVisualElement, _darkTheme, _lightTheme);

            _tagsManagerController = new TagsManagerController(rootVisualElement);
            _sceneDataController = new SceneDataController(rootVisualElement);

            rootVisualElement.Q<ViewManager>().TransitionToFirstViewOfType<SceneDataController>();
        }

        [Shortcut("SceneManagerToolkit", KeyCode.S,
            ShortcutModifiers.Control | ShortcutModifiers.Shift | ShortcutModifiers.Alt)]
        private static void ShortcutToggleWindow()
        {
            if (HasOpenInstances<SceneDataManagerWindow>())
            {
                GetWindow<SceneDataManagerWindow>().Close();
            }
            else
            {
                OpenWindow();
            }
        }

        private void OnDestroy()
        {
            _themeManagerEditor.Dispose();
            _themeManagerEditor = null;
            _tagsManagerController = null;
            _sceneDataController = null;
            
            rootVisualElement.Q<ViewManager>().Reset();
            Signals.Clear<UISignals.RefreshView>();
        }
    }
}