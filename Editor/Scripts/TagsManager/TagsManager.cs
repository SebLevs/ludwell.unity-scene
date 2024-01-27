using Ludwell.Scene.Editor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ludwell.Scene
{
    public class TagsManager : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<TagsManager, UxmlTraits> { }
        
        private const string UxmlPath = "Uxml/" + nameof(TagsManager) + "/" + nameof(TagsManager);
        private const string UssPath = "Uss/" + nameof(TagsManager) + "/" + nameof(TagsManager);

        private const string ReturnButtonName = "button__return";
        private const string TagsContainerName = "tags-container";
        
        private const string LoaderSceneDataPath = "Scriptables/" + nameof(LoaderSceneData);

        private Button _returnButton;
        private VisualElement _tagsContainer;

        private LoaderSceneData _loaderSceneData;

        private ListViewInitializer<TagsManagerElement, Tag> _listViewInitializer;

        private TagsController _currentTagsController;

        public TagsManager()
        {
            this.AddHierarchyFromUxml(UxmlPath);
            this.AddStyleFromUss(UssPath);

            SetReferences();
            RegisterButtonEvents();
        }
        
        public void Show(TagsController tagsController)
        {
            this.Root().Q<TabController>().SwitchView(this);
            _currentTagsController = tagsController;

            InitializeTags(_currentTagsController);
        }

        private void InitializeTags(TagsController tagsController)
        {
            _tagsContainer.Clear();
            foreach (var child in tagsController.GetChildren)
            {
                // todo: list the tags here
                // _tagsContainer.Add(new Label("A"));
                var foo = new TagElement();
                foo.SetTagsController(tagsController);
                _tagsContainer.Add(foo);
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
            this.Root().Q<TabController>().ReturnToPreviousView();
        }
    }
}
