using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ludwell.Scene.Editor
{
    public class QuickLoadElementController
    {
        private ViewManager _viewManager;

        private readonly TagsShelfController _tagsShelfController;

        private QuickLoadElementData _data = new();

        public QuickLoadElementController(VisualElement view)
        {
            _tagsShelfController = new TagsShelfController(view, _ => InitializeViewTransition());

            view.RegisterCallback<AttachToPanelEvent>(_ => { _viewManager = view.Root().Q<ViewManager>(); });
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
            _tagsShelfController.UpdateData(_data);
            _tagsShelfController.Populate();
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

        private void InitializeViewTransition()
        {
            _viewManager.TransitionToFirstViewOfType<TagsManagerController>(new TagsManagerViewArgs(_data));
        }
    }
}