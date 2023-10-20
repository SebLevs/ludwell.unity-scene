using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ludwell.Scene
{
    public class RequiredSceneElement : VisualElement, IBindableListViewElement<SceneDataReference>
    {
        public RequiredSceneElement()
        {
            this.SetHierarchyFromUxml(UxmlPath);
            this.AddStyleFromUss(UssPath);
            SetReferences();
        }

        private const string UxmlPath = "Uxml/required-scene-element";
        private const string UssPath = "Uss/required-scene-element";

        private const string ObjectFieldName = "scene-data";

        private ObjectField _requiredSceneField;

        public SceneDataReference Cache { get; set; }

        private void SetReferences()
        {
            _requiredSceneField = this.Q<ObjectField>(ObjectFieldName);
        }

        public void CacheData(SceneDataReference data)
        {
            Cache = data;
        }

        public void BindElementToCachedData()
        {
            _requiredSceneField.RegisterValueChangedCallback(BindObjectField);
        }

        private void BindObjectField(ChangeEvent<Object> evt)
        {
            Cache.SceneData = evt.newValue as SceneData;
        }

        public void SetElementFromCachedData()
        {
            _requiredSceneField.value = Cache.SceneData;
        }
    }
}