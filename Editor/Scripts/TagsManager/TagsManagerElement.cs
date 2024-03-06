using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ludwell.Scene.Editor
{
    public class TagsManagerElement : VisualElement, IListViewVisualElement<TagWithSubscribers>
    {
        public new class UxmlFactory : UxmlFactory<TagsManagerElement, UxmlTraits>
        {
        }

        private static readonly string UxmlPath = Path.Combine("Uxml", nameof(TagsManager), nameof(TagsManagerElement));
        private static readonly string UssPath = Path.Combine("Uss", nameof(TagsManager), nameof(TagsManagerElement));

        private const string AddButtonName = "button__add";
        private const string RemoveButtonName = "button__remove";

        private const string TagTextFieldName = "tag";

        private Button _addButton;
        private Button _removeButton;

        private TextField _tagTextField;

        private TagsManager _tagsManager;

        private TagWithSubscribers _cache;

        public TagsManagerElement()
        {
            this.AddHierarchyFromUxml(UxmlPath);
            this.AddStyleFromUss(UssPath);

            SetReferences();
            InitializeButtons();
            InitializeValidityHandlingEvents();
            InitializeDelayedSaveEvent();
        }

        public void CacheData(TagWithSubscribers data)
        {
            _cache = data;
        }

        public void BindElementToCachedData()
        {
            _tagTextField.RegisterValueChangedCallback(BindTextField);
        }

        public void SetElementFromCachedData()
        {
            _tagTextField.value = _cache.Name;
            if (!string.IsNullOrEmpty(_cache.Name)) return;
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
            _tagsManager.AddTagToController(_cache);
        }

        private void RemoveFromController()
        {
            _tagsManager.RemoveTagFromController(_cache);
        }

        private void InitializeValidityHandlingEvents()
        {
            _tagTextField.RegisterCallback<BlurEvent>(_ => HandleInvalidTag());
            RegisterCallback<AttachToPanelEvent>(_ => _tagTextField.RegisterCallback<KeyDownEvent>(OnKeyDown));
        }

        private void InitializeDelayedSaveEvent()
        {
            _tagTextField.RegisterValueChangedCallback(_ => DataFetcher.SaveEveryScriptableDelayed());
        }

        private void OnKeyDown(KeyDownEvent evt)
        {
            if (evt.keyCode != KeyCode.Return) return;
            _tagsManager.SortTags();
            _tagsManager.SetPreviousTarget(null);
        }

        public void HandleInvalidTag()
        {
            if (!string.IsNullOrEmpty(_cache.Name) && !_tagsManager.IsTagDuplicate(_cache)) return;
            _tagsManager.RemoveInvalidTagElement(_cache);
            _cache.RemoveFromAllSubscribers();
        }

        private void BindTextField(ChangeEvent<string> evt)
        {
            _cache.Name = evt.newValue;
        }
    }
}