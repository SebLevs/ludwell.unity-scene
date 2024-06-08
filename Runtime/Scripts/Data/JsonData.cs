using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Ludwell.Scene
{
    [Serializable]
    public class JsonData
    {
        public Object Original;
        public string Json;

        public void CopyDataToOriginal() // Object source
        {
            // todo: cache the original values
            // todo: reapply original values when play mode has exited
            
            var fromJson = ScriptableObject.CreateInstance(Original.GetType());
            
            Debug.LogError($"deserialized: {fromJson.GetType()}");
            Debug.LogError(Json);
            Debug.LogError(fromJson);
            
            EditorUtility.CopySerializedIfDifferent(fromJson, Original);
            fromJson.name = Original.name;
        }

        // CACHE
        private void CopyData(Object source, Object target)
        {
            var originalName = target.name;
            EditorUtility.CopySerializedIfDifferent(source, target);
            target.name = originalName;
        }
    }
}
