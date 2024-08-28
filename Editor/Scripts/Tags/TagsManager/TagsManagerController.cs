using System.Collections.Generic;
using System.Linq;
using Ludwell.Architecture;
using Ludwell.UIToolkitElements.Editor;
using Ludwell.UIToolkitUtilities;
using Ludwell.UIToolkitUtilities.Editor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ludwell.SceneManagerToolkit.Editor
{
    internal class TagsManagerViewArgs : ViewArgs
    {
        public TagsManagerViewArgs(TagSubscriberWithTags tagSubscriberWithTags)
        {
            TagSubscriberWithTags = tagSubscriberWithTags;
        }

        public TagSubscriberWithTags TagSubscriberWithTags { get; }
    }

    internal class TagsManagerController : AViewable
    {
        private const string TagElementsContainerName = "tag-elements-container";

        private readonly VisualElement _root;
        private readonly TagsManagerView _view;
        private readonly TagsShelfController _tagsShelfController;

        private readonly Tags _tags;

        private ListViewHandler<TagsManagerElementController, Tag> _listViewHandler;

        private TagsManagerViewArgs _args;

        public TagsManagerController(VisualElement parent) : base(parent)
        {
            Services.Add<TagsManagerController>(this);

            _root = parent.Q(nameof(TagsManagerView));
            _view = new TagsManagerView(_root);

            _tagsShelfController = new TagsShelfController(_root, ReturnToPreviousView);

            _tags = ResourcesLocator.GetTags();

            SetViewReturnIconTooltip();

            _root.Root().RegisterCallback<KeyUpEvent>(OnKeyUpReturn);
            InitializeListViewHandler();
            InitializeDropdownSearchField();
            InitializeContextualMenuManipulator();
        }

        public void Dispose()
        {
            Services.Remove<TagsManagerController>();

            _tagsShelfController.Dispose();

            _root.Root().UnregisterCallback<KeyUpEvent>(OnKeyUpReturn);

            _listViewHandler.OnItemMade -= OnItemMadeRegisterEvents;
            _listViewHandler.ListView.itemsRemoved -= HandleItemsRemoved;
            _listViewHandler.ListView.ClearSelection();

            var listView = _listViewHandler.ListView;

            listView.UnregisterCallback<KeyUpEvent>(OnKeyUpDeleteSelection);
            listView.UnregisterCallback<KeyUpEvent>(OnKeyUpBindSelection);
            listView.UnregisterCallback<KeyUpEvent>(OnKeyUpUnbindSelection);
        }

        public void ScrollToItemIndex(int index)
        {
            _listViewHandler.ListView.ScrollToItem(index);
            Signals.Dispatch<UISignals.RefreshView>();

            _listViewHandler.ListView.SetSelection(index);

            var itemAtIndex = _listViewHandler.ListView.itemsSource[index];
            var tagID = ((Tag)itemAtIndex).ID;
            var controller = _listViewHandler.GetFirstVisualElementWhere(element => element.IsTextFieldValue(tagID));
            controller.FocusTextField();
        }

        protected override void Show(ViewArgs args)
        {
            Signals.Add<UISignals.RefreshView>(_tagsShelfController.Populate);
            Signals.Add<UISignals.RefreshView>(_listViewHandler.ForceRebuild);
            _args = (TagsManagerViewArgs)args;
            _view.Show();
            _view.SetReferenceText(_args.TagSubscriberWithTags.GetTagSubscriberWithTagID());
            BuildTagsController(_args.TagSubscriberWithTags);
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

        private void ReturnToPreviousView()
        {
            var args = new SceneElementsViewArgs(_args.TagSubscriberWithTags as SceneAssetDataBinder);
            ReturnToPreviousView(args);
        }

        private void InitializeListViewHandler()
        {
            _listViewHandler =
                new ListViewHandler<TagsManagerElementController, Tag>(
                    _root.Q<ListView>(TagElementsContainerName),
                    ResourcesLocator.GetTags().Elements);

            _listViewHandler.OnItemMade += OnItemMadeRegisterEvents;
            _listViewHandler.ListView.itemsRemoved += HandleItemsRemoved;

            var listView = _listViewHandler.ListView;

            listView.RegisterCallback<KeyUpEvent>(OnKeyUpDeleteSelection);
            listView.RegisterCallback<KeyUpEvent>(OnKeyUpBindSelection);
            listView.RegisterCallback<KeyUpEvent>(OnKeyUpUnbindSelection);
        }

        private void HandleItemsRemoved(IEnumerable<int> enumerable)
        {
            var dataBinders = SceneAssetDataBinders.Instance.Elements.ToList();

            var itemsSource = _listViewHandler.ListView.itemsSource;
            var removedIndexes = enumerable.ToList();

            foreach (var index in removedIndexes)
            {
                var tag = (Tag)itemsSource[index];
                _tagsShelfController.Remove(tag);
                foreach (var element in dataBinders)
                {
                    if (!element.Tags.Contains(tag)) continue;
                    element.Tags.Remove(tag);
                    ResourcesLocator.SaveSceneAssetDataBindersDelayed();
                }
            }

            ResourcesLocator.SaveTags();
        }

        private void OnKeyUpDeleteSelection(KeyUpEvent keyUpEvent)
        {
            if (!((keyUpEvent.ctrlKey || keyUpEvent.commandKey) && keyUpEvent.keyCode == KeyCode.Delete)) return;
            DeleteSelection();
        }

        private void DeleteSelection()
        {
            var arrayOfElements = _listViewHandler.GetSelectedData().ToArray();
            if (!arrayOfElements.Any()) return;

            for (var i = arrayOfElements.Length - 1; i >= 0; i--)
            {
                RemoveTagFromShelf(arrayOfElements[i]);
                _listViewHandler.RemoveSelectedElement();
            }

            _listViewHandler.ListView.ClearSelection();
        }

        private void DeleteSelection(DropdownMenuAction dropdownMenuAction)
        {
            DeleteSelection();
        }

        private void OnKeyUpBindSelection(KeyUpEvent keyUpEvent)
        {
            if (!((keyUpEvent.ctrlKey || keyUpEvent.commandKey) && keyUpEvent.keyCode == KeyCode.Return)) return;
            BindSelection();
        }

        private void BindSelection(DropdownMenuAction dropdownMenuAction)
        {
            BindSelection();
        }

        private void BindSelection()
        {
            var arrayOfElements = _listViewHandler.GetSelectedData().ToArray();
            if (!arrayOfElements.Any()) return;

            foreach (var tagWithSubscribers in arrayOfElements)
            {
                AddTagToShelf(tagWithSubscribers);
            }
        }

        private void OnKeyUpUnbindSelection(KeyUpEvent keyUpEvent)
        {
            if (!((keyUpEvent.ctrlKey || keyUpEvent.commandKey) && keyUpEvent.keyCode == KeyCode.Backspace)) return;
            UnbindSelection();
        }

        private void UnbindSelection()
        {
            var arrayOfElements = _listViewHandler.GetSelectedData().ToArray();
            if (!arrayOfElements.Any()) return;
            for (var i = arrayOfElements.Length - 1; i >= 0; i--)
            {
                RemoveTagFromShelf(arrayOfElements[i]);
            }
        }

        private void UnbindSelection(DropdownMenuAction dropdownMenuAction)
        {
            UnbindSelection();
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

        private void InitializeContextualMenuManipulator()
        {
            _listViewHandler.ListView.AddManipulator(new ContextualMenuManipulator(context =>
            {
                var status = !_listViewHandler.ListView.selectedIndices.Any()
                    ? DropdownMenuAction.Status.Disabled
                    : DropdownMenuAction.Status.Normal;
                context.menu.AppendAction("Bind selection", BindSelection, status);
                context.menu.AppendAction("Unbind selection", UnbindSelection, status);
                context.menu.AppendSeparator();
                context.menu.AppendAction("Delete selection", DeleteSelection, status);
            }));
        }
    }
}