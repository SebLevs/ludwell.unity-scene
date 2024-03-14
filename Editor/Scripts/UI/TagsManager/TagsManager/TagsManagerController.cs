using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ludwell.Scene.Editor
{
    public class TagsManagerController
    {
        private readonly VisualElement _root;
        private readonly TagsManagerView _view;
        private readonly TagsShelfView _tagsShelfView;

        private readonly TagContainer _tagContainer;

        private VisualElement _previousView;

        private ListViewHandler<TagsManagerElementView, TagWithSubscribers> _listViewHandler;

        private TagsManagerElementView _previousTarget;

        public TagsManagerController(VisualElement parent)
        {
            _root = parent.Q(nameof(TagsManagerView));
            _view = new TagsManagerView(_root, BuildTagsController);

            _tagsShelfView = _root.Q<TagsShelfView>();

            _tagContainer = DataFetcher.GetTagContainer();

            SetViewReturnIconTooltip();

            InitializeReturnEvent();
            InitializeListViewHandler();
            InitializeDropdownSearchField();

            // todo: work around for Focus/Blur overlap issue with ListView Rebuild. Sort logic should be on TME blur.
            InitializeTagSorting();

            _tagContainer.OnRemove += RemoveInvalidTagElement;
        }

        private void AddTagToShelf(TagWithSubscribers tag)
        {
            _tagsShelfView.Add(tag);
        }

        private void RemoveTagFromShelf(TagWithSubscribers tag)
        {
            _tagsShelfView.Remove(tag);
        }

        private void RemoveTagFromAllSubscribers(TagWithSubscribers tag)
        {
            _tagsShelfView.Remove(tag);
            tag.RemoveFromAllSubscribers();
        }

        private void SetPreviousTargetedElement(TagsManagerElementView target)
        {
            _previousTarget = target;
        }

        private void RemoveInvalidTagElement(TagWithSubscribers tag)
        {
            RemoveTagFromShelf(tag);
            DataFetcher.SaveEveryScriptable();
            _listViewHandler.ForceRebuild();
        }

        private void SetViewReturnIconTooltip()
        {
            _tagsShelfView.OverrideIconTooltip("Return");
        }

        public static TagSubscriberWithTags foo;
        private void BuildTagsController() // TagSubscriberWithTags tagSubscriber
        {
            _tagsShelfView
                .WithTagSubscriber(foo)
                .WithOptionButtonEvent(ReturnToPreviousView)
                .PopulateContainer();
        }

        private void ReturnToPreviousView()
        {
            ViewManager.Instance.TransitionToFirstViewOfType<SceneDataView>();
        }

        private void InitializeReturnEvent()
        {
            _root.RegisterCallback<AttachToPanelEvent>(_ => _root.Root().RegisterCallback<KeyUpEvent>(OnKeyUpReturn));
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
                new ListViewHandler<TagsManagerElementView, TagWithSubscribers>(
                    _root.Q<ListView>(),
                    DataFetcher.GetTagContainer().Tags);

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

                DataFetcher.SaveEveryScriptable();
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

        private void OnItemMadeRegisterEvents(TagsManagerElementView view)
        {
            view.OnAdd += AddTagToShelf;
            view.OnRemove += RemoveTagFromShelf;

            view.OnTextEditEnd += () =>
            {
                SortTags();
                SetPreviousTargetedElement(null);
            };
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
            _root.RegisterCallback<MouseUpEvent>(evt =>
            {
                var tagsManagerElement = (evt.target as VisualElement).GetFirstAncestorOfType<TagsManagerElementView>();
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
            DataFetcher.SaveEveryScriptableDelayed();
            _listViewHandler.ForceRebuild();
        }
    }
}