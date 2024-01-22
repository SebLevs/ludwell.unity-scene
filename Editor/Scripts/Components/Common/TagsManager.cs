using Ludwell.Scene.Editor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ludwell.Scene
{
    public class TagsManager : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<TagsManager, UxmlTraits> { }
        
        private const string UxmlPath = "Uxml/" + "Common" + "/" + nameof(TagsManager);
        private const string UssPath = "Uss/" + "Common" + "/" + nameof(TagsManager);

        private const string ReturnButtonName = "button__return";
        private const string TagsContainerName = "tags-container";
        
        private const string LoaderSceneDataPath = "Scriptables/" + nameof(LoaderSceneData);

        private Button _returnButton;
        private VisualElement _tagsContainer;

        private LoaderSceneData _loaderSceneData;

        private ListViewInitializer<Label, Tag> _listViewInitializer;

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
                _tagsContainer.Add(new Label("A"));
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
