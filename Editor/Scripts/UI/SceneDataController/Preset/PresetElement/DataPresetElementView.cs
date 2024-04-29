using System.IO;
using UnityEngine.UIElements;

namespace Ludwell.Scene.Editor
{
    public class DataPresetElementView : VisualElement
    {
        private static readonly string UxmlPath =
            Path.Combine("UI", nameof(DataPresetElementView), "Uxml_" + nameof(DataPresetElementView));

        private static readonly string UssPath =
            Path.Combine("UI", nameof(DataPresetElementView), "Uss_" + nameof(DataPresetElementView));

        private DataPresetElementController _controller;
        
        public DataPresetElementView()
        {
            this.AddHierarchyFromUxml(UxmlPath);
            this.AddStyleFromUss(UssPath);

            _controller = new DataPresetElementController(this);
        }
    }
}
