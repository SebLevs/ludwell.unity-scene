using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ludwell.Scene
{
    public class RequiredSceneElement : VisualElement, IBindableListViewElement<SceneData>
    {
        public new class UxmlFactory : UxmlFactory<RequiredSceneElement, UxmlTraits> { }
        public new class UxmlTraits : VisualElement.UxmlTraits { }
        
        public RequiredSceneElement()
        {
            this.SetHierarchyFromUxml(UxmlPath);
            this.AddStyleFromUss(UssPath);
            SceneField = this.Q<ObjectField>();
        }
        
        private const string UxmlPath = "Uxml/required-scene-element";
        private const string UssPath = "Uss/required-scene-element";
        
        public ObjectField SceneField { get; set; }

        public void InitDataValues(SceneData data)
        {
            
        }

        public void BindElementToData(SceneData data)
        {
            SceneField.RegisterValueChangedCallback(evt =>
            {
                Debug.LogError($"BindElementToData: {evt.newValue}");
                data = evt.newValue as SceneData;
            });
        }

        public void SetElementFromData(SceneData data)
        {
            Debug.LogError($"SetElementFromData: {data}");
            SceneField.value = data;
        }
    }
}