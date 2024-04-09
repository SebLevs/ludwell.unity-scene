using UnityEngine;

namespace Ludwell.Scene
{
    // [CreateAssetMenu(fileName = "CoreScenes", menuName = "SceneDataManager/CoreScenes")]
    public class CoreScenes : ScriptableObject
    {
        [HideInInspector] public SceneData StartingScene;
        [HideInInspector] public SceneData PersistentScene;
        [HideInInspector] public SceneData LoadingScene;
    }
}
