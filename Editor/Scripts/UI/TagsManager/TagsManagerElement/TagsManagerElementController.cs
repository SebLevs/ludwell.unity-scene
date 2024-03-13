using System;
using UnityEngine.UIElements;

namespace Ludwell.Scene.Editor
{
    public class TagsManagerElementController
    {
        public TagWithSubscribers Data;

        private readonly TagsManagerView _tagsManagerView;

        private readonly TagContainer _tagContainer;

        public TagsManagerElementController(VisualElement view)
        {
            _tagsManagerView = view.GetFirstAncestorOfType<TagsManagerView>();
            _tagContainer = DataFetcher.GetTagContainer();
        }

        public void UpdateValue(string value)
        {
            Data.Name = value;
            DataFetcher.SaveEveryScriptableDelayed();
        }

        public void SetValue(TagsManagerElementView view)
        {
            view.SetText(Data.Name);
        }

        public void FocusTextField(TagsManagerElementView view, TextField textField)
        {
            if (!string.IsNullOrEmpty(Data.Name)) return;
            textField.Focus();
            _tagsManagerView.SetPreviousTargetedElementDelegated(view);
        }

        public void HandleInvalidTag()
        {
            if (_tagContainer.CanTagBeAdded(Data)) return;
            _tagsManagerView.RemoveInvalidTagElementDelegated(Data); // todo: find better way to handle either that line or invalidity itself
            Data.RemoveFromAllSubscribers();
        }
    }
}