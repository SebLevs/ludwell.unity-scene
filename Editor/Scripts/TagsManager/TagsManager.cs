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

        private const string ReturnButtonName = "button__return";
        private const string TagsContainerName = "tags-container";

        private const string LoaderSceneDataPath = "Scriptables/" + nameof(LoaderSceneData);

        private Button _returnButton;
        private VisualElement _tagsContainer;

        private LoaderSceneData _loaderSceneData;

        private ListViewInitializer<TagsManagerElement, Tag> _listViewInitializer;
        private List<string> _cachedTags = new();

        public TagsManager()
        {
            this.AddHierarchyFromUxml(UxmlPath);
            this.AddStyleFromUss(UssPath);

            SetReferences();
            RegisterButtonEvents();
        }

        public void AddTag(string tag)
        {
            if (_cachedTags.Contains(tag))
            {
                Debug.LogWarning($"{tag} is already present.");
                return;
            }

            Debug.LogError(nameof(AddTag));
            _tagsContainer.Add(new Label(tag));
            _cachedTags.Add(tag);
        }

        public void RemoveTag(string tag)
        {
            if (!_cachedTags.Contains(tag))
            {
                Debug.LogWarning($"{tag} was not present.");
                return;
            }

            Debug.LogError(nameof(RemoveTag));
            _tagsContainer.RemoveAt(_cachedTags.IndexOf(tag));
            _cachedTags.Remove(tag);
        }

        public void Show(List<string> tags)
        {
            this.Root().Q<TabController>().SwitchView(this);

            _cachedTags = tags;
            InitializeTags(_cachedTags);
        }

        private void InitializeTags(List<string> tags)
        {
            _cachedTags = tags;
            _tagsContainer.Clear();
            foreach (var tag in _cachedTags)
            {
                _tagsContainer.Add(new Label(tag));
                // var foo = new TagElement();
            }
        }

        private void SetReferences()
        {
            _returnButton = this.Q(ReturnButtonName).Q<Button>();
            _tagsContainer = this.Q<VisualElement>(TagsContainerName);
            _loaderSceneData = Resources.Load<LoaderSceneData>(LoaderSceneDataPath);
            _listViewInitializer = new(this.Q<ListView>(), _loaderSceneData.Tags);
        }

        private void RegisterButtonEvents()
        {
            _returnButton.RegisterCallback<ClickEvent>(_ => Return());
        }

        private void Return()
        {
            Debug.LogError(nameof(Return));
            this.Root().Q<TabController>().ReturnToPreviousView();
        }
    }
}