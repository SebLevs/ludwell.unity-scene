using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ludwell.Scene
{
    public class SceneDataEditorWindow : EditorWindow, IDisposable
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
            InitializeLoaderViewController();
        }

        private void InitializeTabController()
        {
            _tabController = new TabController();
            _tabController.Init(rootVisualElement);
        }

        private void InitializeLoaderViewController()
        {
            var loadView = new LoaderViewController();
            loadView.Init(rootVisualElement);
        }
        
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        private void OnDestroy()
        {
            Dispose();
        }
    }
}

// private List<FoldoutHeader> foldouts = new();
// var listView = rootVisualElement.Q<ListView>("scenes__list");
// listView.makeItem = onMakeItem;
// listView.bindItem = OnBindItem;
// listView.itemsSource = foldouts;