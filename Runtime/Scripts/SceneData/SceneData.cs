using UnityEngine;

public class SceneData : ScriptableObject
{
#if UNITY_EDITOR
    public UnityEditor.SceneAsset SceneAsset;
#endif
}
