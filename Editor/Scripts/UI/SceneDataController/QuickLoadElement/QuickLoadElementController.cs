using UnityEditor;
using UnityEngine.UIElements;

namespace Ludwell.Scene.Editor
{
    public class QuickLoadElementController
    {
        private ViewManager _viewManager;

        private readonly TagsShelfController _tagsShelfController;

        private QuickLoadElementData _data = new();

        private VisualElement _view;

        public QuickLoadElementController(VisualElement view)
        {
            _view = view;
            _tagsShelfController = new TagsShelfController(view, _ => InitializeViewTransition());

            view.RegisterCallback<AttachToPanelEvent>(_ => { _viewManager = view.Root().Q<ViewManager>(); });

            view.Q<Toggle>().RegisterCallback<MouseUpEvent>(SaveQuickLoadElements);
        }

        ~QuickLoadElementController()
        {
            _view.Q<Toggle>().UnregisterCallback<MouseUpEvent>(SaveQuickLoadElements);
        }

        public void LoadScene()
        {
            SceneDataManagerEditorApplication.OpenScene(_data.SceneData);

            var persistentScene = DataFetcher.GetCoreScenes().PersistentScene;
            if (persistentScene)
            {
                SceneDataManagerEditorApplication.OpenSceneAdditive(_data.SceneData);
            }

            EditorApplication.isPlaying = true;
        }

        public void OpenScene()
        {
            SceneDataManagerEditorApplication.OpenScene(_data.SceneData);
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
        
        public void SelectSceneDataInProject()
        {
            Selection.activeObject = _data.SceneData;
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

        public void SetSceneData(QuickLoadElementView view)
        {
            view.SetSceneData(_data.SceneData);
        }
        
        public void SetIconAssetOutsideAssets(QuickLoadElementView view)
        {
            view.SetIconAssetOutsideAssets(_data.IsOutsideAssetsFolder);
        }

        private void InitializeViewTransition()
        {
            _viewManager.TransitionToFirstViewOfType<TagsManagerController>(new TagsManagerViewArgs(_data));
        }
        
        private void SaveQuickLoadElements(MouseUpEvent evt)
        {
            DataFetcher.SaveQuickLoadElementsDelayed();
        }
    }
}