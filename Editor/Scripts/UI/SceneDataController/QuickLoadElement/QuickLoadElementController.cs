using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ludwell.Scene.Editor
{
    public class QuickLoadElementController
    {
        private ViewManager _viewManager;

        private readonly TagsShelfController _tagsShelfController;

        private QuickLoadElementData _model = new();

        private VisualElement _view;

        private DelayedEditorUpdateAction _updateAssetNameDelayed;

        private string _cachedUpatedName;
        
        public QuickLoadElementController(VisualElement view)
        {
            _view = view;
            _tagsShelfController = new TagsShelfController(view, _ => InitializeViewTransition());

            view.RegisterCallback<AttachToPanelEvent>(_ => { _viewManager = view.Root().Q<ViewManager>(); });

            view.Q<Toggle>().RegisterCallback<MouseUpEvent>(SaveQuickLoadElements);
            
            _updateAssetNameDelayed = new DelayedEditorUpdateAction(0.5f, UpdateAndSaveAssetDelayed);
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
            _model = data;
            _cachedUpatedName = _model.Name;
        }

        public void UpdateIsOpen(ChangeEvent<bool> evt)
        {
            _model.IsOpen = evt.newValue;
        }

        public void UpdateName(ChangeEvent<string> evt)
        {
            _model.Name = evt.newValue;
        }
        
        public void SelectSceneDataInProject(ClickEvent evt)
        {
            Selection.activeObject = _model.SceneData;
            EditorGUIUtility.PingObject(Selection.activeObject);
        }

        public void UpdateTagsContainer() 
        {
            _tagsShelfController.UpdateData(_model);
            _tagsShelfController.Populate();
        }
        
        public void UpdateAndSaveAssetName(string value)
        {
            // Debug.LogError($"value: {value} | model name: {_model.SceneData.name} | cache: {_cachedUpatedName}");
            if (value == _model.SceneData.name) return;
            _cachedUpatedName = value;
            _updateAssetNameDelayed.StartOrRefresh();
        }

        private void UpdateAndSaveAssetDelayed() 
        {
            Debug.LogError("todo: delayed asset name change + save after ");
            AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(_model.SceneData), _cachedUpatedName);
            SortAndRefocus();
        }

        public void SetIsOpen(QuickLoadElementView view)
        {
            view.SetIsOpen(_model.IsOpen);
        }

        public void SetSceneData(QuickLoadElementView view)
        {
            view.SetSceneData(_model.SceneData);
        }
        
        public void SetIconAssetOutsideAssets(QuickLoadElementView view) 
        {
            view.SetIconAssetOutsideAssets(_model.IsOutsideAssetsFolder);
        }

        private void InitializeViewTransition()
        {
            _viewManager.TransitionToFirstViewOfType<TagsManagerController>(new TagsManagerViewArgs(_model));
        }
        
        private void SaveQuickLoadElements(MouseUpEvent evt)
        {
            ResourcesFetcher.SaveQuickLoadElementsDelayed();
        }
        
        private void LoadScene()
        {
            QuickLoadSceneDataManager.LoadScene(_model.SceneData);
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
            SceneDataManagerEditorApplication.OpenScene(_model.SceneData);
        }

        private void SortAndRefocus()
        {
            Debug.LogError("Sort and refocus (scroll to) current element");
            ResourcesFetcher.GetQuickLoadElements().Elements.Sort();
            Signals.Dispatch<UISignals.RefreshView>();

            var quickLoadController = ResourcesFetcher.QuickLoadController;
            Debug.LogError($"Quick load: {quickLoadController}");
            var index = ResourcesFetcher.GetQuickLoadElements().Elements.FindIndex(x => x == _model);
            quickLoadController.ScrollToItemIndex(index);
        }  
    }
} 