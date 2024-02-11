using Ludwell.Scene.Editor;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ludwell.Scene
{
    public class LoaderListViewElement : VisualElement, IBindableListViewElement<LoaderListViewElementData>
    {
        private const string UxmlPath = "Uxml/" + nameof(LoaderController) + "/" + nameof(LoaderListViewElement);
        private const string UssPath = "Uss/" + nameof(LoaderController) + "/" + nameof(LoaderListViewElement);

        private const string HeaderContentUxmlPath =
            "Uxml/" + nameof(LoaderController) + "/scene-loader-element__header-content";

        private const string HeaderContentUssPath =
            "Uss/" + nameof(LoaderController) + "/scene-loader-element__header-content";

        private const string FoldoutName = "root__foldout";
        private const string FoldoutTextFieldName = "foldout-text-field";
        private const string ToggleBottomName = "toggle-bottom";
        private const string MainSceneName = "main-scene";
        private const string RequiredScenesListViewName = "required-scenes";
        private const string LoadButtonName = "button__load";
        private const string OpenButtonName = "button__open";
        private const string ReorderableHandleName = "unity-list-view__reorderable-handle";

        public const string DefaultHeaderTextValue = "Scene Loader Element";

        private VisualElement _reorderableHandle;
        private Foldout _foldoutElement;
        private TextField _foldoutTextField;
        private ObjectField _mainSceneField;
        private TagsController _tagsController;

        private ListView _listViewRequiredElements;
        private ListViewInitializer<RequiredSceneElement, SceneDataReference> _listViewInitializer;

        public LoaderListViewElementData Cache { get; set; } = new();

        public LoaderListViewElement()
        {
            this.AddHierarchyFromUxml(UxmlPath);
            this.AddStyleFromUss(UssPath);
            SetReferences();
            InitAndReferenceFoldoutTextField();
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
            _listViewRequiredElements = this.Q<ListView>(RequiredScenesListViewName);
            _tagsController = this.Q<TagsController>();
        }

        private void InitAndReferenceFoldoutTextField()
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
            CleanupStyle();
            _foldoutElement.RegisterValueChangedCallback(BindFoldoutValue);
            _foldoutTextField.RegisterValueChangedCallback(BindFoldoutTextField);
            _mainSceneField.RegisterValueChangedCallback(BindMainSceneField);
        }

        private void CleanupStyle()
        {
            _reorderableHandle ??= parent.parent.Q<VisualElement>(ReorderableHandleName);
            _reorderableHandle.style.display = DisplayStyle.None;
            _listViewRequiredElements.Rebuild();
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
            _tagsController.WithTags(Cache.Tags);
            _tagsController.Refresh();
            HandleRequiredSceneListView();
        }

        private void HandleRequiredSceneListView()
        {
            InitRequiredScenesListView();
            PreventRequiredElementWheelCallbackPropagation();
        }

        private void InitRequiredScenesListView()
        {
            _listViewInitializer = new(_listViewRequiredElements, Cache.RequiredScenes);
            _listViewRequiredElements.itemsRemoved += _ => LoaderSceneDataHelper.SaveChangeDelayed();
        }

        private void PreventRequiredElementWheelCallbackPropagation()
        {
            var scroller = _listViewRequiredElements.Q<Scroller>();
            _listViewRequiredElements.RegisterCallback<WheelEvent>(evt =>
            {
                if (scroller.style.display == DisplayStyle.None) return;
                if (evt.delta.y < 0 && Mathf.Approximately(scroller.value, scroller.lowValue))
                {
                    evt.StopPropagation();
                }
                else if (evt.delta.y > 0 && Mathf.Approximately(scroller.value, scroller.highValue))
                {
                    evt.StopPropagation();
                }
            });
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

                foreach (var requiredScene in Cache.RequiredScenes)
                {
                    if (requiredScene.SceneData == null)
                    {
                        Debug.LogError($" {_foldoutTextField.value} | Cannot load a null required scene.");
                        return;
                    }
                }

                if (evt.currentTarget != loadButton) return;
                SceneDataManagerEditorApplication.OpenScene(_mainSceneField.value as SceneData);

                var persistentScene = LoaderSceneDataHelper.GetLoaderSceneData().PersistentScene;
                if (persistentScene)
                {
                    var cache = AssetDatabase.GetAssetPath(persistentScene.EditorSceneAsset);
                    EditorSceneManager.OpenScene(cache, OpenSceneMode.Additive);
                }

                foreach (var requiredScene in Cache.RequiredScenes)
                {
                    var cache = AssetDatabase.GetAssetPath(requiredScene.SceneData.EditorSceneAsset);
                    EditorSceneManager.OpenScene(cache, OpenSceneMode.Additive);
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