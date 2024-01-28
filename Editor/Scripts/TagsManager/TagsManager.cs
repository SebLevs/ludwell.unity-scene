using System.Collections.Generic;
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
        private List<string> _cachedTags = new();

        public TagsManager()
        {
            this.AddHierarchyFromUxml(UxmlPath);
            this.AddStyleFromUss(UssPath);

            SetReferences();
        }

        public void AddTag(string tag)
        {
            _tagsController.Add(tag);
        }

        public void RemoveTag(string tag)
        {
            _tagsController.Remove(tag);
        }

        public void Show(List<string> tags)
        {
            this.Root().Q<TabController>().SwitchView(this);

            _cachedTags = tags;
            BuildTagsController(tags);
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
        }

        private void Return()
        {
            Debug.LogError(nameof(Return));
            this.Root().Q<TabController>().ReturnToPreviousView();
        }
    }
}