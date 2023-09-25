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
            InitializeTabController();
        }

        private void InitializeTabController()
        {
            _tabController = new TabController();
            _tabController.Init(rootVisualElement);
        }
    }
}
