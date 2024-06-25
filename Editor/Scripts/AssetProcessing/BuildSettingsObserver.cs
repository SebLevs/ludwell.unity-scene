using System;
using System.Collections.Generic;
using UnityEditor;

namespace Ludwell.Scene.Editor
{
    [InitializeOnLoad]
    public class BuildSettingsObserver
    {
        private static int _buildSettingsCount;

        private static readonly Dictionary<SceneData, Action> _subscribers = new();

        static BuildSettingsObserver()
        {
            _buildSettingsCount = EditorBuildSettings.scenes.Length;
            EditorApplication.update += Update;
        }

        public static void Clear()
        {
            _subscribers.Clear();
        }

        public static void Subscribe(SceneData sceneData, Action action)
        {
            _subscribers.TryAdd(sceneData, action);
        }

        public static void Unsubscribe(SceneData sceneData)
        {
            _subscribers.Remove(sceneData);
        }

        private static void Update()
        {
            if (_buildSettingsCount == EditorBuildSettings.scenes.Length) return;
            _buildSettingsCount = EditorBuildSettings.scenes.Length;

            foreach (var action in _subscribers)
            {
                action.Value?.Invoke();
            }
        }
    }
}