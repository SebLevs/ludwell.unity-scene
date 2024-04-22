using UnityEngine;

namespace Ludwell.Scene.Editor
{
    [CreateAssetMenu(fileName = "ExampleScriptable", menuName = "ExampleScriptable")]
    public class ExampleScriptable : ScriptableObject
    {
        public int amIFucked;
        [SerializeField] private int _integerValue;
        [SerializeField] private string[] _stringArray;

        [Space(20)]
        [SerializeField] private ExampleSerializedClass _exampleSerializedClass;
    }
}