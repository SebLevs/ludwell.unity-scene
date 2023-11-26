using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ludwell.Scene
{
    public class SceneLoaderListController : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<SceneLoaderListController, UxmlTraits> { }

        private const string UxmlPath = "Uxml/" + nameof(LoaderController) + "/" + nameof(SceneLoaderListController);
        private const string UssPath = "Uss/" + nameof(LoaderController) + "/" + nameof(SceneLoaderListController);

        private const string ListViewName = "scenes__list";
        private const string LoaderSceneDataPath = "Scriptables/" + nameof(LoaderSceneData);

        private const string _buttonCloseAll = "button__close-all";
        private const string _buttonCloseAllClicked = "button__close-all-clicked";

        private LoaderSceneData _loaderSceneData;
        private ListView _listView;
        private ListViewInitializer<LoaderListViewElement, LoaderListViewElementData> _listViewInitializer;
        private DropdownSearchField _dropdownSearchField;

        public SceneLoaderListController()
        {
            this.AddHierarchyFromUxml(UxmlPath);
            this.AddStyleFromUss(UssPath);

            _loaderSceneData = Resources.Load<LoaderSceneData>(LoaderSceneDataPath);
            InitButtonCloseAll();
            InitLoaderListView();
            InitSearchField();
        }

        private void InitButtonCloseAll()
        {
            var closeAllButton = this.Q<ToolbarButton>();

            closeAllButton.RegisterCallback<MouseDownEvent>(_ =>
            {
                closeAllButton.RemoveFromClassList(_buttonCloseAll);
                closeAllButton.AddToClassList(_buttonCloseAllClicked);
            }, TrickleDown.TrickleDown);

            closeAllButton.RegisterCallback<MouseUpEvent>(_ =>
            {
                CloseAll();
                closeAllButton.RemoveFromClassList(_buttonCloseAllClicked);
                closeAllButton.AddToClassList(_buttonCloseAll);
            });
        }

        private void CloseAll()
        {
            if (_loaderSceneData == null || _loaderSceneData.Elements == null) return;

            for (var index = 0; index < _loaderSceneData.Elements.Count; index++)
            {
                if (_loaderSceneData.Elements[index] == null)
                {
                    Debug.LogError("ERROR: Index was out of range");
                    continue;
                }

                var element = _loaderSceneData.Elements[index];
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
            _listView.itemsRemoved += _ => LoaderSceneDataHelper.SaveChangeDelayed();
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
    }
}