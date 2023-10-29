using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ludwell.Scene
{
    public class SceneLoaderListController: VisualElement
    {
        public new class UxmlFactory : UxmlFactory<SceneLoaderListController, UxmlTraits> { }

        private const string UxmlPath = "Uxml/scene-loader-list";
        private const string UssPath = "Uss/scene-loader-list";
        
        private const string ListViewName = "scenes__list";
        private const string LoaderSceneDataPath = "Scriptables/" + nameof(LoaderSceneData);
        
        private LoaderSceneData _loaderSceneData;
        private ListView _listView;
        private ListViewInitializer<LoaderListViewElement, LoaderListViewElementData> _listViewInitializer;
        private DropdownSearchField _dropdownSearchField;

        public SceneLoaderListController()
        {
            this.SetHierarchyFromUxml(UxmlPath);
            this.AddStyleFromUss(UssPath);
            
            _loaderSceneData = Resources.Load<LoaderSceneData>(LoaderSceneDataPath);
            InitButtonCloseAll();
            InitLoaderListView();
            InitSearchField();
            HandleSearchFieldAbsolutePosition();
        }

        private void InitButtonCloseAll()
        {
            this.Q<ToolbarButton>().clicked += CloseAll;
        }

        private void CloseAll()
        {
            foreach (var element in _loaderSceneData.Elements)
            {
                element.IsOpen = false;
            }

            foreach (var item in _listView.Query<LoaderListViewElement>().ToList())
            {
                item.SetFoldoutValue(false);
            }
        }

        private void InitLoaderListView()
        {
            _listView = this.Q<ListView>(ListViewName);
            _listViewInitializer = new(_listView, _loaderSceneData.Elements);
        }

        private void InitSearchField()
        {
            _dropdownSearchField = this.Q<DropdownSearchField>();
            _dropdownSearchField.InitDropdownElementBehaviour(_listView, (index) =>
            {
                _dropdownSearchField.HideDropdown();
                _listView.ScrollToItem(index);
            });

            _dropdownSearchField.InitMouseEvents(this.Root());
        }

        // todo: delete this atrocity and refactor absolute styling when Unity implements z-index or a better idea appears
        private const string MainMenuFoldoutName = "foldout-header__main-menu";
        private const string CoreScenesFoldoutName = "foldout-header__core";

        private float _mainMenuFoldoutContentHeight;
        private float _coreScenesFoldoutContentHeight;

        private void HandleSearchFieldAbsolutePosition()
        {
            var unityContent = UiToolkitNames.UnityContent;
            var mainMenuFoldout = this.Root().Q<FoldoutHeader>(MainMenuFoldoutName);

            mainMenuFoldout.RegisterValueChangedCallback(evt =>
            {
                var currentTopValue = _dropdownSearchField.resolvedStyle.top;

                if (!evt.newValue)
                {
                    _mainMenuFoldoutContentHeight = mainMenuFoldout.Q<VisualElement>(unityContent).resolvedStyle.height;
                    var newValue = currentTopValue - _mainMenuFoldoutContentHeight;
                    _dropdownSearchField.style.top = newValue;
                }
                else
                {
                    var newValue = currentTopValue + _mainMenuFoldoutContentHeight;
                    _dropdownSearchField.style.top = newValue;
                }
            });

            var coreScenesFoldout = this.Root().Q<FoldoutHeader>(CoreScenesFoldoutName);
            coreScenesFoldout.RegisterValueChangedCallback(evt =>
            {
                var currentTopValue = _dropdownSearchField.resolvedStyle.top;

                if (!evt.newValue)
                {
                    _coreScenesFoldoutContentHeight =
                        coreScenesFoldout.Q<VisualElement>(unityContent).resolvedStyle.height;
                    var newValue = currentTopValue - _coreScenesFoldoutContentHeight;
                    _dropdownSearchField.style.top = newValue;
                }
                else
                {
                    var newValue = currentTopValue + _coreScenesFoldoutContentHeight;
                    _dropdownSearchField.style.top = newValue;
                }
            });
        }
    }
}