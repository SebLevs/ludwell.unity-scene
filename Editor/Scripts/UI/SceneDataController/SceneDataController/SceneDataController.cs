using UnityEditor;
using UnityEngine.UIElements;

namespace Ludwell.Scene.Editor
{
    public class SceneDataController : AViewable
    {
        private readonly ViewManager _viewManager;

        private readonly VisualElement _root;
        private SceneDataView _view;
        private readonly QuickLoadController _quickLoadController;

        public SceneDataController(VisualElement parent) : base(parent)
        {
            _root = parent.Q(nameof(SceneDataView));
            _view = new SceneDataView(_root);

            _quickLoadController = new QuickLoadController(_root);
            OnShow = AddRefreshViewSignal;
            OnHide = RemoveRefreshViewSignal;
        }

        protected override void Show(ViewArgs args)
        {
            EditorApplication.playModeStateChanged += HandlePlayModeStateChange;
            _view.Show();
        }

        protected override void Hide()
        {
            EditorApplication.playModeStateChanged -= HandlePlayModeStateChange;
            _view.Hide();
        }

        private void HandlePlayModeStateChange(PlayModeStateChange playModeStateChange)
        {
            Signals.Dispatch<UISignals.RefreshView>();
        }

        private void AddRefreshViewSignal()
        {
            Signals.Add<UISignals.RefreshView>(_quickLoadController.ForceRebuildListView);
        }

        private void RemoveRefreshViewSignal()
        {
            Signals.Remove<UISignals.RefreshView>(_quickLoadController.ForceRebuildListView);
        }
    }
}