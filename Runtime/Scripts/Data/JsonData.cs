using System;
using UnityEditor;
using Object = UnityEngine.Object;

namespace Ludwell.Scene
{
    [Serializable]
    public class JsonData
    {
        public Object Original;
        public string Json;

        private void CopyDataToOriginal(Object source)
        {
            // todo: cache the original values
            // todo: reapply original values when play mode has exited
            EditorUtility.CopySerializedIfDifferent(source, Original);
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
