using System.IO;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ludwell.Scene.Editor
{
    public class DataPresetElementView : VisualElement
    {
        private static readonly string UxmlPath =
            Path.Combine("UI", nameof(DataPresetElementView), "Uxml_" + nameof(DataPresetElementView));

        private static readonly string UssPath =
            Path.Combine("UI", nameof(DataPresetElementView), "Uss_" + nameof(DataPresetElementView));
        
        private const string ReferencedAssetName = "reference";
        private const string ClassContentName = "class-content";

        private Toggle _toggle;
        private ObjectField _objectField;
        private VisualElement _classContentContainer;

        private DataPresetElementController _controller;
        
        public DataPresetElementView()
        {
            this.AddHierarchyFromUxml(UxmlPath);
            this.AddStyleFromUss(UssPath);

            _controller = new DataPresetElementController(this);

            _toggle = this.Q<Toggle>();
            _objectField = this.Q<ObjectField>(ReferencedAssetName);
            _classContentContainer = this.Q<VisualElement>(ClassContentName);
        }
    }
}
