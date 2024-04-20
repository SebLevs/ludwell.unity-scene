using System;
using UnityEngine;

namespace Ludwell.Scene.Editor
{
    [Serializable]
    public class ExampleSerializedClass
    {
        [SerializeField] private string _stringValue;
        [Space]
        [SerializeField] private int[] _integerArray;
    }
}