using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ludwell.Scene
{
    public class LoaderListViewElement : VisualElement, IBindableListViewElement<LoaderListViewElementData>
    {
        public new class UxmlFactory : UxmlFactory<LoaderListViewElement, UxmlTraits> { }

        public new class UxmlTraits : VisualElement.UxmlTraits { }

        public LoaderListViewElement()
        {
            this.SetHierarchyFromUxml(UxmlPath);
            this.AddStyleFromUss(UssPath);
            SetReferences();
            InitAndReferenceFoldoutTextField();
            SetStyleEvents();
            RegisterButtonsClickEventCallback();
            PreventFoldoutToggleFromKeyPress();
        }

        private const string UxmlPath = "Uxml/scene-loader-element";
        private const string UssPath = "Uss/scene-loader-element";
        private const string HeaderContentUxmlPath = "Uxml/scene-loader-element__header-content";
        private const string HeaderContentUssPath = "Uss/scene-loader-element__header-content";

        private const string FoldoutName = "root__foldout";
        private const string FoldoutTextFieldName = "foldout-text-field";
        private const string ToggleBottomName = "toggle-bottom";
        private const string MainSceneName = "main-scene";
        private const string RequiredScenesListViewName = "required-scenes";
        private const string PlayButtonName = "button__play";
        private const string LoadButtonName = "button__load";

        private Foldout _foldoutElement;
        private TextField _foldoutTextField;
        private ObjectField _mainSceneField;

        private ListView _listViewRequiredElements;

        public void SetFoldoutValue(bool value) => _foldoutElement.value = value;

        public void InitDataValues(LoaderListViewElementData data)
        {
            data.IsOpen = _foldoutElement.value;
            data.Name = _foldoutTextField.value;
            data.MainScene = _mainSceneField.value as SceneData;
            HandleRequiredSceneListView(data);
        }

        public void BindElementToData(LoaderListViewElementData data)
        {
            _foldoutElement.RegisterValueChangedCallback(evt =>
                data.IsOpen = evt.newValue);

            _foldoutTextField.RegisterValueChangedCallback(evt =>
                data.Name = evt.newValue);

            _mainSceneField.RegisterValueChangedCallback(evt => 
                data.MainScene = evt.newValue as SceneData);
        }

        private LoaderListViewElementData _data;

        public void SetElementFromData(LoaderListViewElementData data)
        {
            HandleRequiredSceneListView(data);
            _foldoutTextField.value = data.Name;
            _foldoutElement.value = data.IsOpen;
            _mainSceneField.value = data.MainScene;
        }

        private void HandleRequiredSceneListView(LoaderListViewElementData data)
        {
            _data = data;
            InitRequiredScenesListView();
            PreventRequiredElementWheelCallbackPropagation();
        }

        private void SetReferences()
        {
            _foldoutElement = this.Q<Foldout>(FoldoutName);
            _mainSceneField = this.Q<ObjectField>(MainSceneName);
        }

        private void InitAndReferenceFoldoutTextField()
        {
            var headerContent = Resources.Load<VisualTreeAsset>(HeaderContentUxmlPath).CloneTree().ElementAt(0);
            this.Q<Toggle>().Q<VisualElement>().Add(headerContent);
            headerContent.AddStyleFromUss(HeaderContentUssPath);
            _foldoutTextField = this.Q<TextField>(FoldoutTextFieldName);
        }

        private void SetStyleEvents()
        {
            _foldoutElement.RegisterValueChangedCallback(evt =>
            {
                var borderTopWidth = evt.newValue ? 1 : 0;
                this.Q(ToggleBottomName).style.borderTopWidth = borderTopWidth;
            });
        }

        private void RegisterButtonsClickEventCallback()
        {
            var playButton = this.Q(PlayButtonName).Q<Button>();
            playButton.RegisterCallback<ClickEvent>(evt =>
            {
                if (evt.currentTarget == playButton)
                {
                    Debug.LogError("todo: play scene from here");
                }
            });

            var loadButton = this.Q(LoadButtonName).Q<Button>();
            loadButton.RegisterCallback<ClickEvent>(evt =>
            {
                if (evt.currentTarget == loadButton)
                {
                    Debug.LogError("todo: load scene from here");
                }
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


        private void InitRequiredScenesListView()
        {
            _listViewRequiredElements = this.Q<ListView>(RequiredScenesListViewName);
            _listViewRequiredElements.itemsSource = _data.RequiredScenes;
            _listViewRequiredElements.makeItem = AddElement;
            _listViewRequiredElements.bindItem = OnElementScrollIntoView;
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

        private RequiredSceneElement AddElement()
        {
            return new RequiredSceneElement();
        }

        private void OnElementScrollIntoView(VisualElement element, int index)
        {
            Debug.LogError($"count: {_data.RequiredScenes.Count} | at index {index}: {_data.RequiredScenes[index]}");
            var elementAsDataType = element as IBindableListViewElement<SceneData>;
            if (_data.RequiredScenes[index] == null)
            {
                Debug.LogError("is null");
                elementAsDataType?.InitDataValues(_data.RequiredScenes[index]);
                elementAsDataType?.BindElementToData(_data.RequiredScenes[index]);
                return;
            }

            elementAsDataType?.SetElementFromData(_data.RequiredScenes[index]);
        }
    }
}