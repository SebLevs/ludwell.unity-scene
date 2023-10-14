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
            _sceneField = this.Q<ObjectField>();
        }

        private const string UxmlPath = "Uxml/required-scene-element";
        private const string UssPath = "Uss/required-scene-element";

        private ObjectField _sceneField;

        public void InitDataValues(SceneData data)
        {
        }

        public void BindElementToData(SceneData data)
        {
            _sceneField.RegisterValueChangedCallback(evt =>
            {
                data = evt.newValue as SceneData;
            });
        }

        public void SetElementFromData(SceneData data)
        {
            _sceneField = this.Q<ObjectField>();
            _sceneField.value = data;
        }
    }
}