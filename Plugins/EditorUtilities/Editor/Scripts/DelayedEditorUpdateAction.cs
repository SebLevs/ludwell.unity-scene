using UnityEditor;
using UnityEngine;
using CallbackFunction = UnityEditor.EditorApplication.CallbackFunction;

namespace Ludwell.EditorUtilities
{
    public class DelayedEditorUpdateAction
    {
        private bool _started;
        private float _startTime;
        private readonly float _delay;
        private readonly CallbackFunction _callbackFunction;

        /// <param name="delay">When set to 0 the callback will trigger on the next editor update</param>
        public DelayedEditorUpdateAction(float delay, CallbackFunction callbackFunction, bool start = false)
        {
            _delay = delay;
            _callbackFunction = callbackFunction;
            if (start) Start();
        }

        public void StartOrRefresh()
        {
            if (!_started)
            {
                Start();
            }
            else
            {
                RefreshDelayedAction();
            }
        }

        private void Start()
        {
            _startTime = Time.realtimeSinceStartup;
            EditorApplication.update += DelayedAction;
            _started = true;
        }

        private void RefreshDelayedAction()
        {
            _startTime = Time.realtimeSinceStartup;
        }

        public void Stop()
        {
            EditorApplication.update -= DelayedAction;
            _started = false;
        }

        private void DelayedAction()
        {
            if (Time.realtimeSinceStartup - _startTime < _delay) return;
            _callbackFunction?.Invoke();
            Stop();
        }
    }
}
