using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ludwell.Scene.Editor
{
    public class QuickLoadElementView : VisualElement, IListViewVisualElement<QuickLoadElementData>
    {
        private static readonly string UxmlPath =
            Path.Combine("UI", nameof(QuickLoadElementView), "Uxml_" + nameof(QuickLoadElementView));

        private static readonly string UssPath =
            Path.Combine("UI", nameof(QuickLoadElementView), "Uss_" + nameof(QuickLoadElementView));

        private static readonly string HeaderContentUxmlPath =
            Path.Combine("UI", nameof(QuickLoadElementView), "Uxml_" + "quick-load-element__header-content");

        private static readonly string HeaderContentUssPath =
            Path.Combine("UI", nameof(QuickLoadElementView), "Uss_" + "quick-load-element__header-content");

        private const string FoldoutName = "root__foldout";
        private const string ToggleBottomName = "toggle-bottom";
        private const string SceneDataName = "scene-data";
        private const string LoadSceneButtonName = "button__load";
        private const string OpenSceneButtonName = "button__open";
        private const string IconAssetOutsideAssetsName = "icon__package-scene";
        private const string DataPresetLabelName = "section-selection";

        private readonly Foldout _foldout;
        private readonly QuickLoadElementController _controller;
        
        private Button _sceneDataName;
        private VisualElement _iconAssetOutsideAssets;

        private Label _dataPresetLabel;

        public void SetIsOpen(bool value) => _foldout.value = value;

        public void SetSceneData(SceneData sceneData) => _sceneDataName.text = sceneData.name;

        public void SetIconAssetOutsideAssets(bool state) =>
            _iconAssetOutsideAssets.style.display = state ? DisplayStyle.Flex : DisplayStyle.None;

        public void SetSelectedDataPreset(string value) => _dataPresetLabel.text = value;

        public QuickLoadElementView()
        {
            this.AddHierarchyFromUxml(UxmlPath);
            this.AddStyleFromUss(UssPath);

            _controller = new QuickLoadElementController(this);

            _foldout = this.Q<Foldout>(FoldoutName);

            _dataPresetLabel = this.Q<Label>(DataPresetLabelName);

            InitializeAndReferenceFoldoutTextField();
            RegisterStyleEvents();

            InitializeLoadButton();
            InitializeOpenButton();
        }

        private void InitializeAndReferenceFoldoutTextField()
        {
            var headerContent = Resources.Load<VisualTreeAsset>(HeaderContentUxmlPath).CloneTree().ElementAt(0);
            headerContent.AddStyleFromUss(HeaderContentUssPath);
            this.Q<Toggle>().Children().First().Add(headerContent);
            _sceneDataName = this.Q<Button>(SceneDataName);
            _iconAssetOutsideAssets = this.Q<VisualElement>(IconAssetOutsideAssetsName);
        }

        public void CacheData(QuickLoadElementData data)
        {
            _controller.UpdateData(data);
        }

        public void BindElementToCachedData()
        {
            _foldout.RegisterValueChangedCallback(_controller.UpdateIsOpen);
            _sceneDataName.RegisterValueChangedCallback(_controller.UpdateName);
            _sceneDataName.clicked += _controller.SelectSceneDataInProject;
        }

        public void SetElementFromCachedData()
        {
            _controller.SetIsOpen(this);
            _controller.SetSceneData(this);
            _controller.SetIconAssetOutsideAssets(this);
            _controller.SetSelectedDataPreset(this);

            _controller.UpdateTagsContainer();
        }

        private void InitializeLoadButton()
        {
            var loadSceneButton = this.Q<DualStateButton>(LoadSceneButtonName);
            _controller.InitializeLoadButton(loadSceneButton);
        }

        private void InitializeOpenButton()
        {
            var openSceneButton = this.Q<ButtonWithIcon>(OpenSceneButtonName);
            _controller.InitializeOpenButton(openSceneButton);
        }

        private void RegisterStyleEvents()
        {
            _foldout.RegisterValueChangedCallback(evt =>
            {
                var borderTopWidth = evt.newValue ? 1 : 0;
                this.Q(ToggleBottomName).style.borderTopWidth = borderTopWidth;
            });
        }
    }
}