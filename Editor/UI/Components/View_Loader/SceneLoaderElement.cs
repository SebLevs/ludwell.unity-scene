using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ludwell.Scene
{
    public class SceneLoaderElement : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<SceneLoaderElement, UxmlTraits> { }
        public new class UxmlTraits : VisualElement.UxmlTraits { }

        public SceneLoaderElement()
        {
            this.SetHierarchyFromUxml(UxmlPath);
            this.AddStyleFromUss(UssPath);
            SetReferences();
            InitRequiredScenesListView();
        }

        private const string UxmlPath = "Uxml/scene-loader-element";
        private const string UssPath = "Uss/scene-loader-element";
            
        private const string FoldoutName = "root__foldout";
        private const string RequiredScenesListViewName = "required-scenes";
        
        public Foldout FoldoutElement { get; private set; }

        public List<SceneLoaderElement> ItemsSource { get; private set; } = new();
        private ListView _listView;
        
        private void SetReferences()
        {
            FoldoutElement = this.Q<Foldout>(FoldoutName);
        }

        private void InitRequiredScenesListView()
        {
            _listView = this.Q<ListView>(RequiredScenesListViewName);
            _listView.itemsSource = ItemsSource;
            _listView.makeItem = OnMakeItem;
            _listView.bindItem = OnBindItem;
            _listView.unbindItem = OnUnbindItem;
        }
        
        private RequiredSceneElement OnMakeItem()
        {
            Debug.LogError(nameof(OnMakeItem));
            var element = new RequiredSceneElement();
            return element;
        }

        private void OnBindItem(VisualElement element, int index)
        {
            Debug.LogError(nameof(OnBindItem));
        }
        
        private void OnUnbindItem(VisualElement element, int index)
        {
            Debug.LogError(nameof(OnUnbindItem));
        }
    }
}