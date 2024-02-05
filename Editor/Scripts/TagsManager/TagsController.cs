using System;
using System.Collections.Generic;
using Ludwell.Scene.Editor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ludwell.Scene
{
    public class TagsController : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<TagsController, UxmlTraits>
        {
        }

        private const string UxmlPath = "Uxml/" + nameof(TagsManager) + "/" + nameof(TagsController);
        private const string UssPath = "Uss/" + nameof(TagsManager) + "/" + nameof(TagsController);

        private const string AddButtonName = "tags__button-add";
        private const string TagsContainerName = "tags-container";
        private const string NotTaggedName = "not-tagged";
        private const string IconName = "icon";
        private const string IconButtonName = "tags__button-add";

        private List<string> _cachedTags = new();
        
        private Button _manageTagsButton;
        private VisualElement _tagsContainer;
        private Label _notTaggedLabel; 

        public TagsController()
        {
            this.AddHierarchyFromUxml(UxmlPath);
            this.AddStyleFromUss(UssPath);

            SetReferences();
        }
        
        public void OverrideIcon(Texture2D icon)
        {
            this.Q(IconName).style.backgroundImage = new StyleBackground(icon);
        }
        
        public void OverrideIconTooltip(string value)
        {
            this.Q(IconButtonName).tooltip = value;
        }

        public TagsController WithTagList(List<string> tags)
        {
            _cachedTags = tags;
            return this;
        }
        
        public TagsController WithOptionButtonEvent(Action callback)
        {
            _manageTagsButton.style.display = DisplayStyle.Flex;
            _manageTagsButton.RegisterCallback<ClickEvent>(_ =>
            {
                callback.Invoke();
            });
            return this;
        }

        public void Add(string tag)
        {
            if (_cachedTags.Contains(tag)) return;
            
            _cachedTags.Add(tag);
            _tagsContainer.Add(CreateTagElement(tag));
            HandleNotTaggedState();

#if UNITY_EDITOR 
            LoaderSceneDataHelper.SaveChange();
#endif
        }

        public void Remove(string tag)
        {
            if (!_cachedTags.Contains(tag)) return;

            _tagsContainer.RemoveAt(_cachedTags.IndexOf(tag));
            _cachedTags.Remove(tag);
            HandleNotTaggedState();
            
#if UNITY_EDITOR 
            LoaderSceneDataHelper.SaveChange();
#endif
        }
        
        public void ShowElementsWithTag(string tag)
        {
            Debug.LogError(nameof(ShowElementsWithTag));
            // todo: change what list the listview reference 
        }

        public void Refresh()
        {
            _tagsContainer.Clear();
            
            foreach (var tag in _cachedTags)
            {
                _tagsContainer.Add(CreateTagElement(tag));
            }

            HandleNotTaggedState();
        }

        private void SetReferences()
        {
            _manageTagsButton = this.Q<Button>(AddButtonName);
            _tagsContainer = this.Q<VisualElement>(TagsContainerName);
            _notTaggedLabel = this.Q<Label>(NotTaggedName);
        }

        private TagElement CreateTagElement(string value)
        {
            TagElement tagElement = new();
            tagElement.SetTagName(value);
            return tagElement;
        }

        private void HandleNotTaggedState()
        {
            _notTaggedLabel.style.display = _cachedTags.Count == 0 ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }
}