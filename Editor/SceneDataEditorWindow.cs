using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ludwell.Scene
{
    public class SceneDataEditorWindow : EditorWindow
    {
        [SerializeField] private VisualTreeAsset visualTreeAsset;
        [SerializeField] private SceneDataEditorSettings editorSettings;
        private TabController _tabController;
        private ViewLoaderController _viewLoaderController;

        [MenuItem("Tool/Scene Data Manager")]
        public static void OpenWindow()
        {
            GetWindow<SceneDataEditorWindow>(title: "Scene Data Manager");
        }
        
        public void CreateGUI()
        {
            visualTreeAsset.CloneTree(rootVisualElement);
            Init();
        }
        
        private void Init()
        {
            _tabController = new TabController(rootVisualElement);
            _viewLoaderController = rootVisualElement.Q<ViewLoaderController>();
        }
    }
}