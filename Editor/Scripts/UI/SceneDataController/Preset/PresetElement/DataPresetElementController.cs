using System.IO;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ludwell.Scene.Editor
{
    public class DataPresetElementController : VisualElement//, IListViewVisualElement<QuickLoadElementData>
    {
        private static readonly string UxmlPath =
            Path.Combine("UI", nameof(DataPresetElementController), "Uxml_" + nameof(DataPresetElementController));

        private static readonly string UssPath =
            Path.Combine("UI", nameof(DataPresetElementController), "Uss_" + nameof(DataPresetElementController));
        
        private const string ReferencedAssetName = "reference";
        private const string ClassContentName = "class-content";

        private ObjectField _objectField;
        private VisualElement _classContent;

        // private DataPresetSliderModel _model;
        private DataPresetElementView _view;
        
        public DataPresetElementController()
        {
            this.AddHierarchyFromUxml(UxmlPath);
            this.AddStyleFromUss(UssPath);

            _view = new DataPresetElementView(this);
            
            _objectField = this.Q<ObjectField>(ReferencedAssetName);
            _objectField.RegisterCallback<ChangeEvent<Object>>(UpdateSelection);
            
            _classContent = this.Q<VisualElement>(ClassContentName);
        }

        private void UpdateSelection(ChangeEvent<Object> evt)
        { 
            _classContent.Clear();

            var copy = Object.Instantiate(evt.newValue); // todo: make it work for json files?

            var serializedObject = new SerializedObject(copy);
            var container = new IMGUIContainer(() =>
            {
                serializedObject.Update();

                var iterator = serializedObject.GetIterator();
                iterator.NextVisible(true);
                EditorGUILayout.PropertyField(iterator);
                while (iterator.NextVisible(false))
                {
                    EditorGUILayout.PropertyField(iterator);
                }

                if (!serializedObject.ApplyModifiedProperties()) return;

                // todo: wait N time and save the json
                var json = JsonUtility.ToJson(copy);
                Debug.LogError(json);

                var fromJson = DeserializeScriptableObject(copy, json);
                Debug.LogError((fromJson as ExampleScriptable).amIFucked);

                CopyData(copy, evt.newValue);
            });
            
            _classContent.Add(container);
            _view.UpdateToggleLabel(serializedObject.GetIterator().Next(true).ToString());
        }

        // todo: call when any data preset is loaded in the inspector
        private ScriptableObject DeserializeScriptableObject(Object copy, string json)
        {
            var fromJson = ScriptableObject.CreateInstance(copy.GetType());
            JsonUtility.FromJsonOverwrite(json, fromJson);
            return fromJson;
        }

        private void CopyData(Object source, Object target)
        {
            var originalName = target.name;
            EditorUtility.CopySerializedIfDifferent(source, target);
            target.name = originalName;
        }
    }
}
