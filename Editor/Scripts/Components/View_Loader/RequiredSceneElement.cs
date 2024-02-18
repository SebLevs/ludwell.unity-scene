using System.IO;
using Ludwell.Scene.Editor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ludwell.Scene
{
    public class RequiredSceneElement : VisualElement, IBindableListViewElement<SceneDataReference>
    {
        private static readonly string UxmlPath = Path.Combine("Uxml", nameof(LoaderController), nameof(RequiredSceneElement));
        private static readonly string UssPath = Path.Combine("Uss", nameof(LoaderController), nameof(RequiredSceneElement));

        private const string ObjectFieldName = "scene-data";

        private ObjectField _requiredSceneField;

        public SceneDataReference Cache { get; set; }

        public RequiredSceneElement()
        {
            this.AddHierarchyFromUxml(UxmlPath);
            this.AddStyleFromUss(UssPath);
            SetReferences();
        }

        private void SetReferences()
        {
            _requiredSceneField = this.Q<ObjectField>(ObjectFieldName);
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