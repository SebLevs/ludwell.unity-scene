using UnityEngine;
using UnityEngine.UIElements;

namespace Ludwell.Scene.Editor
{
    public class PresetManagerViewArgs : ViewArgs
    {
        public PresetManagerViewArgs(QuickLoadElementData model)
        {
            Model = model;
        }

        public QuickLoadElementData Model { get; }
    }

    public class PresetManagerController : IViewable
    {
        private ViewManager _viewManager;

        private PresetManagerView _view;
        private VisualElement _root;

        private ListViewHandler<DataPresetElementController, JsonData> _selectedPresetlistViewHandler;

        public PresetManagerController(VisualElement parent)
        {
            _root = parent.Q(nameof(PresetManagerView));
            _view = new PresetManagerView(_root, ReturnToPreviousView);

            _viewManager = _root.Root().Q<ViewManager>();
            _viewManager.Add(this);

            InitializeReturnEvent();
        }

        public void Show(ViewArgs args)
        {
            _view.Show();

            var tagsManagerViewArgs = (PresetManagerViewArgs)args;
            _view.SetReferenceText(tagsManagerViewArgs.Model.Name);

            _selectedPresetlistViewHandler = new ListViewHandler<DataPresetElementController, JsonData>(
                _root.Q<ListView>(),
                tagsManagerViewArgs.Model.PresetData.GetValidDataPreset());
        }

        public void Hide()
        {
            _view.Hide();
        }

        private void InitializeReturnEvent()
        {
            _root.Root().RegisterCallback<KeyUpEvent>(OnKeyUpReturn);
        }

        private void OnKeyUpReturn(KeyUpEvent evt)
        {
            if (_root.style.display == DisplayStyle.None) return;

            if (evt.keyCode == KeyCode.Escape)
            {
                ReturnToPreviousView();
            }
        }

        private void ReturnToPreviousView()
        {
            _viewManager.TransitionToPreviousView();
        }
    }
}