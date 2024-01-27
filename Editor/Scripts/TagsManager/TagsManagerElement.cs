using UnityEngine.UIElements;

namespace Ludwell.Scene.Editor
{
    public class TagsManagerElement : VisualElement, IBindableListViewElement<Tag>
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

        public Tag Cache { get; set; }

        public TagsManagerElement()
        {
            this.AddHierarchyFromUxml(UxmlPath);
            this.AddStyleFromUss(UssPath);

            SetReferences();
            InitializeButtons();
        }

        public void BindElementToCachedData()
        {
            _tagTextField.RegisterValueChangedCallback(BindTextField);
        }

        public void SetElementFromCachedData()
        {
            _tagTextField.value = Cache.Value;
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
            _addButton.RegisterCallback<ClickEvent>(_ => Add());
            _removeButton.RegisterCallback<ClickEvent>(_ => Remove());
        }

        private void Add()
        {
            _tagsManager.AddTag(_tagTextField.value);
        }

        private void Remove()
        {
            _tagsManager.RemoveTag(_tagTextField.value);
        }

        private void BindTextField(ChangeEvent<string> evt)
        {
            Cache.Value = evt.newValue;
        }
    }
}