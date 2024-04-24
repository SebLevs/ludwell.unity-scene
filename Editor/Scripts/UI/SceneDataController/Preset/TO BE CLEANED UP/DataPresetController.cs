using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace Ludwell.Scene.Editor
{
    public class DataPresetController : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<DataPresetController, UxmlTraits>
        {
        }

        private ObjectField _objectField;
        private VisualElement _objectContainer;

        public DataPresetController()
        {
            _objectField = new ObjectField();
            Add(_objectField);
            _objectContainer = new VisualElement();
            Add(_objectContainer);

            _objectField.RegisterCallback<ChangeEvent<Object>>(UpdateSelection);
        }

        private void UpdateSelection(ChangeEvent<Object> evt)
        {
            _objectContainer.Clear();

            var copy = Object.Instantiate(evt.newValue);

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

            _objectContainer.Add(container);
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