using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ludwell.Scene.Editor
{
    public class SceneDataManagerWindow : EditorWindow
    {
        [SerializeField] private VisualTreeAsset visualTreeAsset;

        private TagsManagerController _tagsManagerController;
        private SceneDataController _sceneDataController;

        [MenuItem("Tools/Scene Data Manager")]
        public static void OpenWindow()
        {
            GetWindow<SceneDataManagerWindow>(title: "Scene Data Manager");
        }

        public void CreateGUI()
        {
            visualTreeAsset.CloneTree(rootVisualElement);

            _tagsManagerController = new TagsManagerController(rootVisualElement);
            _sceneDataController = new SceneDataController(rootVisualElement);

            rootVisualElement.Q<ViewManager>().TransitionToFirstViewOfType<SceneDataController>();
        }

        private void OnDestroy()
        {
            rootVisualElement.Q<ViewManager>().Reset();
            Signals.Clear<UISignals.RefreshView>();
        }
    }
}