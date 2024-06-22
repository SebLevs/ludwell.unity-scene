using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ludwell.Scene.Editor
{
    public class QuickLoadController
    {
        private const string TagListingStrategyName = "tag";
        private const string TagIconName = "icon_tag";

        private readonly QuickLoadElements _quickLoadElements;
        private ListViewHandler<QuickLoadElementController, QuickLoadElementData> _listViewHandler;

        private readonly QuickLoadView _view;
        private ListView _listView;
        private DropdownSearchField _dropdownSearchField;

        public QuickLoadController(VisualElement parent)
        {
            var root = parent.Q(nameof(QuickLoadView));
            _view = new QuickLoadView(root, CloseAll, SceneDataGenerator.CreateSceneAssetAtPath, DeleteSelection);

            _listView = root.Q<ListView>();
            _dropdownSearchField = root.Q<DropdownSearchField>();

            _quickLoadElements = ResourcesLocator.GetQuickLoadElements();

            InitializeListViewHandler(root.Q<ListView>());
            InitializeSearchField(root, root.Q<DropdownSearchField>());
            InitializeListViewKeyUpEvents();

            ResourcesLocator.QuickLoadController = this; // todo: change for DI or service

            EditorSceneManager.sceneOpened += HandleAdditiveSceneOpened;
            EditorSceneManager.sceneClosed += HandleAdditiveSceneClosed;
            EditorSceneManager.activeSceneChangedInEditMode += HandleActiveSceneChange;
        }

        internal void Dispose()
        {
            EditorSceneManager.sceneOpened += HandleAdditiveSceneOpened;
            EditorSceneManager.sceneClosed += HandleAdditiveSceneClosed;
            EditorSceneManager.activeSceneChangedInEditMode += HandleActiveSceneChange;
        }

        private void HandleAdditiveSceneOpened(UnityEngine.SceneManagement.Scene scene, OpenSceneMode mode)
        {
            if (EditorApplication.isPlaying) return;
            if (mode == OpenSceneMode.Single) return;

            foreach (var quickLoadElementView in _listViewHandler.VisualElements)
            {
                if (quickLoadElementView.Model.SceneData.name != scene.name) continue;
                var scenePath = Path.ChangeExtension(AssetDatabase.GetAssetPath(quickLoadElementView.Model.SceneData),
                    ".unity");
                if (scenePath != scene.path) continue;
                quickLoadElementView.SwitchOpenAdditiveButtonState(true);
                return;
            }
        }

        private void HandleAdditiveSceneClosed(UnityEngine.SceneManagement.Scene scene)
        {
            if (EditorApplication.isPlaying) return;
            foreach (var quickLoadElementView in _listViewHandler.VisualElements)
            {
                if (quickLoadElementView.Model.SceneData.name != scene.name) continue;
                var scenePath = Path.ChangeExtension(AssetDatabase.GetAssetPath(quickLoadElementView.Model.SceneData),
                    ".unity");
                if (scenePath != scene.path) continue;
                quickLoadElementView.SetOpenButtonEnable(true);
                quickLoadElementView.SetOpenAdditiveButtonEnable(true);
                quickLoadElementView.SwitchOpenAdditiveButtonState(false);
                return;
            }
        }

        private void HandleActiveSceneChange(UnityEngine.SceneManagement.Scene arg0,
            UnityEngine.SceneManagement.Scene arg1)
        {
            if (EditorApplication.isPlaying) return;
            var breakAtCount = EditorSceneManager.sceneCount;
            var count = 0;
            foreach (var quickLoadElementView in _listViewHandler.VisualElements)
            {
                var sceneDataName = quickLoadElementView.Model.SceneData.name;
                if (sceneDataName != arg0.name && sceneDataName != arg1.name) continue;
                var scenePath = Path.ChangeExtension(AssetDatabase.GetAssetPath(quickLoadElementView.Model.SceneData),
                    ".unity");
                if (scenePath != arg0.path && scenePath != arg1.path) continue;

                if (scenePath == arg0.path) // previous active scene
                {
                    quickLoadElementView.SetOpenButtonEnable(true);
                    quickLoadElementView.SolveOpenAdditiveButton();
                    if (++count == breakAtCount) return;
                    continue;
                }

                if (scenePath != arg1.path) continue; // new active scene
                quickLoadElementView.SetOpenButtonEnable(false);
                quickLoadElementView.SetOpenAdditiveButtonEnable(false);
                quickLoadElementView.SwitchOpenAdditiveButtonState(false);
                if (++count == breakAtCount) return;
            }
        }

        /// <summary> If no item is selected, deletes the last item. </summary>
        public void DeleteSelection()
        {
            if (_listViewHandler.ListView.itemsSource.Count == 0) return;

            var selectedElementData = _listViewHandler.GetSelectedElementData() != null
                ? _listViewHandler.GetSelectedElementData()
                : _listViewHandler.GetLastData();

            var sceneDataPath = AssetDatabase.GetAssetPath(selectedElementData.SceneData);

            AssetDatabase.DeleteAsset(sceneDataPath);

            if (selectedElementData.IsOutsideAssetsFolder)
            {
                Debug.LogWarning($"Suspicious delete action | Path was outside the Assets folder | {sceneDataPath}");
            }

            Signals.Dispatch<UISignals.RefreshView>();
        }

        // todo: delete when either service or DI is implemented
        public void ScrollToItemIndex(int index)
        {
            _listViewHandler.ListView.ScrollToItem(index);
            _listViewHandler.ListView.SetSelection(index);
            _listViewHandler.GetVisualElementAt(index)?.FocusTextField();
        }

        private void CloseAll()
        {
            if (_quickLoadElements == null || _quickLoadElements.Elements == null) return;

            foreach (var item in _listViewHandler.Data)
            {
                var id = item.SceneData.GetInstanceID().ToString();
                SessionState.SetBool(id, false);
            }

            foreach (var item in _listViewHandler.VisualElements)
            {
                item.SetOpenState(false);
            }
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

                ResourcesLocator.SaveQuickLoadElementsAndTagContainerDelayed();
            };
        }

        private void InitializeSearchField(VisualElement root, DropdownSearchField dropdownSearchField)
        {
            dropdownSearchField.BindToListView(_listViewHandler.ListView);

            var icon = Resources.Load<Texture2D>(Path.Combine("Sprites", TagIconName));
            var searchListingStrategy = new ListingStrategy(TagListingStrategyName, icon, ListTag);

            dropdownSearchField
                .WithResizableParent(root)
                .WithDropdownBehaviour(index =>
                {
                    dropdownSearchField.HideDropdown();
                    _listViewHandler.ListView.ScrollToItem(index);
                })
                .WithCyclingListingStrategy(searchListingStrategy);
        }

        private void InitializeListViewKeyUpEvents()
        {
            _listView.RegisterCallback<KeyUpEvent>(OnKeyUpDeleteSelected);
        }

        private void OnKeyUpDeleteSelected(KeyUpEvent keyUpEvent)
        {
            if (_listView.selectedItem == null) return;
            if (!((keyUpEvent.ctrlKey || keyUpEvent.commandKey) && keyUpEvent.keyCode == KeyCode.Delete)) return;

            DeleteSelection();
        }

        public void ForceRebuildListView()
        {
            _listViewHandler.ForceRebuild();
            _dropdownSearchField.RebuildActiveListing();
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