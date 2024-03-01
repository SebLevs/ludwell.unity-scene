using UnityEngine;

namespace Ludwell.Scene.Editor
{
    // [CreateAssetMenu(fileName = "CoreScenes", menuName = "SceneDataManager/CoreScenes")]
    public class CoreScenes : ScriptableObject
    {
        [HideInInspector] [SerializeField] private SceneData mainMenuScene;
        [HideInInspector] [SerializeField] private SceneData persistentScene;
        [HideInInspector] [SerializeField] private SceneData loadingScene;
        
        public SceneData MainMenuScene
        {
            get => mainMenuScene;
            set
            {
                mainMenuScene = value;
                DataFetcher.SaveEveryScriptable();
            }
        }

        public SceneData PersistentScene
        {
            get => persistentScene;
            set
            {
                persistentScene = value;
                DataFetcher.SaveEveryScriptable();
            }
        }

        public SceneData LoadingScene
        {
            get => loadingScene;
            set
            {
                loadingScene = value;
                DataFetcher.SaveEveryScriptable();
            }
        }

    }

}
