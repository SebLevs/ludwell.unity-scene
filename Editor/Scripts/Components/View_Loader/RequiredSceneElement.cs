using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ludwell.Scene
{
    // todo: inquire about systemic null ref on _requiredSceneField
    public class RequiredSceneElement : VisualElement, IBindableListViewElement<SceneData>
    {
        public new class UxmlFactory : UxmlFactory<RequiredSceneElement, UxmlTraits> { }

        public new class UxmlTraits : VisualElement.UxmlTraits { }

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

        public void SetReferences()
        {
            _requiredSceneField = this.Q<ObjectField>(ObjectFieldName);
        }

        public void InitDataValues(SceneData data) { }

        public void BindElementToData(SceneData data)
        {
            _requiredSceneField.RegisterValueChangedCallback(evt =>
                data = evt.newValue as SceneData);
        }

        public void SetElementFromData(SceneData data)
        {
            _requiredSceneField.value = data;
        }
    }
}