using System.Collections.Generic;
using System.Linq;
using Ludwell.Architecture;
using Ludwell.UIToolkitElements.Editor;
using Ludwell.UIToolkitUtilities;
using Ludwell.UIToolkitUtilities.Editor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ludwell.Scene.Editor
{
    public class TagsManagerViewArgs : ViewArgs
    {
        public TagsManagerViewArgs(TagSubscriberWithTags tagSubscriberWithTags)
        {
            TagSubscriberWithTags = tagSubscriberWithTags;
        }

        public TagSubscriberWithTags TagSubscriberWithTags { get; }
    }

    public class TagsManagerController : AViewable
    {
        private const string TagElementsContainerName = "tag-elements-container";

        private readonly VisualElement _root;
        private readonly TagsManagerView _view;
        private readonly TagsShelfController _tagsShelfController;

        private readonly TagContainer _tagContainer;

        private ListViewHandler<TagsManagerElementController, Tag> _listViewHandler;

        public TagsManagerController(VisualElement parent) : base(parent)
        {
            Services.Add<TagsManagerController>(this);

            _root = parent.Q(nameof(TagsManagerView));
            _view = new TagsManagerView(_root);

            _tagsShelfController = new TagsShelfController(_root, ReturnToPreviousView);

            _tagContainer = ResourcesLocator.GetTagContainer();

            SetViewReturnIconTooltip();

            _root.Root().RegisterCallback<KeyUpEvent>(OnKeyUpReturn);
            InitializeListViewHandler();
            InitializeDropdownSearchField();
        }

        public void Dispose()
        {
            _tagsShelfController.Dispose();

            _root.Root().UnregisterCallback<KeyUpEvent>(OnKeyUpReturn);

            _listViewHandler.OnItemMade -= OnItemMadeRegisterEvents;
            _listViewHandler.ListView.itemsRemoved -= HandleItemsRemoved;

            var listView = _listViewHandler.ListView;

            listView.RegisterCallback<KeyUpEvent>(OnKeyUpDeleteSelected);
            listView.RegisterCallback<KeyUpEvent>(OnKeyUpAddSelected);
            listView.RegisterCallback<KeyUpEvent>(OnKeyUpRemoveSelected);
        }

        public void ScrollToItemIndex(int index)
        {
            var focusController = _root.focusController;
            var focusedElement = focusController?.focusedElement;
            focusedElement?.Blur();

            _listViewHandler.ListView.ScrollToItem(index);
            _listViewHandler.ListView.SetSelection(index);
            _listViewHandler.GetVisualElementAt(index)?.FocusTextField();
        }

        protected override void Show(ViewArgs args)
        {
            Signals.Add<UISignals.RefreshView>(_tagsShelfController.Populate);
            Signals.Add<UISignals.RefreshView>(_listViewHandler.ForceRebuild);
            var tagsManagerViewArgs = (TagsManagerViewArgs)args;
            _view.Show();
            _view.SetReferenceText(tagsManagerViewArgs.TagSubscriberWithTags.ID);
            BuildTagsController(tagsManagerViewArgs.TagSubscriberWithTags);
        }

        protected override void Hide()
        {
            Signals.Remove<UISignals.RefreshView>(_tagsShelfController.Populate);
            Signals.Remove<UISignals.RefreshView>(_listViewHandler.ForceRebuild);
            _view.Hide();
        }

        private void BuildTagsController(TagSubscriberWithTags tagSubscriberWithTags)
        {
            _tagsShelfController.UpdateData(tagSubscriberWithTags);
            _tagsShelfController.Populate();
        }

        private void AddTagToShelf(Tag tag)
        {
            _tagsShelfController.Add(tag);
        }

        private void RemoveTagFromShelf(Tag tag)
        {
            _tagsShelfController.Remove(tag);
        }

        private void SetViewReturnIconTooltip()
        {
            _tagsShelfController.OverrideIconTooltip("Return");
        }

        private void OnKeyUpReturn(KeyUpEvent evt)
        {
            if (_root.style.display == DisplayStyle.None) return;
            if (evt.keyCode == KeyCode.Escape && (evt.modifiers & EventModifiers.Control) != 0) ReturnToPreviousView();
        }

        private void InitializeListViewHandler()
        {
            _listViewHandler =
                new ListViewHandler<TagsManagerElementController, Tag>(
                    _root.Q<ListView>(TagElementsContainerName),
                    ResourcesLocator.GetTagContainer().Tags);

            _listViewHandler.OnItemMade += OnItemMadeRegisterEvents;
            _listViewHandler.ListView.itemsRemoved += HandleItemsRemoved;

            var listView = _listViewHandler.ListView;

            listView.RegisterCallback<KeyUpEvent>(OnKeyUpDeleteSelected);
            listView.RegisterCallback<KeyUpEvent>(OnKeyUpAddSelected);
            listView.RegisterCallback<KeyUpEvent>(OnKeyUpRemoveSelected);
        }

        private void HandleItemsRemoved(IEnumerable<int> enumerable)
        {
            var quickLoadElementData = ResourcesLocator.GetQuickLoadElements().Elements;

            var itemsSource = _listViewHandler.ListView.itemsSource;
            var removedIndexes = enumerable.ToList();

            foreach (var index in removedIndexes)
            {
                var tag = (Tag)itemsSource[index];
                _tagsShelfController.Remove(tag);
                foreach (var element in quickLoadElementData)
                {
                    if (!element.Tags.Contains(tag)) continue;
                    element.Tags.Remove(tag);
                    ResourcesLocator.SaveQuickLoadElementsDelayed();
                }
            }

            ResourcesLocator.SaveTagContainer();
        }

        private void OnKeyUpDeleteSelected(KeyUpEvent keyUpEvent)
        {
            if (!((keyUpEvent.ctrlKey || keyUpEvent.commandKey) && keyUpEvent.keyCode == KeyCode.Delete)) return;

            var arrayOfElements = _listViewHandler.GetSelectedData().ToArray();
            if (!arrayOfElements.Any()) return;

            for (var i = arrayOfElements.Length - 1; i >= 0; i--)
            {
                RemoveTagFromShelf(arrayOfElements[i]);
                _listViewHandler.RemoveSelectedElement();
            }
        }

        private void OnKeyUpAddSelected(KeyUpEvent keyUpEvent)
        {
            if (!((keyUpEvent.ctrlKey || keyUpEvent.commandKey) && keyUpEvent.keyCode == KeyCode.Return)) return;

            var arrayOfElements = _listViewHandler.GetSelectedData().ToArray();
            if (!arrayOfElements.Any()) return;

            foreach (var tagWithSubscribers in arrayOfElements)
            {
                AddTagToShelf(tagWithSubscribers);
            }
        }

        private void OnKeyUpRemoveSelected(KeyUpEvent keyUpEvent)
        {
            var arrayOfElements = _listViewHandler.GetSelectedData().ToArray();
            if (!arrayOfElements.Any()) return;
            if (!((keyUpEvent.ctrlKey || keyUpEvent.commandKey) && keyUpEvent.keyCode == KeyCode.Backspace)) return;

            for (var i = arrayOfElements.Length - 1; i >= 0; i--)
            {
                RemoveTagFromShelf(arrayOfElements[i]);
            }
        }

        private void OnItemMadeRegisterEvents(TagsManagerElementController controller)
        {
            controller.OnAddToShelf += AddTagToShelf;
            controller.OnRemoveFromShelf += RemoveTagFromShelf;
        }

        private void InitializeDropdownSearchField()
        {
            var dropDropdownSearchField = _root.Q<DropdownSearchField>();
            dropDropdownSearchField.BindToListView(_listViewHandler.ListView);
            dropDropdownSearchField.WithDropdownBehaviour(itemIndex =>
            {
                _listViewHandler.ListView.ScrollToItem(itemIndex);
            });
        }
    }
}