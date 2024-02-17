using System;
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

        private ListViewInitializer<TagsManagerElement, TagWithSubscribers> _listViewInitializer;
        private DropdownSearchField _dropdownSearchField;

        private TagsManagerElement _previousTarget;

        public TagsManager()
        {
            this.AddHierarchyFromUxml(UxmlPath);
            this.AddStyleFromUss(UssPath);

            SetReferences();

            InitializeListViewBehaviours();
            InitializeDropdownSearchField();
            InitializeReturnInput();
            // todo: work around for Focus/Blur overlap issue with ListView Rebuild. Sort logic should be on TME blur.
            InitializeTagSorting();

            HandleTagController();
        }

        public void Show(TagSubscriberWithTags tagSubscriber)
        {
            _tagSubscriber = tagSubscriber;
            this.Root().Q<TabController>().SwitchView(this);
            BuildTagsController(tagSubscriber.Tags);
        }

        public void AddTagToController(TagWithSubscribers tag)
        {
            _tagsController.Add(tag);
            tag.AddSubscriber(_tagSubscriber);
        }

        public void RemoveTagFromController(TagWithSubscribers tag)
        {
            _tagsController.Remove(tag);
            tag.RemoveSubscriber(_tagSubscriber);
        }

        public void RemoveInvalidTagElement(TagWithSubscribers tag)
        {
            _tagsController.Remove(tag);
            _loaderSceneData.Tags.Remove(tag);
            LoaderSceneDataHelper.SaveChange();
            _listViewInitializer.ForceRebuild();
        }

        public void SetPreviousTarget(TagsManagerElement target)
        {
            _previousTarget = target;
        }

        public bool IsTagDuplicate(Tag elementTag)
        {
            foreach (var tag in _loaderSceneData.Tags)
            {
                if (tag == elementTag) continue;
                if (string.Equals(tag.Name, elementTag.Name, StringComparison.CurrentCultureIgnoreCase)) return true;
            }

            return false;
        }

        public void SortTags()
        {
            _loaderSceneData.Tags.Sort();
            LoaderSceneDataHelper.SaveChangeDelayed();
            _listViewInitializer.ForceRebuild();
        }

        private void BuildTagsController(List<Tag> tags)
        {
            _tagsController
                .WithTags(tags)
                .WithOptionButtonEvent(Return)
                .Populate();
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
                    var tag = itemsSource[index] as TagWithSubscribers;
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

        private void InitializeTagSorting()
        {
            RegisterCallback<MouseUpEvent>(evt =>
            {
                var tagsManagerElement = (evt.target as VisualElement).GetFirstAncestorOfType<TagsManagerElement>();
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
            _tagsController.OverrideIconTooltip("Return");
        }

        private void Return()
        {
            this.Root().Q<TabController>().ReturnToPreviousView();
        }
    }
}