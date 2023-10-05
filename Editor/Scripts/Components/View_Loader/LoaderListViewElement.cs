using System.Collections.Generic;
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
            InitRequiredScenesListView();
            InitFoldoutTextField();
            PreventFoldoutToggleFromKeyPress();
            RegisterButtonsClickEventCallback();
        }

        private const string UxmlPath = "Uxml/scene-loader-element";
        private const string UssPath = "Uss/scene-loader-element";
        private const string HeaderContentUxmlPath = "Uxml/scene-loader-element__header-content";
        private const string HeaderContentUssPath = "Uss/scene-loader-element__header-content";

        private const string FoldoutName = "root__foldout";
        private const string FoldoutTextFieldName = "foldout-text-field";
        private const string MainSceneName = "main-scene";
        private const string RequiredScenesListViewName = "required-scenes";
        private const string PlayButtonName = "button__play";
        private const string LoadButtonName = "button__load";

        public List<RequiredSceneElement> RequiredSceneElements { get; private set; } = new();
        
        private Foldout _foldoutElement;
        private VisualElement _headerContent;
        private TextField _foldoutTextField;

        private ListView _listViewRequiredElements;

        public void BindElementToData(LoaderListViewElementData data)
        {
            _foldoutTextField.RegisterValueChangedCallback((evt) =>
            {
                data.Name = evt.newValue;
            });
            
            this.Q<ObjectField>(MainSceneName).RegisterValueChangedCallback((evt) =>
            {
                data.MainScene = evt.newValue as SceneData;
            });

            _listViewRequiredElements.RegisterCallback<AttachToPanelEvent>((evt) =>
            {
                Debug.LogError("attached to panel event");
            });
        }

        public void SetElementFromData(LoaderListViewElementData data)
        {
            this.Q<TextField>(FoldoutTextFieldName).value = data.Name;
            _foldoutElement.value = data.IsOpen;

            Debug.LogError(
                "todo: create data holding class for both this and RequiredSceneElement & replace LoaderSceneData element with it");
            foreach (var requiredScene in data.RequiredScenes)
            {
                // todo: add required scene elements & bind them
            }
        }

        private void SetReferences()
        {
            _foldoutElement = this.Q<Foldout>(FoldoutName);
            _foldoutTextField = this.Q<TextField>(FoldoutTextFieldName);
        }

        private void InitRequiredScenesListView()
        {
            _listViewRequiredElements = this.Q<ListView>(RequiredScenesListViewName);
            _listViewRequiredElements.itemsSource = RequiredSceneElements;
            _listViewRequiredElements.makeItem = AddElement;
            _listViewRequiredElements.bindItem = OnElementScrollIntoView;

            foreach (var element in RequiredSceneElements)
            {
                Debug.LogError("DELETE WHEN COMPLETE | " + element);
            }
        }

        private void InitFoldoutTextField()
        {
            _headerContent = Resources.Load<VisualTreeAsset>(HeaderContentUxmlPath).CloneTree().ElementAt(0);
            this.Q<Toggle>().Q<VisualElement>().Add(_headerContent);
            _headerContent.AddStyleFromUss(HeaderContentUssPath);
        }

        private void RegisterButtonsClickEventCallback()
        {
            var playButton = this.Q(PlayButtonName).Q<Button>();
            playButton.RegisterCallback<ClickEvent>((evt) =>
            {
                if (evt.currentTarget == playButton)
                {
                    Debug.LogError("todo: play scene from here");
                }
            });

            var loadButton = this.Q(LoadButtonName).Q<Button>();
            loadButton.RegisterCallback<ClickEvent>((evt) =>
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
            foldoutTextField.RegisterCallback<KeyDownEvent>((evt) =>
            {
                evt.StopPropagation();
                if (evt.currentTarget == foldoutTextField)
                {
                    if (evt.keyCode == KeyCode.Return || evt.keyCode == KeyCode.Space)
                    {
                        _foldoutElement.value = !_foldoutElement.value;
                    }
                }
            });

            foldoutTextField.RegisterCallback<ClickEvent>((evt) => { evt.StopPropagation(); });
        }

        private RequiredSceneElement AddElement()
        {
            return new RequiredSceneElement();
        }

        private void OnElementScrollIntoView(VisualElement element, int index)
        {
            if (RequiredSceneElements[index] == null)
            {
                RequiredSceneElements[index] = new RequiredSceneElement();
            }

            (element as IBindableListViewElement<RequiredSceneElement>)?.SetElementFromData(
                RequiredSceneElements[index]);
        }
    }
}