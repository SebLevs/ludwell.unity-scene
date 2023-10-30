using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ludwell.Scene
{
    public class SceneDataEditorWindow : EditorWindow
    {
        [SerializeField] private VisualTreeAsset visualTreeAsset;
        [SerializeField] private SceneDataEditorSettings editorSettings;

        [MenuItem("Tool/Scene Data Manager")]
        public static void OpenWindow()
        {
            GetWindow<SceneDataEditorWindow>(title: "Scene Data Manager");
        }

        public void CreateGUI()
        {
            visualTreeAsset.CloneTree(rootVisualElement);
        }
    }
}