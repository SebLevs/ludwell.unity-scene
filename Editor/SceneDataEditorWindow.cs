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
        private LoaderViewController _loaderViewController;

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
            _loaderViewController = new LoaderViewController(rootVisualElement);
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