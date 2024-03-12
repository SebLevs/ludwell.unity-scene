using System;
using System.IO;
using System.Linq;
using Ludwell.Scene.Editor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ludwell.Scene
{
    public class TagsManagerView : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<TagsManagerView, UxmlTraits>
        {
        }

        private static readonly string
            UxmlPath = Path.Combine("Uxml", nameof(TagsManagerView), nameof(TagsManagerView));

        private static readonly string UssPath = Path.Combine("Uss", nameof(TagsManagerView), nameof(TagsManagerView));

        private static readonly string TagContainerPath = Path.Combine("Scriptables", nameof(TagContainer));

        private const string ReferenceName = "reference-name";
        private Label _referenceName;


        private TagContainer _tagContainer;

        private ListViewHandler<TagsManagerElementView, TagWithSubscribers> _listViewHandler;
        private DropdownSearchField _dropdownSearchField;

        private TagsManagerElementView _previousTarget;

        private TagsManagerPresentor _presentor;

        public TagsManagerView()
        {
            this.AddHierarchyFromUxml(UxmlPath);
            this.AddStyleFromUss(UssPath);

            SetReferences();

            InitializeListViewBehaviours();
            InitializeDropdownSearchField();
            InitializeListViewKeyUpEVents();
            // todo: work around for Focus/Blur overlap issue with ListView Rebuild. Sort logic should be on TME blur.
            InitializeTagSorting();

            HandleTagController();
        }

        public void ShowDelegated(TagSubscriberWithTags tagSubscriber, VisualElement previousView)
        {
            _presentor.Show(tagSubscriber, previousView);
        }

        public void Show()
        {
            style.display = DisplayStyle.Flex;
        }

        public void SetReferenceText(string value)
        {
            _referenceName.text = value;
        }

        public void AddTagToShelfDelegated(TagWithSubscribers tag)
        {
            _presentor.AddTagToShelf(tag);
            _presentor.AddSubscriberToTag(tag);
        }

        public void RemoveTagFromShelfDelegated(TagWithSubscribers tag)
        {
            _presentor.RemoveTagFromShelf(tag);
            _presentor.RemoveSubscriberFromTag(tag);
        }

        public void RemoveInvalidTagElement(TagWithSubscribers tag)
        {
            _presentor.RemoveTagFromShelf(tag);
            _tagContainer.Tags.Remove(tag);
            DataFetcher.SaveEveryScriptable();
            _listViewHandler.ForceRebuild();
        }

        public void SetPreviousTarget(TagsManagerElementView target)
        {
            _previousTarget = target;
        }

        public bool IsTagDuplicate(Tag elementTag)
        {
            foreach (var tag in _tagContainer.Tags)
            {
                if (tag == elementTag) continue;
                if (string.Equals(tag.Name, elementTag.Name, StringComparison.CurrentCultureIgnoreCase)) return true;
            }

            return false;
        }

        public void SortTags()
        {
            _tagContainer.Tags.Sort();
            DataFetcher.SaveEveryScriptableDelayed();
            _listViewHandler.ForceRebuild();
        }
        
        private void SetReferences()
        {
            _referenceName = this.Q<Label>(ReferenceName);

            _tagContainer = Resources.Load<TagContainer>(TagContainerPath);
            _listViewHandler = new(this.Q<ListView>(), _tagContainer.Tags);
            _dropdownSearchField = this.Q<DropdownSearchField>();

            _presentor = new TagsManagerPresentor(this);
        }

        private void InitializeListViewBehaviours()
        {
            _listViewHandler.ListView.itemsRemoved += indexEnumerable =>
            {
                var itemsSource = _listViewHandler.ListView.itemsSource;
                var removedIndexes = indexEnumerable.ToList();
                foreach (var index in removedIndexes)
                {
                    var tag = itemsSource[index] as TagWithSubscribers;
                    _presentor.RemoveTagFromAllSubscribers(tag);
                }

                DataFetcher.SaveEveryScriptable();
            };
        }

        private void InitializeDropdownSearchField()
        {
            _dropdownSearchField.BindToListView(_listViewHandler.ListView);
            _dropdownSearchField.WithDropdownBehaviour(itemIndex =>
            {
                _listViewHandler.ListView.ScrollToItem(itemIndex);
            });
        }

        private void InitializeListViewKeyUpEVents()
        {
            RegisterCallback<AttachToPanelEvent>(_ => this.Root().RegisterCallback<KeyUpEvent>(OnKeyUpReturn));

            _listViewHandler.ListView.RegisterCallback<KeyUpEvent>(OnKeyUpDeleteSelected);
            _listViewHandler.ListView.RegisterCallback<KeyUpEvent>(OnKeyUpAddSelected);
            _listViewHandler.ListView.RegisterCallback<KeyUpEvent>(OnKeyUpRemoveSelected);
        }

        private void OnKeyUpReturn(KeyUpEvent evt)
        {
            if (style.display == DisplayStyle.None) return;

            if (evt.keyCode == KeyCode.Escape)
            {
                _presentor.ReturnToPreviousView();
            }
        }

        private void OnKeyUpDeleteSelected(KeyUpEvent keyUpEvent)
        {
            if (_listViewHandler.ListView.selectedItem == null) return;
            if (!((keyUpEvent.ctrlKey || keyUpEvent.commandKey) && keyUpEvent.keyCode == KeyCode.Delete)) return;

            var data = _listViewHandler.GetSelectedElementData();
            _presentor.RemoveTagFromShelf(data);
            data.RemoveFromAllSubscribers();
            _listViewHandler.RemoveSelectedElement();
        }

        private void OnKeyUpAddSelected(KeyUpEvent keyUpEvent)
        {
            if (_listViewHandler.ListView.selectedItem == null) return;
            if (!((keyUpEvent.ctrlKey || keyUpEvent.commandKey) && keyUpEvent.keyCode == KeyCode.Return)) return;

            var data = _listViewHandler.GetSelectedElementData();
            _presentor.AddTagToShelf(data);
        }

        private void OnKeyUpRemoveSelected(KeyUpEvent keyUpEvent)
        {
            if (_listViewHandler.ListView.selectedItem == null) return;
            if (!((keyUpEvent.ctrlKey || keyUpEvent.commandKey) && keyUpEvent.keyCode == KeyCode.Backspace)) return;

            var data = _listViewHandler.GetSelectedElementData();
            _presentor.RemoveTagFromShelf(data);
        }

        private void InitializeTagSorting()
        {
            RegisterCallback<MouseUpEvent>(evt =>
            {
                var tagsManagerElement = (evt.target as VisualElement).GetFirstAncestorOfType<TagsManagerElementView>();
                if (_previousTarget != null && _previousTarget != tagsManagerElement)
                {
                    SortTags();
                    _previousTarget.HandleInvalidTag();
                    _previousTarget = null;
                }

                if (evt.target is not TextElement) return;
                if (tagsManagerElement == null) return;
                _previousTarget = tagsManagerElement;
            });
        }

        private void HandleTagController()
        {
            _presentor.HandleTagController();
        }
    }
}