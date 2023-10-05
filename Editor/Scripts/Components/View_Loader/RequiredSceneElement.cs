using UnityEngine.UIElements;

namespace Ludwell.Scene
{
    public class RequiredSceneElement : VisualElement, IBindableListViewElement<RequiredSceneElement>
    {
        public new class UxmlFactory : UxmlFactory<RequiredSceneElement, UxmlTraits> { }
        public new class UxmlTraits : VisualElement.UxmlTraits { }
        
        public RequiredSceneElement()
        {
            this.SetHierarchyFromUxml(UxmlPath);
            this.AddStyleFromUss(UssPath);
        }
        
        private const string UxmlPath = "Uxml/required-scene-element";
        private const string UssPath = "Uss/required-scene-element";

        public void BindElementToData(RequiredSceneElement data)
        {
            throw new System.NotImplementedException();
        }

        public void SetElementFromData(RequiredSceneElement data)
        {
            throw new System.NotImplementedException();
        }
    }
}
