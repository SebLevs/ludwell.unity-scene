using System;
using UnityEngine;
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
            AutomateTagValidityManagement();
        }

        private void AutomateTagValidityManagement()
        {
            _tagTextField.RegisterCallback<BlurEvent>(_ =>
            {
                if (String.IsNullOrEmpty(_tagTextField.value))
                {
                    _tagsManager.RemoveTag(this);
                }
                else if (_tagsManager.IsTagDuplicate(this, _tagTextField.value))
                {
                    Debug.LogError($"{_tagTextField.value} already exists, duplicate tag has been removed.");
                    _tagsManager.RemoveTag(this);
                }
            });
        }

        public void BindElementToCachedData()
        {
            _tagTextField.RegisterValueChangedCallback(BindTextField);
        }

        public void SetElementFromCachedData()
        {
            _tagTextField.value = Cache.Value;
        }

        public void FocusTextField()
        {
            _tagTextField.Focus();
        }

        private void SetReferences()
        {
            _addButton = this.Q<Button>(AddButtonName);
            _removeButton = this.Q<Button>(RemoveButtonName);

            _tagTextField = this.Q<TextField>(TagTextFieldName);

            RegisterCallback<AttachToPanelEvent>(_ =>
            {
                _tagsManager = GetFirstAncestorOfType<TagsManager>();
            });
        }

        private void InitializeButtons()
        {
            _addButton.RegisterCallback<ClickEvent>(_ => AddToController());
            _removeButton.RegisterCallback<ClickEvent>(_ => RemoveFromController());
        }

        private void AddToController()
        {
            _tagsManager.AddTagToController(_tagTextField.value);
        }

        private void RemoveFromController()
        {
            _tagsManager.RemoveTagFromController(_tagTextField.value);
        }

        private void BindTextField(ChangeEvent<string> evt)
        {
            Cache.Value = evt.newValue;
        }
    }
}