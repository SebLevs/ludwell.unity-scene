using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ludwell.Scene
{
    public class SceneDataManagerWindow : EditorWindow
    {
        [SerializeField] private VisualTreeAsset visualTreeAsset;

        [MenuItem("Tools/Scene Data Manager")]
        public static void OpenWindow()
        {
            GetWindow<SceneDataManagerWindow>(title: "Scene Data Manager");
        }

        public void CreateGUI()
        {
            visualTreeAsset.CloneTree(rootVisualElement);
        }
    }
}