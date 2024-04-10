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

        public void InitializeLoadButton(DualStateButton dualStateButton)
        {
            var stateOne = new DualStateButtonState(
                dualStateButton,
                LoadScene,
                Resources.Load<Sprite>(SpritesPath.LoadIcon));

            var stateTwo = new DualStateButtonState(
                dualStateButton,
                EditorApplication.ExitPlaymode,
                Resources.Load<Sprite>(SpritesPath.StopIcon));

            dualStateButton.Initialize(stateOne, stateTwo);
        }
        
        public void InitializeOpenButton(ButtonWithIcon buttonWithIcon)
        {
            buttonWithIcon.SetIcon(Resources.Load<Sprite>(SpritesPath.OpenIcon));
            buttonWithIcon.clicked += OpenScene;
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
            EditorGUIUtility.PingObject(Selection.activeObject);
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
        
        private void LoadScene()
        {
            QuickLoadSceneDataManager.LoadScene(_data.SceneData);
            EditorApplication.playModeStateChanged += OnExitPlayModeSwitchToStateOne;
        }

        private void OnExitPlayModeSwitchToStateOne(PlayModeStateChange obj)
        {
            if (obj != PlayModeStateChange.ExitingPlayMode) return;
            EditorApplication.playModeStateChanged -= OnExitPlayModeSwitchToStateOne;
            var dualStateButton = _view.Q<DualStateButton>();
            dualStateButton.SwitchState(dualStateButton.StateOne);
        }

        private void OpenScene()
        {
            SceneDataManagerEditorApplication.OpenScene(_data.SceneData);
        }
    }
}