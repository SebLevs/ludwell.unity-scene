using UnityEngine;

namespace Ludwell.Scene
{
    public class CoreScenes : ScriptableObject
    {
        [HideInInspector] public SceneData StartingScene;
        [HideInInspector] public SceneData PersistentScene;
        [HideInInspector] public SceneData LoadingScene;
    }
}
