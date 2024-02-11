using System.Collections.Generic;
using System.Linq;
using Ludwell.Scene.Editor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ludwell.Scene
{
    public class TagsManager : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<TagsManager, UxmlTraits>
        {
        }

        private const string UxmlPath = "Uxml/" + nameof(TagsManager) + "/" + nameof(TagsManager);
        private const string UssPath = "Uss/" + nameof(TagsManager) + "/" + nameof(TagsManager);

        private const string LoaderSceneDataPath = "Scriptables/" + nameof(LoaderSceneData);

        private TagsController _tagsController;
        private TagSubscriber _tagSubscriber;

        private LoaderSceneData _loaderSceneData;

        private ListViewInitializer<TagsManagerElement, Tag> _listViewInitializer;
        private DropdownSearchField _dropdownSearchField;

        public TagsManager()
        {
            this.AddHierarchyFromUxml(UxmlPath);
            this.AddStyleFromUss(UssPath);

            SetReferences();

            InitializeListViewBehaviours();
            InitializeDropdownSearchField();
            InitializeReturnInput();

            HandleTagController();
        }

        public void Show(TagSubscriber tagSubscriber)
        {
            _tagSubscriber = tagSubscriber;
            this.Root().Q<TabController>().SwitchView(this);
            BuildTagsController(tagSubscriber.Tags);
        }

        public void AddTagToController(Tag tag)
        {
            _tagsController.Add(tag);
            tag.AddSubscriber(_tagSubscriber);
        }

        public void RemoveTagFromController(Tag tag)
        {
            _tagsController.Remove(tag);
            tag.RemoveSubscriber(_tagSubscriber);
        }

        public void RemoveInvalidTagElement(Tag tag)
        {
            _loaderSceneData.Tags.Remove(tag);
            LoaderSceneDataHelper.SaveChange();
            _listViewInitializer.ForceRebuild();
        }

        public void SortTags()
        {
            Debug.LogError("SortTags");
            _loaderSceneData.Tags.Sort();
            LoaderSceneDataHelper.SaveChange();
            _listViewInitializer.ForceRebuild();
        }

        public bool IsTagDuplicate(Tag elementTag)
        {
            foreach (var tag in _loaderSceneData.Tags)
            {
                if (tag == elementTag) continue;
                if (tag.Value == elementTag.Value) return true;
            }

            return false;
        }

        private void BuildTagsController(List<Tag> tags)
        {
            _tagsController
                .WithTags(tags)
                .WithOptionButtonEvent(Return)
                .Refresh();
        }

        private void SetReferences()
        {
            _tagsController = this.Q<TagsController>();
            _loaderSceneData = Resources.Load<LoaderSceneData>(LoaderSceneDataPath);
            _listViewInitializer = new(this.Q<ListView>(), _loaderSceneData.Tags);
            _dropdownSearchField = this.Q<DropdownSearchField>();
        }

        private void InitializeListViewBehaviours()
        {
            _listViewInitializer.ListView.itemsRemoved += indexEnumerable =>
            {
                var itemsSource = _listViewInitializer.ListView.itemsSource;
                var removedIndexes = indexEnumerable.ToList();
                foreach (var index in removedIndexes)
                {
                    var tag = itemsSource[index] as Tag;
                    _tagsController.Remove(tag);
                    tag.RemoveFromAllSubscribers();
                }

                LoaderSceneDataHelper.SaveChange();
            };
        }

        private void InitializeDropdownSearchField()
        {
            _dropdownSearchField.BindToListView(_listViewInitializer.ListView);
            _dropdownSearchField.WithDropdownBehaviour(itemIndex =>
            {
                _listViewInitializer.ListView.ScrollToItem(itemIndex);
            });
        }

        private void InitializeReturnInput()
        {
            RegisterCallback<AttachToPanelEvent>(_ => this.Root().RegisterCallback<KeyDownEvent>(OnKeyDown));
        }

        private void OnKeyDown(KeyDownEvent evt)
        {
            if (style.display == DisplayStyle.None) return;

            if (evt.keyCode == KeyCode.Escape)
            {
                Return();
            }
        }

        private void HandleTagController()
        {
            _tagsController.OverrideIconTooltip("Return");
        }

        private void Return()
        {
            this.Root().Q<TabController>().ReturnToPreviousView();
        }
    }
}