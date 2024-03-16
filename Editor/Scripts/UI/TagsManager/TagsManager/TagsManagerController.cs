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

    public class TagsManagerController : IViewable
    {
        private readonly ViewManager _viewManager;
        
        private readonly VisualElement _root;
        private readonly TagsManagerView _view;
        private readonly TagsShelfController _tagsShelfController;

        private readonly TagContainer _tagContainer;

        private ListViewHandler<TagsManagerElementController, TagWithSubscribers> _listViewHandler;

        private TagsManagerElementController _previousTarget;

        public TagsManagerController(VisualElement parent)
        {
            _root = parent.Q(nameof(TagsManagerView));
            _view = new TagsManagerView(_root);
            
            _viewManager = _root.Root().Q<ViewManager>();
            _viewManager.Add(this);

            _tagsShelfController = new TagsShelfController(_root, _ => ReturnToPreviousView());

            _tagContainer = DataFetcher.GetTagContainer();

            SetViewReturnIconTooltip();

            InitializeReturnEvent();
            InitializeListViewHandler();
            InitializeDropdownSearchField();

            // todo: work around for Focus/Blur overlap issue with ListView Rebuild.
            InitializeTagSorting();

            _tagContainer.OnRemove += RemoveInvalidTagElement;
        }

        ~TagsManagerController()
        {
            _tagContainer.OnRemove -= RemoveInvalidTagElement;
        }

        public void Show(ViewArgs args)
        {
            var tagsManagerViewArgs = (TagsManagerViewArgs)args;
            _view.Show();
            _view.SetReferenceText(tagsManagerViewArgs.TagSubscriberWithTags.Name);
            BuildTagsController(tagsManagerViewArgs.TagSubscriberWithTags);
        }

        public void Hide()
        {
            _view.Hide();
        }

        private void BuildTagsController(TagSubscriberWithTags tagSubscriberWithTags)
        {
            _tagsShelfController.UpdateData(tagSubscriberWithTags);
            _tagsShelfController.PopulateContainer();
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
            DataFetcher.SaveEveryScriptable();
            _listViewHandler.ForceRebuild();
        }

        private void SetViewReturnIconTooltip()
        {
            _tagsShelfController.OverrideIconTooltip("Return");
        }

        private void ReturnToPreviousView()
        {
            _viewManager.TransitionToFirstViewOfType<SceneDataController>();
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
                new ListViewHandler<TagsManagerElementController, TagWithSubscribers>(
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

        private void OnItemMadeRegisterEvents(TagsManagerElementController controller)
        {
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
            _root.RegisterCallback<MouseUpEvent>(evt =>
            {
                var tagsManagerElement = ((VisualElement)evt.target).GetFirstAncestorOfType<TagsManagerElementController>();
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