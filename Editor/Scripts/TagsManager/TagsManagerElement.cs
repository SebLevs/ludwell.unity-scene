using UnityEngine;
using UnityEngine.UIElements;

namespace Ludwell.Scene.Editor
{
    public class TagsManagerElement : VisualElement, IBindableListViewElement<TagWithSubscribers>
    {
        public new class UxmlFactory : UxmlFactory<TagsManagerElement, UxmlTraits>
        {
        }

        private const string UxmlPath = "Uxml/" + nameof(TagsManager) + "/" + nameof(TagsManagerElement);
        private const string UssPath = "Uss/" + nameof(TagsManager) + "/" + nameof(TagsManagerElement);

        private const string AddButtonName = "button__add";
        private const string RemoveButtonName = "button__remove";

        private const string TagTextFieldName = "tag";

        private Button _addButton;
        private Button _removeButton;

        private TextField _tagTextField;

        private TagsManager _tagsManager;

        public TagWithSubscribers Cache { get; set; }

        public TagsManagerElement()
        {
            this.AddHierarchyFromUxml(UxmlPath);
            this.AddStyleFromUss(UssPath);

            SetReferences();
            InitializeButtons();
            InitializeValidityHandlingEvents();
        }

        public void BindElementToCachedData()
        {
            _tagTextField.RegisterValueChangedCallback(BindTextField);
        }

        public void SetElementFromCachedData()
        {
            _tagTextField.value = Cache.Name;
            if (!string.IsNullOrEmpty(Cache.Name)) return;
            _tagTextField.Focus();
            _tagsManager.SetPreviousTarget(this);
        }

        private void SetReferences()
        {
            _addButton = this.Q<Button>(AddButtonName);
            _removeButton = this.Q<Button>(RemoveButtonName);

            _tagTextField = this.Q<TextField>(TagTextFieldName);

            RegisterCallback<AttachToPanelEvent>(_ => { _tagsManager = GetFirstAncestorOfType<TagsManager>(); });
        }

        private void InitializeButtons()
        {
            _addButton.RegisterCallback<ClickEvent>(_ => AddToController());
            _removeButton.RegisterCallback<ClickEvent>(_ => RemoveFromController());
        }

        private void AddToController()
        {
            _tagsManager.AddTagToController(Cache);
        }

        private void RemoveFromController()
        {
            _tagsManager.RemoveTagFromController(Cache);
        }

        private void InitializeValidityHandlingEvents()
        {
            _tagTextField.RegisterCallback<BlurEvent>(_ => HandleInvalidTag());
            RegisterCallback<AttachToPanelEvent>(_ => _tagTextField.RegisterCallback<KeyDownEvent>(OnKeyDown));
        }

        private void OnKeyDown(KeyDownEvent evt)
        {
            if (evt.keyCode != KeyCode.Return) return;
            _tagsManager.SortTags();
            _tagsManager.SetPreviousTarget(null);
        }

        public void HandleInvalidTag()
        {
            Debug.LogError(Cache);
            Debug.LogError(Cache.Name);
            if (!string.IsNullOrEmpty(Cache.Name) && !_tagsManager.IsTagDuplicate(Cache)) return;
            _tagsManager.RemoveInvalidTagElement(Cache);
            Cache.RemoveFromAllSubscribers();
        }

        private void BindTextField(ChangeEvent<string> evt)
        {
            Cache.Name = evt.newValue;
        }
    }
}