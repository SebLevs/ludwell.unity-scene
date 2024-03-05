using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ludwell.Scene.Editor
{
    public class QuickLoadController
    {
        private const string TagListingStrategyName = "tag";
        private const string TagIconName = "icon_tag";
        
        private readonly QuickLoadElements _quickLoadElements;
        private ListViewHandler<QuickLoadElement, QuickLoadElementData> _listViewHandler;

        public QuickLoadController(VisualElement view, ListView listView, DropdownSearchField dropdownSearchField)
        {
            _quickLoadElements = DataFetcher.GetQuickLoadElements();
            
            InitializeListViewHandler(listView);
            
            InitializeSearchField(view, dropdownSearchField);
        }
        
        public void DeleteSceneAtPath()
        {
            if (_listViewHandler.ListView.itemsSource.Count == 0) return;

            SceneData sceneData;

            var selectedIndex = _listViewHandler.ListView.selectedIndex;
            QuickLoadElementData elementToDelete;
            if (selectedIndex == -1)
            {
                elementToDelete = _listViewHandler.ListView.itemsSource[^1] as QuickLoadElementData;
                sceneData = elementToDelete.MainScene;
                _listViewHandler.ListView.itemsSource.Remove(sceneData);
                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(sceneData));
                return;
            }

            elementToDelete = _listViewHandler.ListView.itemsSource[selectedIndex] as QuickLoadElementData;
            sceneData = elementToDelete.MainScene;
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(sceneData));
        }
        
        public void CloseAll()
        {
            if (_quickLoadElements == null || _quickLoadElements.Elements == null) return;

            foreach (var element in _quickLoadElements.Elements)
            {
                element.IsOpen = false;
            }

            foreach (var item in _listViewHandler.ListView.Query<QuickLoadElement>().ToList())
            {
                item.SetFoldoutValue(false);
            }
        }
        
        public void DeleteSelectedScene()
        {
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(_listViewHandler.GetSelectedElementData().MainScene));
        }
        
        private void InitializeListViewHandler(ListView listView)
        {
            _listViewHandler = new(listView, _quickLoadElements.Elements);
            
            _listViewHandler.ListView.itemsRemoved += indexEnumerable =>
            {
                foreach (var index in indexEnumerable)
                {
                    var element = _quickLoadElements.Elements[index] as TagSubscriberWithTags;
                    element.RemoveFromAllTags();
                }

                DataFetcher.SaveEveryScriptableDelayed();
            };
        }
        
        private void InitializeSearchField(VisualElement view, DropdownSearchField dropdownSearchField)
        {
            dropdownSearchField.BindToListView(_listViewHandler.ListView);

            var icon = Resources.Load<Texture2D>(Path.Combine("Sprites", TagIconName));
            var searchListingStrategy = new ListingStrategy(TagListingStrategyName, icon, ListTag);

            dropdownSearchField
                .WithResizableParent(view)
                .WithDropdownBehaviour(index =>
                {
                    dropdownSearchField.HideDropdown();
                    _listViewHandler.ListView.ScrollToItem(index);
                })
                .WithCyclingListingStrategy(searchListingStrategy);
        }
        
        private List<IListable> ListTag(string searchFieldValue, IList boundItemSource)
        {
            List<IListable> filteredList = new();

            foreach (var listViewElement in boundItemSource)
            {
                foreach (var tag in (listViewElement as QuickLoadElementData).Tags)
                {
                    if (tag.Name != searchFieldValue) continue;
                    filteredList.Add(listViewElement as IListable);
                    break;
                }
            }

            return filteredList;
        }
    }
}
