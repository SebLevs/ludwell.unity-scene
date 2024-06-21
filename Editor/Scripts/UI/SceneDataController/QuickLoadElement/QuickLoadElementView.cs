using System.IO;
using System.Linq;
using UnityEditor;
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

        public readonly QuickLoadElementController Controller;

        public void SetFoldoutValue(bool value) => _foldout.value = value;

        public void SetSceneData(SceneData sceneData) => _sceneDataTextField.value = sceneData.name;

        public void SetIconAssetOutsideAssets(bool state) =>
            _iconAssetOutsideAssets.style.display = state ? DisplayStyle.Flex : DisplayStyle.None;

        public void SetDirectoryChangeButtonEnable(bool state) => _directoryChangeButton.SetEnabled(state);

        public void SetOpenButtonEnable(bool state) => _openButton.SetEnabled(state);

        public void SetOpenAdditiveButtonEnable(bool state) => _openAdditiveButton.SetEnabled(state);

        public QuickLoadElementData Model => Controller.Model;

        public QuickLoadElementView()
        {
            this.AddHierarchyFromUxml(UxmlPath);
            this.AddStyleFromUss(UssPath);

            Controller = new QuickLoadElementController(this);

            InitializeFoldout();
            InitializeFoldoutTextField();

            InitializePingButton();
            InitializeDirectoryChangeButton();
            InitializeLoadButton();
            InitializeOpenButton();
            InitializeOpenAdditiveButton();

            _foldout.RegisterCallback<ClickEvent>(SessionStateCacheFoldoutValue);
        }

        private void InitializeFoldout()
        {
            _foldout = this.Q<Foldout>(FoldoutName);
            _foldout.RegisterValueChangedCallback(ToggleFoldoutStyle);
            _foldout.value = false;
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
                SetFoldoutValue(!_foldout.value);
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
            Controller.UpdateData(data);
        }

        public void BindElementToCachedData()
        {
            _sceneDataTextField.RegisterValueChangedCallback(UpdateAndSaveAssetName);
        }

        public void SetElementFromCachedData()
        {
            Controller.SetFoldoutValueFromSavedState();
            Controller.SetSceneData(this);
            Controller.SetIconAssetOutsideAssets(this);

            Controller.SetTooltipAsAssetPath(_directoryChangeButton);

            Controller.SetTagsContainer();

            Controller.SetDirectoryChangeButton();
            Controller.SetLoadButtonState();
            Controller.SolveOpenButton();
            Controller.SolveOpenAdditiveButton();
        }

        public void FocusTextField()
        {
            _sceneDataTextField.Blur();
            _sceneDataTextField.Focus();
            var textLength = _sceneDataTextField.text.Length;
            _sceneDataTextField.SelectRange(textLength, textLength);
        }

        private void UpdateAndSaveAssetName(ChangeEvent<string> evt)
        {
            Controller.UpdateAndSaveAssetName(evt.newValue);
        }

        private void InitializePingButton()
        {
            var button = this.Q<ButtonWithIcon>(PingButtonName);
            Controller.InitializePingButton(button);
        }

        private void InitializeDirectoryChangeButton()
        {
            _directoryChangeButton = this.Q<ButtonWithIcon>(DirectoryChangeButtonName);
            Controller.InitializeDirectoryChangeButton(_directoryChangeButton);
        }

        private void InitializeLoadButton()
        {
            var button = this.Q<DualStateButton>(LoadSceneButtonName);
            Controller.InitializeLoadButton(button);
        }

        private void InitializeOpenButton()
        {
            _openButton = this.Q<ButtonWithIcon>(OpenSceneButtonName);
            Controller.InitializeOpenButton(_openButton);
        }

        private void InitializeOpenAdditiveButton()
        {
            _openAdditiveButton = this.Q<DualStateButton>(OpenSceneAdditiveButtonName);
            Controller.InitializeOpenAdditiveButton(_openAdditiveButton);
        }

        private void SessionStateCacheFoldoutValue(ClickEvent evt)
        {
            var id = Controller.Model.SceneData.GetInstanceID().ToString();
            SessionState.SetBool(id, _foldout.value);
        }
    }
}