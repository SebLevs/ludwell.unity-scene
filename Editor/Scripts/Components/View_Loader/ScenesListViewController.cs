using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ludwell.Scene
{
    [Serializable]
    public class ScenesListViewController
    {
        private const string ListViewName = "scenes__list";
        private const string ToolbarSearchFieldName = "search__scene-loader-element";
        private const string LoaderSceneDataPath = "Scriptables/" + nameof(LoaderSceneData);

        private LoaderSceneData _loaderSceneData;
        private ListView _listView;
        private ListViewInitializer<LoaderListViewElement, LoaderListViewElementData> _listViewInitializer;
        private DropdownSearchField _dropdownSearchField;

        public ScenesListViewController(VisualElement queryFrom)
        {
            _loaderSceneData = Resources.Load<LoaderSceneData>(LoaderSceneDataPath);
            InitLoaderListView(queryFrom, _loaderSceneData);
            InitSearchField(queryFrom);
            HandleSearchFieldAbsolutePosition(queryFrom);
        }

        public void CloseAll()
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

        private void InitLoaderListView(VisualElement queryFrom, LoaderSceneData data)
        {
            _listView = queryFrom.Q<ListView>(ListViewName);
            _listViewInitializer = new(_listView, data.Elements);
        }

        private void InitSearchField(VisualElement queryFrom)
        {
            _dropdownSearchField = queryFrom.Q<DropdownSearchField>(ToolbarSearchFieldName);
            _dropdownSearchField.InitDropdownElementBehaviour(_listView, (index) =>
            {
                _dropdownSearchField.HideDropdown();
                _listView.ScrollToItem(index);
            });

            _dropdownSearchField.InitMouseEvents(queryFrom.Root());
        }

        // todo: delete this atrocity and refactor absolute styling when Unity implements z-index or a better idea appears
        private const string MainMenuFoldoutName = "foldout-header__main-menu";
        private const string CoreScenesFoldoutName = "foldout-header__core";
        private const string UnityContentName = "unity-content";
        
        private float _mainMenuFoldoutContentHeight;
        private float _coreScenesFoldoutContentHeight;

        private void HandleSearchFieldAbsolutePosition(VisualElement queryFrom)
        {
            var mainMenuFoldout = queryFrom.Q<FoldoutHeader>(MainMenuFoldoutName);
            
            mainMenuFoldout.RegisterValueChangedCallback(evt =>
            {
                var currentTopValue = _dropdownSearchField.resolvedStyle.top;
                
                if (!evt.newValue)
                {
                    _mainMenuFoldoutContentHeight = mainMenuFoldout.Q<VisualElement>(UnityContentName).resolvedStyle.height;
                    var newValue = currentTopValue - _mainMenuFoldoutContentHeight;
                    _dropdownSearchField.style.top = newValue;
                }
                else
                {
                    var newValue = currentTopValue + _mainMenuFoldoutContentHeight;
                    _dropdownSearchField.style.top = newValue;
                }
            });

            var coreScenesFoldout = queryFrom.Q<FoldoutHeader>(CoreScenesFoldoutName);
            coreScenesFoldout.RegisterValueChangedCallback(evt =>
            {
                var currentTopValue = _dropdownSearchField.resolvedStyle.top;

                if (!evt.newValue)
                {
                    _coreScenesFoldoutContentHeight = coreScenesFoldout.Q<VisualElement>(UnityContentName).resolvedStyle.height;
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