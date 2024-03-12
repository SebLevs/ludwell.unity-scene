using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ludwell.Scene.Editor
{
    public class QuickLoadElementController
    {
        private readonly TagsShelfView _tagsShelfView;

        private QuickLoadElementData _data = new();

        public QuickLoadElementController(VisualElement view)
        {
            _tagsShelfView = view.Q<TagsShelfView>();
            BuildTagsController(view);
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

        public void UpdateData(QuickLoadElementData data)
        {
            _data = data;
        }

        public void UpdateIsOpen(ChangeEvent<bool> evt)
        {
            _data.IsOpen = evt.newValue;
        }

        public void UpdateName(ChangeEvent<string> evt)
        {
            _data.Name = evt.newValue;
        }

        public void UpdateSceneData(ChangeEvent<Object> evt)
        {
            _data.SceneData = evt.newValue as SceneData;
        }

        public void UpdateTagsContainer()
        {
            _tagsShelfView.WithTagSubscriber(_data);
            _tagsShelfView.PopulateContainer();
        }

        public void SetIsOpen(QuickLoadElementView view)
        {
            view.SetIsOpen(_data.IsOpen);
        }

        public void SetName(QuickLoadElementView view)
        {
            view.SetName(_data.Name);
        }

        public void SetSceneData(QuickLoadElementView view)
        {
            view.SetSceneData(_data.SceneData);
        }

        private void BuildTagsController(VisualElement view)
        {
            _tagsShelfView.WithOptionButtonEvent(() =>
            {
                var root = view.Root();
                var tagsManager = root.Q<TagsManagerView>();
                tagsManager.ShowDelegated(_data, root.Q<SceneDataController>());
            });
        }
    }
}