using System.Linq;
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

        private ListViewHandler<TagsManagerElementController, TagWithSubscribers> _listViewHandler;

        private TagsManagerElementController _previousTarget;

        public TagsManagerController(VisualElement parent) : base(parent)
        {
            _root = parent.Q(nameof(TagsManagerView));
            _view = new TagsManagerView(_root);

            _tagsShelfController = new TagsShelfController(_root, _ => ReturnToPreviousView());

            _tagContainer = ResourcesFetcher.GetTagContainer();

            SetViewReturnIconTooltip();

            InitializeReturnEvent();
            InitializeListViewHandler();
            InitializeDropdownSearchField();

            // todo: work around for Focus/Blur overlap issue with ListView Rebuild.
            InitializeTagSorting();
        }

        protected override void Show(ViewArgs args)
        {
            _tagContainer.OnRemove += RemoveInvalidTagElement;
            Signals.Add<UISignals.RefreshView>(_tagsShelfController.Populate);
            Signals.Add<UISignals.RefreshView>(_listViewHandler.ForceRebuild);
            var tagsManagerViewArgs = (TagsManagerViewArgs)args;
            _view.Show();
            _view.SetReferenceText(tagsManagerViewArgs.TagSubscriberWithTags.Name);
            BuildTagsController(tagsManagerViewArgs.TagSubscriberWithTags);
        }

        protected override void Hide()
        {
            _tagContainer.OnRemove -= RemoveInvalidTagElement;
            Signals.Remove<UISignals.RefreshView>(_tagsShelfController.Populate);
            Signals.Remove<UISignals.RefreshView>(_listViewHandler.ForceRebuild);
            _view.Hide();
        }

        private void BuildTagsController(TagSubscriberWithTags tagSubscriberWithTags)
        {
            _tagsShelfController.UpdateData(tagSubscriberWithTags);
            _tagsShelfController.Populate();
        }

        private void AddTagToShelf(TagWithSubscribers tag)
        {
            _tagsShelfController.Add(tag);
        }

        private void RemoveTagFromShelf(TagWithSubscribers tag)
        {
            _tagsShelfController.Remove(tag);
        }

        private void RemoveTagFromAllSubscribers(TagWithSubscribers tag)
        {
            _tagsShelfController.Remove(tag);
            tag.RemoveFromAllSubscribers();
        }

        private void SetPreviousTargetedElement(TagsManagerElementController target)
        {
            _previousTarget = target;
        }

        private void RemoveInvalidTagElement(TagWithSubscribers tag)
        {
            RemoveTagFromShelf(tag);
            ResourcesFetcher.SaveTagContainer();
            _listViewHandler.ForceRebuild();
        }

        private void SetViewReturnIconTooltip()
        {
            _tagsShelfController.OverrideIconTooltip("Return");
        }

        private void InitializeReturnEvent()
        {
            _root.Root().RegisterCallback<KeyUpEvent>(OnKeyUpReturn);
        }

        private void OnKeyUpReturn(KeyUpEvent evt)
        {
            if (_root.style.display == DisplayStyle.None) return;

            if (evt.keyCode == KeyCode.Escape)
            {
                ReturnToPreviousView();
            }
        }

        private void InitializeListViewHandler()
        {
            _listViewHandler =
                new ListViewHandler<TagsManagerElementController, TagWithSubscribers>(
                    _root.Q<ListView>(TagElementsContainerName),
                    ResourcesFetcher.GetTagContainer().Tags);

            _listViewHandler.OnItemMade += OnItemMadeRegisterEvents;

            var listView = _listViewHandler.ListView;
            listView.RegisterCallback<KeyUpEvent>(OnKeyUpDeleteSelected);
            listView.RegisterCallback<KeyUpEvent>(OnKeyUpAddSelected);
            listView.RegisterCallback<KeyUpEvent>(OnKeyUpRemoveSelected);

            _listViewHandler.ListView.itemsRemoved += indexEnumerable =>
            {
                var itemsSource = _listViewHandler.ListView.itemsSource;
                var removedIndexes = indexEnumerable.ToList();
                foreach (var index in removedIndexes)
                {
                    var tag = itemsSource[index] as TagWithSubscribers;
                    RemoveTagFromAllSubscribers(tag);
                }

                ResourcesFetcher.SaveQuickLoadElementsAndTagContainerDelayed();
            };
        }

        private void OnKeyUpDeleteSelected(KeyUpEvent keyUpEvent)
        {
            if (_listViewHandler.ListView.selectedItem == null) return;
            if (!((keyUpEvent.ctrlKey || keyUpEvent.commandKey) && keyUpEvent.keyCode == KeyCode.Delete)) return;

            var data = _listViewHandler.GetSelectedElementData();
            RemoveTagFromShelf(data);
            data.RemoveFromAllSubscribers();
            _listViewHandler.RemoveSelectedElement();
        }

        private void OnKeyUpAddSelected(KeyUpEvent keyUpEvent)
        {
            if (_listViewHandler.ListView.selectedItem == null) return;
            if (!((keyUpEvent.ctrlKey || keyUpEvent.commandKey) && keyUpEvent.keyCode == KeyCode.Return)) return;

            var data = _listViewHandler.GetSelectedElementData();
            AddTagToShelf(data);
        }

        private void OnKeyUpRemoveSelected(KeyUpEvent keyUpEvent)
        {
            if (_listViewHandler.ListView.selectedItem == null) return;
            if (!((keyUpEvent.ctrlKey || keyUpEvent.commandKey) && keyUpEvent.keyCode == KeyCode.Backspace)) return;

            var data = _listViewHandler.GetSelectedElementData();
            RemoveTagFromShelf(data);
        }

        private void OnItemMadeRegisterEvents(TagsManagerElementController controller)
        {
            SetPreviousTargetedElement(controller);
            controller.OnAdd += AddTagToShelf;
            controller.OnRemove += RemoveTagFromShelf;
            controller.OnValueChanged += OnControllerTextEditEnd;
        }

        private void OnControllerTextEditEnd(string _)
        {
            SortTags();
            SetPreviousTargetedElement(null);
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

        private void InitializeTagSorting()
        {
            _root.parent.RegisterCallback<MouseUpEvent>(evt =>
            {
                if (_root.style.display == DisplayStyle.None) return;

                var tagsManagerElement =
                    ((VisualElement)evt.target).GetFirstAncestorOfType<TagsManagerElementController>();
                if (_previousTarget != null && _previousTarget != tagsManagerElement)
                {
                    SortTags();
                    _previousTarget = null;
                }

                if (evt.target is not TextElement) return;
                if (tagsManagerElement == null) return;
                _previousTarget = tagsManagerElement;
            });
        }

        private void SortTags()
        {
            _tagContainer.Tags.Sort();
            ResourcesFetcher.SaveTagContainerDelayed();
            _listViewHandler.ForceRebuild();
        }
    }
}