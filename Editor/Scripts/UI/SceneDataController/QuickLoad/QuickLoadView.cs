using System.IO;
using Ludwell.Scene.Editor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ludwell.Scene
{
    public class QuickLoadView : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<QuickLoadView, UxmlTraits>
        {
        }

        private static readonly string UxmlPath =
            Path.Combine("Uxml", nameof(SceneDataController), nameof(QuickLoadView));

        private static readonly string UssPath =
            Path.Combine("Uss", nameof(SceneDataController), nameof(QuickLoadView));

        private const string ListViewName = "scenes__list";

        private const string ButtonAddName = "add";
        private const string ButtonRemoveName = "remove";

        private readonly DropdownSearchField _dropdownSearchField;
        private readonly ListView _listView;

        private readonly QuickLoadController _controller;

        public QuickLoadView()
        {
            this.AddHierarchyFromUxml(UxmlPath);
            this.AddStyleFromUss(UssPath);

            _listView = this.Q<ListView>(ListViewName);
            _dropdownSearchField = this.Q<DropdownSearchField>();
            _controller = new QuickLoadController(this, _listView, _dropdownSearchField);

            InitializeListViewKeyUpEvents();

            InitializeButtonCloseAll();
            InitializeAddRemoveButtons();

            Signals.Add<UISignals.RefreshQuickLoadListView>(ForceRebuildListView);
        }

        ~QuickLoadView()
        {
            ClearAddRemoveButtons();
            Signals.Remove<UISignals.RefreshQuickLoadListView>(ForceRebuildListView);
        }

        private void InitializeButtonCloseAll()
        {
            var closeAllButton = this.Q<Button>();
            closeAllButton.RegisterCallback<MouseUpEvent>(_ => _controller.CloseAll());
        }

        private void InitializeListViewKeyUpEvents()
        {
            _listView.RegisterCallback<KeyUpEvent>(OnKeyUpDeleteSelected);
        }

        private void OnKeyUpDeleteSelected(KeyUpEvent keyUpEvent)
        {
            if (_listView.selectedItem == null) return;
            if (!((keyUpEvent.ctrlKey || keyUpEvent.commandKey) && keyUpEvent.keyCode == KeyCode.Delete)) return;

            _controller.DeleteSelection();
        }

        private void InitializeAddRemoveButtons()
        {
            this.Q<Button>(ButtonAddName).clicked += SceneDataGenerator.CreateSceneAssetAtPath;
            this.Q<Button>(ButtonRemoveName).clicked += _controller.DeleteSelection;
        }

        private void ClearAddRemoveButtons()
        {
            this.Q<Button>(ButtonAddName).clicked -= SceneDataGenerator.CreateSceneAssetAtPath;
            this.Q<Button>(ButtonRemoveName).clicked -= _controller.DeleteSelection;
        }

        private void ForceRebuildListView()
        {
            _listView.Rebuild();
            _dropdownSearchField.RebuildActiveListing();
        }
    }
}