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
        
        private LoaderSceneData _loaderSceneData;

        private ListViewInitializer<TagsManagerElement, Tag> _listViewInitializer;
        private DropdownSearchField _dropdownSearchField;

        private VisualElement _contentContainer;

        public TagsManager()
        {
            this.AddHierarchyFromUxml(UxmlPath);
            this.AddStyleFromUss(UssPath);

            SetReferences();

            InitializeListViewBehaviours();
            InitializeDropdownSearchField();
        }

        private void InitializeDropdownSearchField()
        {
            _dropdownSearchField.InitDropdownElementBehaviour(_listViewInitializer.ListView, itemIndex =>
            {
                _listViewInitializer.ListView.ScrollToItem(itemIndex);
            });
        }

        public void Show(List<string> tags)
        {
            this.Root().Q<TabController>().SwitchView(this);
            BuildTagsController(tags);
        }

        public void AddTagToController(string tag)
        {
            _tagsController.Add(tag);
        }

        public void RemoveTagFromController(string tag)
        {
            _tagsController.Remove(tag);
        }

        public void RemoveTag(VisualElement tagElement)
        {
            var index = _contentContainer.IndexOf(tagElement.FindFirstParentWithName(UiToolkitNames.UnityListViewReorderableItem));
            _contentContainer.RemoveAt(index);
            _loaderSceneData.Tags.RemoveAt(index);
            LoaderSceneDataHelper.SaveChange();
        }

        public bool IsTagDuplicate(VisualElement tagElement, string tag)
        {
            var index = _contentContainer.IndexOf(tagElement.FindFirstParentWithName(UiToolkitNames.UnityListViewReorderableItem));
            for (var i = 0; i < _loaderSceneData.Tags.Count; i++)
            {
                if (i == index) continue;
                if (_loaderSceneData.Tags[i].Value == tag) return true;
            }

            return false;
        }

        private void BuildTagsController(List<string> tags)
        {
            _tagsController
                .WithTagList(tags)
                .WithOptionButtonEvent(Return)
                .Refresh();
        }

        private void SetReferences()
        {
            _tagsController = this.Q<TagsController>();
            _loaderSceneData = Resources.Load<LoaderSceneData>(LoaderSceneDataPath);
            _listViewInitializer = new(this.Q<ListView>(), _loaderSceneData.Tags);
            _dropdownSearchField = this.Q<DropdownSearchField>();
            _contentContainer = this.Q(UiToolkitNames.UnityContentContainer);
        }
        
        private void InitializeListViewBehaviours()
        {
            _listViewInitializer.ListView.itemsAdded += _ =>
            {
                var last = _contentContainer.Children().Last().Q<TagsManagerElement>();
                last.FocusTextField();
            };
            _listViewInitializer.ListView.itemsRemoved += _ =>
            {
                LoaderSceneDataHelper.SaveChange();
            };
        }

        private void Return()
        {
            Debug.LogError(nameof(Return));
            this.Root().Q<TabController>().ReturnToPreviousView();
        }
    }
}