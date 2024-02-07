using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ludwell.Scene.Editor
{
    // [CreateAssetMenu(fileName = "LoaderSceneData", menuName = "SceneDataManager/LoaderSceneData")]
    [Serializable]
    public class LoaderSceneData : ScriptableObject
    {
        [HideInInspector] [SerializeField] private SceneData mainMenuScene;
        [HideInInspector] [SerializeField] private SceneData persistentScene;
        [HideInInspector] [SerializeField] private SceneData loadingScene;

        [field: HideInInspector]
        [field: SerializeField]
        public List<LoaderListViewElementData> Elements { get; set; } = new();

        [field: HideInInspector]
        [field: SerializeField]
        public List<Tag> Tags { get; set; } = new();

        public SceneData MainMenuScene
        {
            get => mainMenuScene;
            set
            {
                mainMenuScene = value;
                LoaderSceneDataHelper.SaveChange();
            }
        }

        public SceneData PersistentScene
        {
            get => persistentScene;
            set
            {
                persistentScene = value;
                LoaderSceneDataHelper.SaveChange();
            }
        }

        public SceneData LoadingScene
        {
            get => loadingScene;
            set
            {
                loadingScene = value;
                LoaderSceneDataHelper.SaveChange();
            }
        }
    }

    [Serializable]
    public class LoaderListViewElementData : TagSubscriber, IListable
    {
        [SerializeField] private string name = LoaderListViewElement.DefaultHeaderTextValue;
        [SerializeField] private bool isOpen = true;
        [SerializeField] private SceneData mainScene;

        [field: SerializeField] public List<SceneDataReference> RequiredScenes { get; set; } = new();

        public string Name
        {
            get => name;
            set
            {
                if (name == value) return;
                name = value;
                LoaderSceneDataHelper.SaveChangeDelayed();
            }
        }

        public bool IsOpen
        {
            get => isOpen;
            set
            {
                if (isOpen == value) return;
                isOpen = value;
                LoaderSceneDataHelper.SaveChangeDelayed();
            }
        }

        public SceneData MainScene
        {
            get => mainScene;
            set
            {
                mainScene = value;
                LoaderSceneDataHelper.SaveChange();
            }
        }

        public string GetName()
        {
            return name;
        }
    }

    [Serializable]
    public class SceneDataReference
    {
        private SceneData _sceneData;

        public SceneData SceneData
        {
            get => _sceneData;
            set
            {
                _sceneData = value;
                LoaderSceneDataHelper.SaveChange();
            }
        }
    }

    [Serializable]
    public class Tag : IListable, IComparable
    {
        [SerializeField] private string _value;

        [SerializeField] private List<TagSubscriber> _subscribers = new();

        public void AddSubscriber(TagSubscriber subscriber)
        {
            _subscribers.Add(subscriber);
        }

        public void RemoveSubscriber(TagSubscriber subscriber)
        {
            _subscribers.Remove(subscriber);
        }

        public void RemoveFromAllSubscribers()
        {
            foreach (var subscriber in _subscribers)
            {
                subscriber.Tags.Remove(this);
            }
        }

        public string Value
        {
            get => _value;
            set
            {
                if (_value == value) return;
                _value = value;
                LoaderSceneDataHelper.SaveChangeDelayed();
            }
        }

        public string GetName()
        {
            return _value;
        }

        public int CompareTo(object obj)
        {
            if (obj is not Tag other) return 1;
            return string.Compare(Value, other.Value, StringComparison.InvariantCulture);
        }
    }

    [Serializable]
    public class TagSubscriber
    {
        [field: SerializeField] public List<Tag> Tags { get; set; } = new();
    }
}