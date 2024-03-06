using System.IO;
using Ludwell.Scene.Editor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ludwell.Scene
{
    public class QuickLoadElementView : VisualElement, IListViewVisualElement<QuickLoadElementData>
    {
        private static readonly string UxmlPath =
            Path.Combine("Uxml", nameof(SceneDataController), nameof(QuickLoadElementView));

        private static readonly string UssPath =
            Path.Combine("Uss", nameof(SceneDataController), nameof(QuickLoadElementView));

        private static readonly string HeaderContentUxmlPath =
            Path.Combine("Uxml", nameof(SceneDataController), "quick-load-element__header-content");

        private static readonly string HeaderContentUssPath =
            Path.Combine("Uss", nameof(SceneDataController), "quick-load-element__header-content");

        private const string FoldoutName = "root__foldout";
        private const string FoldoutTextFieldName = "foldout-text-field";
        private const string ToggleBottomName = "toggle-bottom";
        private const string MainSceneName = "main-scene";
        private const string LoadButtonName = "button__load";
        private const string OpenButtonName = "button__open";

        private Foldout _foldout;
        private TextField _foldoutText;
        private ObjectField _sceneData;

        private readonly QuickLoadElementController _controller;
        
        public void SetFoldoutValue(bool value) => _foldout.value = value;
        
        public void SetFoldoutText(string text) => _foldoutText.value = text;

        public void SetSceneData(SceneData sceneData) => _sceneData.value = sceneData;
        
        public QuickLoadElementData Cache 
        {
            get => _controller.Cache;
            set => _controller.Cache = value;
        }
        
        public QuickLoadElementView()
        {
            this.AddHierarchyFromUxml(UxmlPath);
            this.AddStyleFromUss(UssPath);

            SetReferences();
            InitializeAndReferenceFoldoutTextField();
            RegisterStyleEvents();

            RegisterButtonsClickEventCallback();
            PreventFoldoutToggleFromKeyPress();

            _controller = new QuickLoadElementController(this);
        }

        private void SetReferences()
        {
            _foldout = this.Q<Foldout>(FoldoutName);
            _sceneData = this.Q<ObjectField>(MainSceneName);
        }

        private void InitializeAndReferenceFoldoutTextField()
        {
            var headerContent = Resources.Load<VisualTreeAsset>(HeaderContentUxmlPath).CloneTree().ElementAt(0);
            headerContent.AddStyleFromUss(HeaderContentUssPath);
            this.Q<Toggle>().Q<VisualElement>().Add(headerContent);
            _foldoutText = this.Q<TextField>(FoldoutTextFieldName);
        }

        public void BindElementToCachedData()
        {
            _foldout.RegisterValueChangedCallback(_controller.UpdateIsOpen);
            _foldoutText.RegisterValueChangedCallback(_controller.UpdateName);
            _sceneData.RegisterValueChangedCallback(_controller.UpdateSceneData);
        }

        public void SetElementFromCachedData()
        {
            SetFoldoutValue(_controller.Cache.IsOpen);
            SetFoldoutText(_controller.Cache.Name);
            SetSceneData(_controller.Cache.SceneData);

            _controller.UpdateTagsContainer();
        }

        private void RegisterButtonsClickEventCallback()
        {
            RegisterLoadButtonEvents();
            RegisterOpenButtonEvents();
        }

        private void RegisterLoadButtonEvents()
        {
            var loadButton = this.Q(LoadButtonName).Q<Button>();
            loadButton.RegisterCallback<ClickEvent>(_ => _controller.LoadScene(_sceneData.value as SceneData));
        }

        private void RegisterOpenButtonEvents()
        {
            var openButton = this.Q(OpenButtonName).Q<Button>();
            openButton.RegisterCallback<ClickEvent>(_ => _controller.OpenScene(_sceneData.value as SceneData));
        }
        
        private void RegisterStyleEvents()
        {
            _foldout.RegisterValueChangedCallback(evt =>
            {
                var borderTopWidth = evt.newValue ? 1 : 0;
                this.Q(ToggleBottomName).style.borderTopWidth = borderTopWidth;
            });
        }

        private void PreventFoldoutToggleFromKeyPress()
        {
            var foldoutTextField = this.Q<TextField>(FoldoutTextFieldName);
            foldoutTextField.RegisterCallback<KeyDownEvent>(evt =>
            {
                evt.StopPropagation();
                if (evt.currentTarget != foldoutTextField) return;
                if (evt.keyCode != KeyCode.Return && evt.keyCode != KeyCode.Space) return;
                SetFoldoutValue(!_foldout.value);
            });

            foldoutTextField.RegisterCallback<ClickEvent>(evt => evt.StopPropagation());
        }
    }
}