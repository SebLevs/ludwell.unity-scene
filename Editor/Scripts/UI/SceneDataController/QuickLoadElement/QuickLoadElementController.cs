using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ludwell.Scene.Editor
{
    public class QuickLoadElementController
    {
        
        private TagsController _tagsController;
        
        public QuickLoadElementData Cache { get; set; } = new();

        public QuickLoadElementController(VisualElement view)
        {
            _tagsController = view.Q<TagsController>();
            BuildTagsController();
        }

        public void LoadScene(SceneData sceneData)
        {
            SceneDataManagerEditorApplication.OpenScene(sceneData);

            var persistentScene = DataFetcher.GetCoreScenes().PersistentScene;
            if (persistentScene)
            {
                SceneDataManagerEditorApplication.OpenSceneAdditive(sceneData);
            }

            EditorApplication.isPlaying = true;
        }

        public void OpenScene(SceneData sceneData)
        {
            SceneDataManagerEditorApplication.OpenScene(sceneData);
        }
        
        public void UpdateIsOpen(ChangeEvent<bool> evt)
        {
            Cache.IsOpen = evt.newValue;
        }

        public void UpdateName(ChangeEvent<string> evt)
        {
            Cache.Name = evt.newValue;
        }

        public void UpdateSceneData(ChangeEvent<Object> evt)
        {
            Cache.SceneData = evt.newValue as SceneData;
        }

        public void UpdateTagsContainer()
        {
            _tagsController.WithTagSubscriber(Cache);
            _tagsController.Populate();
        }
        
        private void BuildTagsController()
        {
            _tagsController.WithOptionButtonEvent(() => { _tagsController.Q<TagsManager>().Show(Cache); });
        }
    }
}
