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
        private const string PingButtonName = "button__ping";
        private const string DirectoryChangeButtonName = "button__directory-path";
        private const string LoadSceneButtonName = "button__load";
        private const string OpenSceneButtonName = "button__open";
        private const string OpenSceneAdditiveButtonName = "button__open-additive";
        private const string IconAssetOutsideAssetsName = "icon__package-scene";

        private const string TextFieldUnselectedClass = "scene-data__unselected";
        private const string TextFieldSelectedClass = "scene-data__selected";

        private Foldout _foldout;
        private TextField _sceneDataTextField;
        private VisualElement _iconAssetOutsideAssets;

        private ButtonWithIcon _directoryChangeButton;
        private ButtonWithIcon _openButton;
        private DualStateButton _openAdditiveButton;

        private readonly QuickLoadElementController _controller;

        public void SetIsOpen(bool value) => _foldout.value = value;

        public void SetSceneData(SceneData sceneData) => _sceneDataTextField.value = sceneData.name;

        public void SetIconAssetOutsideAssets(bool state) =>
            _iconAssetOutsideAssets.style.display = state ? DisplayStyle.Flex : DisplayStyle.None;
        
        public void SetDirectoryChangeButtonEnable(bool state) => _directoryChangeButton.SetEnabled(state);
        
        public void SetOpenButtonEnable(bool state) => _openButton.SetEnabled(state);
        
        public void SetOpenAdditiveButtonEnable(bool state) => _openAdditiveButton.SetEnabled(state);

        public QuickLoadElementView()
        {
            this.AddHierarchyFromUxml(UxmlPath);
            this.AddStyleFromUss(UssPath);

            _controller = new QuickLoadElementController(this);

            InitializeFoldout();
            InitializeFoldoutTextField();

            InitializePingButton();
            InitializeDirectoryChangeButton();
            InitializeLoadButton();
            InitializeOpenButton();
            InitializeOpenAdditiveButton();
        }

        private void InitializeFoldout()
        {
            _foldout = this.Q<Foldout>(FoldoutName);
            _foldout.RegisterValueChangedCallback(ToggleFoldoutStyle);
        }

        private void ToggleFoldoutStyle(ChangeEvent<bool> evt)
        {
            var borderTopWidth = evt.newValue ? 1 : 0;
            this.Q(ToggleBottomName).style.borderTopWidth = borderTopWidth;
        }

        private void InitializeFoldoutTextField()
        {
            var headerContent = Resources.Load<VisualTreeAsset>(HeaderContentUxmlPath).CloneTree().ElementAt(0);
            headerContent.AddStyleFromUss(HeaderContentUssPath);
            this.Q<Toggle>().Children().First().Add(headerContent);
            _sceneDataTextField = this.Q<TextField>(SceneDataName);
            _iconAssetOutsideAssets = this.Q<VisualElement>(IconAssetOutsideAssetsName);

            _sceneDataTextField.RegisterCallback<KeyDownEvent>(evt =>
            {
                // todo: hack fix. Investigate better solution
                if (evt.keyCode != KeyCode.Return && evt.keyCode != KeyCode.Space) return;
                SetIsOpen(!_foldout.value);
            });

            _sceneDataTextField.RegisterCallback<FocusEvent>(evt =>
            {
                _sceneDataTextField.RemoveFromClassList(TextFieldUnselectedClass);
                _sceneDataTextField.AddToClassList(TextFieldSelectedClass);
            });

            _sceneDataTextField.RegisterCallback<BlurEvent>(evt =>
            {
                _sceneDataTextField.RemoveFromClassList(TextFieldSelectedClass);
                _sceneDataTextField.AddToClassList(TextFieldUnselectedClass);
            });
        }

        public void CacheData(QuickLoadElementData data)
        {
            _controller.UpdateData(data);
        }

        public void BindElementToCachedData()
        {
            _foldout.RegisterValueChangedCallback(_controller.UpdateIsOpen);
            _sceneDataTextField.RegisterValueChangedCallback(UpdateAndSaveAssetName);
        }

        public void SetElementFromCachedData()
        {
            _controller.SetIsOpen(this);
            _controller.SetSceneData(this);
            _controller.SetIconAssetOutsideAssets(this);

            _controller.SetTooltipAsAssetPath(_directoryChangeButton);

            _controller.SetTagsContainer();

            _controller.SetDirectoryChangeButton();
            _controller.SetLoadButtonState();
            _controller.SolveOpenButton();
            _controller.SolveOpenAdditiveButton();
        }

        private void InitializePingButton()
        {
            var button = this.Q<ButtonWithIcon>(PingButtonName);
            _controller.InitializePingButton(button);
        }

        private void InitializeDirectoryChangeButton()
        {
            _directoryChangeButton = this.Q<ButtonWithIcon>(DirectoryChangeButtonName);
            _controller.InitializeDirectoryChangeButton(_directoryChangeButton);
        }

        private void InitializeLoadButton()
        {
            var button = this.Q<DualStateButton>(LoadSceneButtonName);
            _controller.InitializeLoadButton(button);
        }

        private void InitializeOpenButton()
        {
            _openButton = this.Q<ButtonWithIcon>(OpenSceneButtonName);
            _controller.InitializeOpenButton(_openButton);
        }

        private void InitializeOpenAdditiveButton()
        {
            _openAdditiveButton = this.Q<DualStateButton>(OpenSceneAdditiveButtonName);
            _controller.InitializeOpenAdditiveButton(_openAdditiveButton);
        }

        private void UpdateAndSaveAssetName(ChangeEvent<string> evt)
        {
            _controller.UpdateAndSaveAssetName(evt.newValue);
        }

        public void FocusTextField()
        {
            _sceneDataTextField.Blur();
            _sceneDataTextField.Focus();
            var textLength = _sceneDataTextField.text.Length;
            _sceneDataTextField.SelectRange(textLength, textLength);
        }
    }
}