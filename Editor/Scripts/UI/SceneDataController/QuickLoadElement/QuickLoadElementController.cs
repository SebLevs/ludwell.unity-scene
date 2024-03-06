using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ludwell.Scene.Editor
{
    public class QuickLoadElementController
    {
        private readonly TagsController _tagsController;

        public QuickLoadElementData Data { private get; set; } = new();

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
            Data.IsOpen = evt.newValue;
        }

        public void UpdateName(ChangeEvent<string> evt)
        {
            Data.Name = evt.newValue;
        }

        public void UpdateSceneData(ChangeEvent<Object> evt)
        {
            Data.SceneData = evt.newValue as SceneData;
        }

        public void UpdateTagsContainer()
        {
            _tagsController.WithTagSubscriber(Data);
            _tagsController.Populate();
        }

        public void SetIsOpen(QuickLoadElementView view)
        {
            view.SetIsOpen(Data.IsOpen);
        }

        public void SetName(QuickLoadElementView view)
        {
            view.SetName(Data.Name);
        }

        public void SetSceneData(QuickLoadElementView view)
        {
            view.SetSceneData(Data.SceneData);
        }

        private void BuildTagsController()
        {
            _tagsController.WithOptionButtonEvent(() => { _tagsController.Q<TagsManager>().Show(Data); });
        }
    }
}