using System.IO;
using Ludwell.Scene.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ludwell.Scene
{
    public class QuickLoadElement : VisualElement, IListViewVisualElement<LoaderListViewElementData>
    {
        private static readonly string UxmlPath =
            Path.Combine("Uxml", nameof(SceneDataController), nameof(QuickLoadElement));

        private static readonly string UssPath =
            Path.Combine("Uss", nameof(SceneDataController), nameof(QuickLoadElement));

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

        public const string DefaultHeaderTextValue = "Quick load element";

        private VisualElement _reorderableHandle;
        private Foldout _foldoutElement;
        private TextField _foldoutTextField;
        private ObjectField _mainSceneField;
        private TagsController _tagsController;

        public LoaderListViewElementData Cache { get; set; } = new();

        public QuickLoadElement()
        {
            this.AddHierarchyFromUxml(UxmlPath);
            this.AddStyleFromUss(UssPath);

            SetReferences();
            InitializeAndReferenceFoldoutTextField();
            RegisterStyleEvents();

            RegisterButtonsClickEventCallback();
            PreventFoldoutToggleFromKeyPress();

            BuildTagsController();
        }

        public void SetFoldoutValue(bool value) => _foldoutElement.value = value;

        private void SetReferences()
        {
            _foldoutElement = this.Q<Foldout>(FoldoutName);
            _mainSceneField = this.Q<ObjectField>(MainSceneName);
            _tagsController = this.Q<TagsController>();
        }

        private void InitializeAndReferenceFoldoutTextField()
        {
            var headerContent = Resources.Load<VisualTreeAsset>(HeaderContentUxmlPath).CloneTree().ElementAt(0);
            headerContent.AddStyleFromUss(HeaderContentUssPath);
            this.Q<Toggle>().Q<VisualElement>().Add(headerContent);
            _foldoutTextField = this.Q<TextField>(FoldoutTextFieldName);
        }

        private void RegisterStyleEvents()
        {
            _foldoutElement.RegisterValueChangedCallback(evt =>
            {
                var borderTopWidth = evt.newValue ? 1 : 0;
                this.Q(ToggleBottomName).style.borderTopWidth = borderTopWidth;
            });
        }

        public void BindElementToCachedData()
        {
            _foldoutElement.RegisterValueChangedCallback(BindFoldoutValue);
            _foldoutTextField.RegisterValueChangedCallback(BindFoldoutTextField);
            _mainSceneField.RegisterValueChangedCallback(BindMainSceneField);
        }

        private void BindFoldoutValue(ChangeEvent<bool> evt)
        {
            Cache.IsOpen = evt.newValue;
        }

        private void BindFoldoutTextField(ChangeEvent<string> evt)
        {
            Cache.Name = evt.newValue;
        }

        private void BindMainSceneField(ChangeEvent<Object> evt)
        {
            Cache.MainScene = evt.newValue as SceneData;
        }

        public void SetElementFromCachedData()
        {
            _foldoutTextField.value = Cache.Name;
            _foldoutElement.value = Cache.IsOpen;
            _mainSceneField.value = Cache.MainScene;
            _tagsController.WithTagSubscriber(Cache);
            _tagsController.Populate();
        }

        private void RegisterButtonsClickEventCallback()
        {
            RegisterLoadButtonEvents();
            RegisterOpenButtonEvents();
        }

        private void RegisterLoadButtonEvents()
        {
            var loadButton = this.Q(LoadButtonName).Q<Button>();
            loadButton.RegisterCallback<ClickEvent>(evt =>
            {
                if (_mainSceneField.value == null)
                {
                    Debug.LogError($"{_foldoutTextField.value} | Cannot load without a main scene.");
                    return;
                }

                if (evt.currentTarget != loadButton) return;
                SceneDataManagerEditorApplication.OpenScene(_mainSceneField.value as SceneData);

                var persistentScene = DataFetcher.GetCoreScenes().PersistentScene;
                if (persistentScene)
                {
                    SceneDataManagerEditorApplication.OpenSceneAdditive(_mainSceneField.value as SceneData);
                }

                EditorApplication.isPlaying = true;
            });
        }

        private void RegisterOpenButtonEvents()
        {
            var openButton = this.Q(OpenButtonName).Q<Button>();
            openButton.RegisterCallback<ClickEvent>(evt =>
            {
                if (_mainSceneField.value == null)
                {
                    Debug.LogError("Cannot open without a main scene.");
                    return;
                }

                if (evt.currentTarget != openButton) return;
                SceneDataManagerEditorApplication.OpenScene(_mainSceneField.value as SceneData);
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
                _foldoutElement.value = !_foldoutElement.value;
            });

            foldoutTextField.RegisterCallback<ClickEvent>(evt => evt.StopPropagation());
        }

        private void BuildTagsController()
        {
            _tagsController.WithOptionButtonEvent(() => { this.Root().Q<TagsManager>().Show(Cache); });
        }
    }
}