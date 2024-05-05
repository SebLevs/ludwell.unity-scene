using System.IO;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ludwell.Scene.Editor
{
    public class DataPresetElementController : VisualElement, IListViewVisualElement<JsonData>
    {
        private static readonly string UxmlPath =
            Path.Combine("UI", nameof(DataPresetElementView), "Uxml_" + nameof(DataPresetElementView));

        private static readonly string UssPath =
            Path.Combine("UI", nameof(DataPresetElementView), "Uss_" + nameof(DataPresetElementView));

        private const string ReferencedAssetName = "reference";
        private const string ClassContentName = "class-content";

        private ObjectField _objectField;
        private VisualElement _classContent;

        private DataPresetElementModel _model = new();
        private DataPresetElementView _view;

        public DataPresetElementController()
        {
            this.AddHierarchyFromUxml(UxmlPath);
            this.AddStyleFromUss(UssPath);

            _view = new DataPresetElementView(this);

            _objectField = this.Q<ObjectField>(ReferencedAssetName);
            _classContent = this.Q<VisualElement>(ClassContentName);
        }

        private void UpdateSelection(ChangeEvent<Object> evt) // todo: cleanup
        {
            _classContent.Clear();

            if (evt.newValue == null) return;

            var copy = Object.Instantiate(evt.newValue); // todo: make it work for json files?

            _model.JsonData.Original = evt.newValue;
            _model.JsonData.Json = JsonUtility.ToJson(copy);
            _view.UpdateToggleLabel(evt.newValue.name);

            var serializedObject = new SerializedObject(copy);
            var container = new IMGUIContainer(() =>
            {
                serializedObject.Update();

                var iterator = serializedObject.GetIterator();
                iterator.NextVisible(true); // skip base
                iterator.NextVisible(true); // hides class object field
                EditorGUILayout.PropertyField(iterator);
                while (iterator.NextVisible(false))
                {
                    EditorGUILayout.PropertyField(iterator);
                }

                if (!serializedObject.ApplyModifiedProperties()) return;

                // todo: wait N time and save the json
                var json = JsonUtility.ToJson(copy);
                _model.JsonData.Json = json;

                // LOG TEST
                // var fromJson = DeserializeScriptableObject(copy, json);
                // Debug.LogError((fromJson as ExampleScriptable).amIFucked);
            });
            
            _classContent.Add(container);
        }

        public void CacheData(JsonData data)
        {
            _model.JsonData = data;
        }

        public void BindElementToCachedData()
        {
            _objectField.RegisterCallback<ChangeEvent<Object>>(UpdateSelection);
        }

        public void SetElementFromCachedData()
        {
            if (_model.JsonData.Original == null) return;
            var copy = Object.Instantiate(_model.JsonData.Original); // todo: make it work for json files?
            _objectField.value = DeserializeScriptableObject(copy, _model.JsonData.Json);
        }

        // todo: call when any data preset is loaded in the inspector
        private ScriptableObject DeserializeScriptableObject(Object copy, string json)
        {
            var fromJson = ScriptableObject.CreateInstance(copy.GetType());
            JsonUtility.FromJsonOverwrite(json, fromJson);
            return fromJson;
        }
    }
}